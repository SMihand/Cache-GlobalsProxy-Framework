using InterSystems.Globals;
using System;
using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace CacheEXTREME2.WProxyGlobal
{
    public class CacheProxySerializer
    {

        private List<ValueMeta> entityFieldsMeta;
        private List<ValueMeta> metaAsVectorList;
        private List<ValueMeta> entityKeysMeta;
        private List<ValueMeta> entityValuesMeta;
        //
        public FieldInfo[] entityFieldsInfo;
        //
        public FieldInfo[] entityKeysFieldsInfo;
        public FieldInfo[] entityValuesFieldsInfo;
        //
        private Type proxyT;
        //
        private ArrayList vectorizedKeyHolder;
        //
        private ConstructorInfo entityDefaultConstructor;
        private object constructedEntity;
        //
        private Connection conn;
        //
        private static int valueListsCounter;
        private static List<ValueList> valuesListsHolder;

        private static ValueList getHoldedValueList(Connection conn)
        {
            if (valueListsCounter >= valuesListsHolder.Count)
            {
                valuesListsHolder.Add(conn.CreateList());
            }
            return valuesListsHolder[valueListsCounter++];
        }
        public void ClearValuesListHolders()
        {
            foreach (ValueList vl in valuesListsHolder)
            {
                vl.Clear();
            }
            valueListsCounter = 0;
            foreach (ClearHolderDelegate del in holdersCleaners)
            {
                del.Invoke();
            }
        }

        //
        public delegate void ClearHolderDelegate();
        private List<ClearHolderDelegate> holdersCleaners;

        //
        public List<IStructManager> structsManagers;
        //
        public CacheProxySerializer(Type proxyT, List<ValueMeta> keysMeta, List<ValueMeta> valuesMeta, List<IStructManager> structsManagers, Connection conn)
        {
            this.conn = conn;
            this.proxyT = proxyT;
            this.entityFieldsMeta = new List<ValueMeta>(keysMeta);
            this.entityFieldsMeta.AddRange(valuesMeta);
            this.entityKeysMeta = new List<ValueMeta>(keysMeta);
            this.entityValuesMeta = new List<ValueMeta>(valuesMeta);
            //
            this.entityFieldsInfo = new FieldInfo[entityFieldsMeta.Count];
            for (int i = 0; i < entityFieldsMeta.Count; i++)
            {
                entityFieldsInfo[i] = proxyT.GetField(entityFieldsMeta[i].SemanticName);
            }
            //
            this.entityKeysFieldsInfo = new FieldInfo[keysMeta.Count];
            for (int i = 0; i < keysMeta.Count; i++)
            {
                entityKeysFieldsInfo[i] = proxyT.GetField(keysMeta[i].SemanticName);
            }
            //
            this.entityValuesFieldsInfo = new FieldInfo[valuesMeta.Count];
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                entityValuesFieldsInfo[i] = proxyT.GetField(valuesMeta[i].SemanticName);
            }
            entityDefaultConstructor = null;
            ConstructorInfo[] constructrors = proxyT.GetConstructors();
            foreach (ConstructorInfo constructor in constructrors)
            {
                if (constructor.GetParameters().Length == 0)
                {
                    this.entityDefaultConstructor = constructor;
                    this.constructedEntity = this.entityDefaultConstructor.Invoke(new object[]{});
                    break;
                }
            }
            if (entityDefaultConstructor == null)
            {
                throw new MissingMemberException(proxyT.Name, proxyT.Name + "()");
            }
            //
            metaAsVectorList = new List<ValueMeta>();
            vectorizeMeta(metaAsVectorList, entityFieldsMeta);
            //
            this.structsManagers = structsManagers;
            //
            initHolders();
            //
        }
        private void vectorizeMeta(List<ValueMeta> metaVector, List<ValueMeta> valuesMeta)
        {
            foreach (ValueMeta valueMeta in valuesMeta)
            {
                switch (valueMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_INT:
                        metaVector.Add(valueMeta);
                        break;
                    case ExtremeTypes.EXTREME_STRING:
                        metaVector.Add(valueMeta);
                        break;
                    case ExtremeTypes.EXTREME_DOUBLE:
                        metaVector.Add(valueMeta);
                        break;
                    case ExtremeTypes.EXTREME_STRUCT:
                        vectorizeMeta(metaVector, (valueMeta as StructValMeta).structDefinition.elementsMeta);
                        break;
                }
            }

        }


        private void initHolders()
        {
            vectorizedKeyHolder = new ArrayList();
            createVectorAListKeyHolder(vectorizedKeyHolder, this.entityKeysMeta);
            //
            valuesListsHolder = new List<ValueList>();
            valuesListsHolder.Add(conn.CreateList());
            initValuesListsHolder();
            //
            holdersCleaners = new List<ClearHolderDelegate>();
        }
        //
        private void initValuesListsHolder()
        {
            for (int i = 0; i < entityFieldsMeta.Count; i++)
            {
                switch (entityFieldsMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        valuesListsHolder.Add(conn.CreateList());
                        holdRecursiveList(entityFieldsMeta[i] as ListValMeta, 5);
                        break;
                    case ExtremeTypes.EXTREME_STRUCT:
                        continue;
                    default:
                        continue;
                }
            }
        }
        private void holdRecursiveList(ListValMeta list, int recLevel)
        {
            ValueMeta lstElem = list.ElemMeta;
            if (lstElem.ExtremeType == ExtremeTypes.EXTREME_LIST && recLevel > 0)
            {
                valuesListsHolder.Add(conn.CreateList());
                holdRecursiveList(lstElem as ListValMeta, recLevel - 1);
            }
        }

        private ArrayList createVectorAListKeyHolder(ArrayList serialized, IList<ValueMeta> meta)
        {
            for (int i = 0; i < meta.Count; i++)
            {
                switch (meta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            createVectorAListKeyHolder(serialized, (meta[i] as StructValMeta).structDefinition.elementsMeta);
                            break;
                        }
                    case ExtremeTypes.EXTREME_INT:
                        {
                            serialized.Add((int)777);
                            break;
                        }
                    case ExtremeTypes.EXTREME_DOUBLE:
                        {
                            serialized.Add((double)777);
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            serialized.Add("string");
                            break;
                        }
                    default:
                        {
                            return serialized;
                        }
                }
            }
            return serialized;
        }
        

        //Serialize work
        public ValueList SerializeValues(object theEntity, Connection conn)
        {
            ClearHolderDelegate clearHolder = ClearValuesListHolders;
            return SerializeValues(theEntity, conn, ref clearHolder);
        }
        public ValueList SerializeValues(object theEntity, Connection conn, ref ClearHolderDelegate holderCleaner)
        {
            holderCleaner = ClearValuesListHolders;

            ValueList serialized = CacheProxySerializer.getHoldedValueList(conn);
            for (int i = 0; i < this.entityValuesFieldsInfo.Length; i++)
            {
                switch (entityValuesMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        ValueList subValueList = CacheProxySerializer.getHoldedValueList(conn);
                        createValueListTyped(subValueList, entityValuesFieldsInfo[i].GetValue(theEntity) as IList, (entityValuesMeta[i] as ListValMeta).ElemMeta);
                        serialized.Append(subValueList);
                        break;
                    case ExtremeTypes.EXTREME_STRUCT:
                        CacheProxySerializer structSerializer = structsManagers[(entityValuesMeta[i] as StructValMeta).structDefinition.StructId].GetNewSerializer();
                        ValueList subStructValueList = structSerializer.SerializeValues(entityValuesFieldsInfo[i].GetValue(theEntity), conn);
                        //ValueList subStructValueList = structsManagers[(entityFieldsMeta[i] as StructValMeta).structDefinition.StructId].SerializeAsComplexVList(entityFieldsInfo[i].GetValue(theEntity), conn);
                        serialized.Append(subStructValueList);
                        break;
                    default:
                        serialized.Append(entityValuesFieldsInfo[i].GetValue(theEntity));
                        break;
                }
            }
            return serialized;
        }
        private void createValueListTyped(ValueList list, IList values, ValueMeta valueMeta)
        {
            for (int i = 0; i < values.Count; i++)
            {
                switch (valueMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        ValueList valueListT = CacheProxySerializer.getHoldedValueList(conn);
                        createValueListTyped(valueListT, values[i] as IList, (valueMeta as ListValMeta).ElemMeta);
                        list.Append(valueListT);
                        continue;
                    case ExtremeTypes.EXTREME_STRUCT:
                        //ValueList valueListS = createValueList(conn, values[i] as IList, (valueMeta as StructValMeta).elementsMeta);
                        CacheProxySerializer structSerializer = structsManagers[(valueMeta as StructValMeta).structDefinition.StructId].GetNewSerializer();
                        ValueList serizlizedStruct = structSerializer.SerializeValues(values[i], conn);
                        //ValueList valueListS = structsManagers[(valueMeta as StructValMeta).structDefinition.StructId].SerializeAsComplexVList(values[i], conn);
                        list.Append(serizlizedStruct);
                        continue;
                    default:
                        list.Append(values[i]);
                        continue;
                }
                throw new ArgumentException("Lists and primitives are available", "values");
            }
        }
        //
        public ArrayList SerializeKeysPart(object theEntity)
        {
            for (int mi = 0, hi = 0; mi < this.entityKeysFieldsInfo.Length; mi++)
            {
                switch (entityKeysMeta[mi].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            //Seems it works well
                            // i dont thisnk so
                            ArrayList serializedStruct = structsManagers[(entityKeysMeta[mi] as StructValMeta).structDefinition.StructId]
                                .SerializeAsKeys(this.entityKeysFieldsInfo[mi].GetValue(theEntity));
                            for (int j = 0; j < serializedStruct.Count; j++, hi++)
                            {
                                // cause here we are copying reference, not a value
                                vectorizedKeyHolder[hi] = serializedStruct[j];
                            }
                            //entityFieldsAsKeysHolder[hi] = serializedStruct
                            break;
                        }
                    default:
                        {
                            vectorizedKeyHolder[hi] = this.entityKeysFieldsInfo[mi].GetValue(theEntity);
                            hi++;
                            break;
                        }
                }
            }
            return vectorizedKeyHolder;
        }



        //Deserialize work
        public object Deserialize(IList values)
        {
            for (int i = 0; i < entityFieldsMeta.Count; i++)
            {
                initValueField(constructedEntity, entityFieldsInfo, i, values[i], entityFieldsMeta[i]);
            }
            return constructedEntity;
        }
        //
        public void DeserializeValues(object theDefaultConstructedEntity, IList values)
        {
            for (int i = 0, j = entityKeysMeta.Count; i < this.entityValuesMeta.Count; i++, j++)
            {
                initValueField(theDefaultConstructedEntity, entityValuesFieldsInfo, i, values[i], entityValuesMeta[i]);
            }
        }
        public void DeserializeKeys(object theDefaultConstructedEntity, ref ArrayList values, ref int startPosition)
        {
            for (int i = 0; i < this.entityKeysMeta.Count; i++)
            {
                switch (entityKeysMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            object structEntity = structsManagers[(entityKeysMeta[i] as StructValMeta).structDefinition.StructId].CreateStructEntity();
                            structsManagers[(entityKeysMeta[i] as StructValMeta).structDefinition.StructId].GetNewSerializer().DeserializeKeys(structEntity, ref values, ref startPosition);
                            //entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, structsManagers[(keysMeta[i] as StructValMeta).StructId].CreateStructEntity((IList)values));
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, structEntity);
                            break;
                        }
                    case ExtremeTypes.EXTREME_DOUBLE:
                        {
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, double.Parse(values[startPosition].ToString()));
                            startPosition++;
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, values[startPosition].ToString());
                            startPosition++;
                            break;
                        }
                    default:
                        {
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, values[startPosition]);
                            startPosition++;
                            break;
                        }
                }
            }
        }
        //  
        private void initValueField(object entity, FieldInfo[] valuesFields, int valueIndex, object value, ValueMeta valueMeta)
        {
            switch (valueMeta.ExtremeType)
            {
                case ExtremeTypes.EXTREME_LIST:
                    {
                        initGenericListField(
                            (IList)valuesFields[valueIndex].GetValue(entity)
                            , (valueMeta as ListValMeta).ElemMeta
                            , (IList)value
                        );
                        break;
                    }
                case ExtremeTypes.EXTREME_STRUCT:
                    {
                        valuesFields[valueIndex].SetValue(entity
                            , structsManagers[(valueMeta as StructValMeta).structDefinition.StructId].CreateStructEntity(value as ArrayList));
                        break;
                    }
                /*MODIFIED to parse keys values, because string key in global has no diference with number field*/
                case ExtremeTypes.EXTREME_DOUBLE:
                    {
                        valuesFields[valueIndex].SetValue(entity, double.Parse(value.ToString()));
                        break;
                    }
                case ExtremeTypes.EXTREME_STRING:
                    {
                        valuesFields[valueIndex].SetValue(entity, value.ToString());
                        break;
                    }
                /*is added double and string*/
                default:
                    {
                        valuesFields[valueIndex].SetValue(entity, value);
                        return;
                    }
            }
        }
        private void initGenericListField(IList genericList, ValueMeta elemMeta, IList values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                object toAdd = values[i];
                switch (elemMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        {
                            Type subListType = genericList.GetType().GetGenericArguments()[0];
                            object subList = subListType.GetConstructors()[0].Invoke(new object[] { });
                            initGenericListField(
                                subList as IList
                                , (elemMeta as ListValMeta).ElemMeta
                                , values[i] as IList
                            );
                            genericList.Add(subList as IList);
                            continue;
                        }
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            genericList.Add(structsManagers[(elemMeta as StructValMeta).structDefinition.StructId].CreateStructEntity(toAdd as IList));
                            break;
                        }
                    default:
                        {
                            genericList.Add(toAdd);
                            break;
                        }
                }
            }
        }
    }
}
