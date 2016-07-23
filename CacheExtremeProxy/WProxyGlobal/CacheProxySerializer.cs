using InterSystems.Globals;
using System;
using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CacheEXTREME2.WProxyGlobal
{
    public class CacheProxySerializer
    {
        public FieldInfo[] entityFieldsInfo;
        public FieldInfo[] entityKeysFieldsInfo;
        public FieldInfo[] entityValuesFieldsInfo;
        public EntityMeta entityMeta;
        private List<ValueMeta> keysMeta;
        private List<ValueMeta> valuesMeta;
        //
        public List<IStructManager> structsManagers;
        //
        public CacheProxySerializer(FieldInfo[] entityFieldsInfo, EntityMeta entityMeta, List<IStructManager> structsManagers)
            : this(entityFieldsInfo, entityMeta.KyesMeta, entityMeta.ValuesMeta, structsManagers)
        {
            this.entityMeta = entityMeta;
        }
        public CacheProxySerializer(FieldInfo[] entityFieldsInfo, List<ValueMeta> keysMeta, List<ValueMeta> valuesMeta, List<IStructManager> structsManagers)
        {
            this.entityFieldsInfo = entityFieldsInfo;
            this.keysMeta = keysMeta;
            this.valuesMeta = valuesMeta;
            //
            this.entityKeysFieldsInfo = new FieldInfo[keysMeta.Count];
            for (int i = 0; i < keysMeta.Count; i++)
            {
                this.entityKeysFieldsInfo[i] = entityFieldsInfo[i];
            }
            this.entityValuesFieldsInfo = new FieldInfo[valuesMeta.Count];
            for (int i = keysMeta.Count, j = 0; j < valuesMeta.Count; i++, j++)
            {
                this.entityValuesFieldsInfo[j] = entityFieldsInfo[i];
            }
            //
            this.structsManagers = structsManagers;
        }
        public CacheProxySerializer(FieldInfo[] entityKeysFieldsInfo, FieldInfo[] entityValuesFieldsInfo, List<ValueMeta> keysMeta, List<ValueMeta> valuesMeta, List<IStructManager> structsManagers)
        {
            this.entityFieldsInfo = new FieldInfo[entityKeysFieldsInfo.Length + entityValuesFieldsInfo.Length];
            entityKeysFieldsInfo.CopyTo(entityFieldsInfo, 0);
            entityValuesFieldsInfo.CopyTo(entityFieldsInfo, entityKeysFieldsInfo.Length);
            this.entityKeysFieldsInfo = entityKeysFieldsInfo;
            this.entityValuesFieldsInfo = entityValuesFieldsInfo;
            this.keysMeta = keysMeta;
            this.valuesMeta = valuesMeta;
            this.structsManagers = structsManagers;
        }
        //
        private void initHolders()
        {
        }
        //
        public ArrayList Serialize(object theEntity)
        {
            ArrayList serialized = new ArrayList(keysMeta.Count + valuesMeta.Count);
            serialized.AddRange(SerializeKeys(theEntity));
            serialized.AddRange(SerializeValues(theEntity));
            return serialized;
        }
        public ArrayList SerializeKeys(object theEntity)
        {
            ArrayList serialized = new ArrayList(keysMeta.Count);
            for (int i = 0; i < keysMeta.Count; i++)
            {
                object fieldVal = entityKeysFieldsInfo[i].GetValue(theEntity);
                serialized.Add(fieldVal);
            }
            return serialized;
        }
        public ArrayList SerializeValues(object theEntity)
        {
            ArrayList serialized = new ArrayList(valuesMeta.Count);
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                //object fieldVal = entityValuesFieldsInfo[i].GetValue(theEntity);
                switch (valuesMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        {
                            ArrayList subList = new ArrayList();
                            serializeList(subList, (valuesMeta[i] as ListValMeta).ElemMeta
                                , entityValuesFieldsInfo[i].GetValue(theEntity) as IList);
                            serialized.Add(subList);
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            serialized.Add(structsManagers[(valuesMeta[i] as StructValMeta).StructId]
                                .Serialize(entityValuesFieldsInfo[i].GetValue(theEntity)));
                            break;
                        }
                    default:
                        {
                            serialized.Add(entityValuesFieldsInfo[i].GetValue(theEntity));
                            break;
                        }
                }
            }
            return serialized;
        }
        public void Deserialize(object theDefaultConstructedEntity, IList values)
        {
            for (int i = 0; i < keysMeta.Count; i++)
            {
                initValueField(theDefaultConstructedEntity, entityKeysFieldsInfo, i, values[i], keysMeta[i]);
            }
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                initValueField(theDefaultConstructedEntity, entityValuesFieldsInfo, i, values[i], valuesMeta[i]);
            }
        }
        public void DeserializeKeys(object theDefaultConstructedEntity, IList values)
        {
            for (int i = 0; i < keysMeta.Count; i++)
            {
                initValueField(theDefaultConstructedEntity, entityKeysFieldsInfo, i, values[i], keysMeta[i]);
            }
        }
        public void DeserializeValues(object theDefaultConstructedEntity, IList values)
        {
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                initValueField(theDefaultConstructedEntity, entityValuesFieldsInfo, i, values[i], valuesMeta[i]);
            }
        }
        //
        public ArrayList SerializeKeyWithStructsAsValues(object theEntity)
        {
            ArrayList serialized = new ArrayList();
            for (int i = 0; i < this.entityKeysFieldsInfo.Length; i++)
            {
                //object value = this.entityKeysFieldsInfo[i].GetValue(obj);
                switch (keysMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            ArrayList subList = structsManagers[(keysMeta[i] as StructValMeta).StructId].Serialize(this.entityKeysFieldsInfo[i].GetValue(theEntity));
                            serialized.Add(subList);
                            break;
                        }
                    default:
                        {
                            serialized.Add(this.entityKeysFieldsInfo[i].GetValue(theEntity));
                            break;
                        }
                }
            }
            return serialized;
        }
        public ArrayList SerializeStructedKey(object theEntity)
        {
            ArrayList serialized = new ArrayList();
            serializeKeys(theEntity, serialized);
            return serialized;
        }
        public void DeserializeStructedKeys(object theDefaultConstructedEntity, Queue values)
        {
            for (int i = 0; i < this.keysMeta.Count; i++)
            {
                switch (keysMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            object structEntity = structsManagers[(keysMeta[i] as StructValMeta).StructId].CreateStructEntity();
                            structsManagers[(keysMeta[i] as StructValMeta).StructId].GetSerializer().DeserializeStructedKeys(structEntity,values);
                            //entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, structsManagers[(keysMeta[i] as StructValMeta).StructId].CreateStructEntity((IList)values));
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, structEntity);
                            break;
                        }
                    case ExtremeTypes.EXTREME_DOUBLE:
                        {
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, double.Parse(values.Dequeue().ToString()));
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, values.Dequeue().ToString());
                            break;
                        }
                    default:
                        {
                            entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, values.Dequeue());
                            break;
                        }
                }
            }
        }
        //Serialize work
        private void serializeList(IList list, ValueMeta elemMeta, IList values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                //object toAdd = values[i];
                switch (elemMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        {
                            ArrayList subList = new ArrayList();
                            serializeList(
                                subList as IList
                                , (elemMeta as ListValMeta).ElemMeta
                                , values[i] as IList
                            );
                            list.Add(subList as IList);
                            continue;
                        }
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            list.Add(structsManagers[(elemMeta as StructValMeta).StructId].Serialize(values[i]));
                            break;
                        }
                    default:
                        {
                            list.Add(values[i]);
                            break;
                        }
                }
            }
        }
        private void serializeKeys(object obj, ArrayList serialized)
        {
            for (int i = 0; i < this.entityKeysFieldsInfo.Length; i++)
            {
                //object value = this.entityKeysFieldsInfo[i].GetValue(obj);
                switch (keysMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            serialized.AddRange(structsManagers[(keysMeta[i] as StructValMeta).StructId]
                                .GetSerializer().SerializeStructedKey(this.entityKeysFieldsInfo[i].GetValue(obj)));
                            break;
                        }
                    default:
                        {
                            serialized.Add(this.entityKeysFieldsInfo[i].GetValue(obj));
                            break;
                        }
                }
            }
        }
        //Deserialize work
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
                            , structsManagers[(valueMeta as StructValMeta).StructId].CreateStructEntity(value as ArrayList));
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
                            genericList.Add(structsManagers[(elemMeta as StructValMeta).StructId].CreateStructEntity(toAdd as IList));
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

    public class CacheProxySerializer<ProxyT>
    {
        private List<ValueMeta> entityFieldsMeta;
        private List<ValueMeta> metaAsVectorList;
        public FieldInfo[] entityFieldsInfo;
        //
        private ArrayList asVectorAListHolder;
        //
        private ConstructorInfo entityDefaultConstructor;
        private ProxyT constructedEntity;
        //
        public List<IStructManager> structsManagers;
        //
        public CacheProxySerializer(List<ValueMeta> entityFieldsMeta, List<IStructManager> structsManagers)
        {
            this.entityFieldsInfo = new FieldInfo[entityFieldsMeta.Count];
            for (int i = 0; i < entityFieldsMeta.Count; i++)
            {
                entityFieldsInfo[i] = typeof(ProxyT).GetField(entityFieldsMeta[i].SemanticName);
            }
            entityDefaultConstructor = null;
            ConstructorInfo[] constructrors = typeof(ProxyT).GetConstructors();
            foreach (ConstructorInfo constructor in constructrors)
            {
                if (constructor.GetParameters().Length == 0)
                {
                    this.entityDefaultConstructor = constructor;
                    this.constructedEntity = (ProxyT)this.entityDefaultConstructor.Invoke(new object[]{});
                    break;
                }
            }
            if (entityDefaultConstructor == null)
            {
                throw new MissingMemberException(typeof(ProxyT).Name, typeof(ProxyT).Name + "()");
            }
            //
            this.entityFieldsMeta = entityFieldsMeta;
            metaAsVectorList = new List<ValueMeta>();
            vectorizeMeta(metaAsVectorList, entityFieldsMeta);
            //
            this.structsManagers = structsManagers;
            //
            initHolders();
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
                        vectorizeMeta(metaVector, (valueMeta as StructValMeta).elementsMeta);
                        break;
                }
            }

        }
        //
        private void initHolders()
        {
            asVectorAListHolder = new ArrayList();
            createVectorAListHolder(asVectorAListHolder, this.entityFieldsMeta);
        }
        private ArrayList createVectorAListHolder(ArrayList serialized, IList<ValueMeta> meta)
        {
            for (int i = 0; i < meta.Count; i++)
            {
                switch (meta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            createVectorAListHolder(serialized,(meta[i] as StructValMeta).elementsMeta);
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
        //
        //Serialize work
        public ValueList Serialize(object theEntity, Connection conn)
        {
            ValueList serizlized = conn.CreateList();
            for (int i = 0; i < entityFieldsInfo.Length; i++)
            {
                switch (entityFieldsMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        serizlized.Append(createValueListTyped(conn, entityFieldsInfo[i].GetValue(theEntity) as IList, (entityFieldsMeta[i] as ListValMeta).ElemMeta));
                        break;
                    case ExtremeTypes.EXTREME_STRUCT:
                        serizlized.Append(structsManagers[(entityFieldsMeta[i] as StructValMeta).StructId].SerializeAsComplexVList(entityFieldsInfo[i].GetValue(theEntity), conn));    
                        break;
                    default:
                        serizlized.Append(entityFieldsInfo[i].GetValue(theEntity));
                        break;
                }
            }
            return serizlized;
        }   
        private ValueList createValueListTyped(Connection conn, IList values, ValueMeta valueMeta)
        {
            ValueList list = values.Count != 0 ? conn.CreateList(values.Count) : conn.CreateList();
            for (int i = 0; i < values.Count; i++)
            {
                switch (valueMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        ValueList valueListT = createValueListTyped(conn, values[i] as IList, (valueMeta as ListValMeta).ElemMeta);
                        list.Append(valueListT);
                        continue;
                    case ExtremeTypes.EXTREME_STRUCT:
                        //ValueList valueListS = createValueList(conn, values[i] as IList, (valueMeta as StructValMeta).elementsMeta);
                        ValueList valueListS = structsManagers[(valueMeta as StructValMeta).StructId].SerializeAsComplexVList(values[i], conn);                    
                        list.Append(valueListS);
                        continue;
                    default:
                        list.Append(values[i]);
                        continue;
                }
                throw new ArgumentException("Lists and primitives are available", "values");
            }
            return list;
        }
        //
        public ArrayList SerializeAsKeys(object theEntity)
        {
            for (int mi = 0, hi = 0; mi < this.entityFieldsInfo.Length; mi++)
            {
                switch (entityFieldsMeta[mi].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            //Seem it works well
                            ArrayList serializedStruct = structsManagers[(entityFieldsMeta[mi] as StructValMeta).StructId]
                                .SerializeAsKeys(this.entityFieldsInfo[mi].GetValue(theEntity));
                            for (int j = 0; j < serializedStruct.Count; j++, hi++)
                            {
                                asVectorAListHolder[hi] = serializedStruct[j];
                            }
                            //entityFieldsAsKeysHolder[hi] = serializedStruct
                            break;
                        }
                    default:
                        {
                            asVectorAListHolder[hi] = this.entityFieldsInfo[mi].GetValue(theEntity);
                            hi++;
                            break;
                        }
                }
            }
            return asVectorAListHolder;
        }
        //
        //Deserialize work
        public ProxyT Deserialize(IList values)
        {
            for (int i = 0; i < entityFieldsMeta.Count; i++)
            {
                initValueField(constructedEntity, entityFieldsInfo, i, values[i], entityFieldsMeta[i]);
            }
            return constructedEntity;
        }
        public ProxyT DeserializeAsVectorPresented(Queue values)
        {
            for (int i = 0; i < this.metaAsVectorList.Count; i++)
            {
                switch (metaAsVectorList[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            object structEntity = structsManagers[(metaAsVectorList[i] as StructValMeta).StructId].CreateEntityFromVector(values);
                            structsManagers[(metaAsVectorList[i] as StructValMeta).StructId].GetSerializer().DeserializeStructedKeys(structEntity, values);
                            //entityKeysFieldsInfo[i].SetValue(theDefaultConstructedEntity, structsManagers[(keysMeta[i] as StructValMeta).StructId].CreateStructEntity((IList)values));
                            entityFieldsInfo[i].SetValue(constructedEntity, structEntity);
                            break;
                        }
                    case ExtremeTypes.EXTREME_DOUBLE:
                        {
                            entityFieldsInfo[i].SetValue(constructedEntity, double.Parse(values.Dequeue().ToString()));
                            break;
                        }
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            entityFieldsInfo[i].SetValue(constructedEntity, values.Dequeue().ToString());
                            break;
                        }
                    default:
                        {
                            entityFieldsInfo[i].SetValue(constructedEntity, values.Dequeue());
                            break;
                        }
                }
            }
            return constructedEntity;
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
                            , structsManagers[(valueMeta as StructValMeta).StructId].CreateStructEntity(value as ArrayList));
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
                            genericList.Add(structsManagers[(elemMeta as StructValMeta).StructId].CreateStructEntity(toAdd as IList));
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
