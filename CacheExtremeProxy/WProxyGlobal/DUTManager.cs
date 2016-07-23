using InterSystems.Globals;
using System;
using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CacheEXTREME2.WProxyGlobal
{
    public class DUTManager<KeyT, ProxyT> where ProxyT: class where KeyT : class
    {
        private TrueNodeReference globalRef;
        //
        private Type nodeType;
        private Type keyType;
        private FieldInfo[] proxyTfields;
        private FieldInfo keysField;
        private FieldInfo[] valuesFields;
        //
        private StructValMeta keyMeta;
        private List<ValueMeta> valuesMeta;
        //
        private ConstructorInfo nodeFullConstructor;
        private ConstructorInfo nodeDefaultConstructor;
        private MethodInfo nodeCustomValidator;
        //
        private List<IStructManager> structsManagers;
        private CacheProxySerializer serializer;
        private KeyStructSerializer<KeyT> keySerializer;
        //
        private ProxyT[] methodParam = new ProxyT[1];
        public bool Validate = false;

        public DUTManager(StructValMeta keysStruct, List<ValueMeta> valuesMeta, TrueNodeReference globalRef, List<IStructManager> structsManagers = null)
        {
            this.globalRef = new TrueNodeReference(globalRef);
            this.nodeType = typeof(ProxyT);
            this.keyType = typeof(KeyT);
            this.keyMeta = keysStruct;
            this.valuesMeta = valuesMeta;
            this.structsManagers = structsManagers;
            this.proxyTfields = nodeType.GetFields();
            this.nodeFullConstructor = nodeType.GetConstructors()[0];
            this.nodeDefaultConstructor = nodeType.GetConstructors()[1];
            this.nodeCustomValidator = nodeType.GetMethod(nodeType.Name + "Validator");
            initNodeValuesFieldsAndHolders();
            this.keysField = nodeType.GetField(keysStruct.SemanticName);
            this.serializer = new CacheProxySerializer(keyType.GetFields(), valuesFields
                , keysStruct.elementsMeta , valuesMeta
                , structsManagers);
            //HARDcODED serializer DEPRECATED
            this.keySerializer = new KeyStructSerializer<KeyT>(
                keysStruct.elementsMeta
                , structsManagers);
        }

        private void initNodeValuesFieldsAndHolders()
        {
            valuesFields = new FieldInfo[valuesMeta.Count];
            for (int i = 0; i < valuesMeta.Count; i++)
            {
                valuesFields[i] = nodeType.GetField(valuesMeta[i].SemanticName);
            }
        }
        //
        //
        public void Save(ProxyT entity)
        {
            if (Validate)
            {
                methodParam[0] = entity;
                if (!(bool)this.nodeCustomValidator.Invoke(entity, methodParam))
                {
                    throw new Exception("CustomValidationException! in method " + nodeType.Name + "Validator");
                }
                try
                {
                    IList keys = keysField.GetValue(entity) as IList;
                    for (int i = 0; i < keys.Count; i++)
                    {
                        this.keyMeta.Validate(keys[i]);
                    }
                    for (int i = 0; i < valuesFields.Length; i++)
                    {
                        valuesMeta[i].Validate(valuesFields[i].GetValue(entity));
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("In " + keysField.Name + ":\n" + ex.Message, ex);
                }
            }
            IList values = (IList)keysField.GetValue(entity);
            ArrayList keysHolders = new ArrayList();
            for (int i = 0; i < values.Count; i++)
            {
                keysHolders.AddRange(structsManagers[this.keyMeta.StructId].GetSerializer().SerializeStructedKey(values[i]));
            }
            globalRef.SetValues(keysHolders, serializer.SerializeValues(entity));
        }

        public ProxyT Get(List<KeyT> keys)
        {

            ArrayList keysHolders = new ArrayList();
            for (int i = 0; i < keys.Count; i++)
            {
                keysHolders.AddRange(structsManagers[this.keyMeta.StructId].GetSerializer().SerializeStructedKey(keys[i]));
            }
            globalRef.SetSubscripts(keysHolders);
            if (globalRef.HasValues())
            {
                ArrayList values = globalRef.GetValues(valuesMeta);
                return createEntity(keysHolders, values);
            }

            return null;
        }

        public List<ProxyT> TrueGet(List<KeyT> keys)
        {
            List<ProxyT> result = new List<ProxyT>();
            ArrayList keysHolders = new ArrayList();
            for (int i = 0; i < keys.Count; i++)
            {
                keysHolders.AddRange(keySerializer.Serialize(keys[i]));
            }
            globalRef.Reset();
            treeWalkForEntities(keysHolders, result);
            return result;
        }

        public void Kill(List<KeyT> keys)
        {
            ArrayList keysHolders = new ArrayList();
            for (int i = 0; i < keys.Count; i++)
            {
                keysHolders.AddRange(structsManagers[this.keyMeta.StructId].GetSerializer().SerializeStructedKey(keys[i]));
            }
            globalRef.SetSubscripts(keysHolders);
            globalRef.Kill();
        }

        private ProxyT createEntity(ArrayList keys, ArrayList values)
        {
            ProxyT entity = (ProxyT)nodeDefaultConstructor.Invoke(new object[] { });
            serializer.DeserializeValues(entity, values);
            //
            Queue keysQueue = new Queue(keys);
            do
            {
                KeyT key = (KeyT)keyType.GetConstructors()[0].Invoke(new object[] { });
                //keySerializer.Deserialize(key, keys);
                serializer.DeserializeStructedKeys(key, keysQueue);
                ((IList)keysField.GetValue(entity)).Add(key);
            } while (keysQueue.Count != 0);
            return entity;
        }

        private void treeWalkForEntities(ArrayList baseSubscripts, List<ProxyT> entities)
        {
            globalRef.AppendSubscript("");
            if (baseSubscripts[globalRef.SubsCount - 1] != null)
            {
                if (!baseSubscripts[globalRef.SubsCount - 1].ToString().Equals(String.Empty))
                {
                    globalRef.SetSubscript(globalRef.SubsCount, baseSubscripts[globalRef.SubsCount - 1]);
                    if (globalRef.SubsCount == baseSubscripts.Count)
                    {
                        if (globalRef.HasValues())
                        {
                            entities.Add(createEntity(globalRef.GetSubscripts(), globalRef.GetValues(valuesMeta)));
                        }
                        return;
                    }
                    treeWalkForEntities(baseSubscripts, entities);
                    globalRef.GoParentNodeSubscripts();
                    return;
                }
            }
            while (globalRef.NextSubscript() != "")
            {
                globalRef.GoNextSubscript();
                if (globalRef.SubsCount == baseSubscripts.Count && globalRef.HasValues())
                {
                    entities.Add(createEntity(globalRef.GetSubscripts(), globalRef.GetValues(valuesMeta)));
                    continue;
                }
                if (globalRef.SubsCount < baseSubscripts.Count && globalRef.HasSubnodes())
                {
                    treeWalkForEntities(baseSubscripts, entities);
                    globalRef.GoParentNodeSubscripts();
                }
            }
        }
    }


    public class KeyStructSerializer<KeyT>
    {
        private FieldInfo[] keyFields;
        private List<ValueMeta> keyFieldsMeta;
        private List<IStructManager> structsManagers;

        public KeyStructSerializer(List<ValueMeta> keyFieldsMeta, List<IStructManager> structsManagers = null)
        {
            this.keyFields = typeof(KeyT).GetFields();
            this.keyFieldsMeta = keyFieldsMeta;
            this.structsManagers = structsManagers;
        }

        public ArrayList Serialize(object theKey)
        {
            ArrayList serialized = new ArrayList();
            serialize(theKey, keyFieldsMeta, serialized);
            return serialized;
        }

        public void Deserialize(object theKey, IList values)
        {
            for (int i = 0; i < keyFieldsMeta.Count; i++)
            {
                initKeyValueField(theKey, keyFields, i, values, keyFieldsMeta[i]);
            }
        }
        //
        //
        private void serialize(object obj, List<ValueMeta> meta, ArrayList serialized)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object value = fields[i].GetValue(obj);
                switch (meta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            serialize(value, (meta[i] as StructValMeta).elementsMeta, serialized);
                            break;
                        }
                    default:
                        {
                            serialized.Add(value);
                            break;
                        }
                }
            }
        }
        //
        private void initKeyValueField(object entity, FieldInfo[] valuesFields, int valueIndex, IList values, ValueMeta valueMeta)
        {
            switch (valueMeta.ExtremeType)
            {
                case ExtremeTypes.EXTREME_STRUCT:
                    {
                        valuesFields[valueIndex].SetValue(entity, structsManagers[(valueMeta as StructValMeta).StructId].CreateStructEntity(values));
                        for (int i = 0; i < (valueMeta as StructValMeta).elementsMeta.Count; i++)
                        {
                            values.RemoveAt(0);
                        }
                        break;
                    }
                case ExtremeTypes.EXTREME_DOUBLE:
                    {
                        valuesFields[valueIndex].SetValue(entity, double.Parse(values[0].ToString()));
                        values.RemoveAt(0);
                        break;
                    }
                case ExtremeTypes.EXTREME_STRING:
                    {
                        valuesFields[valueIndex].SetValue(entity, values[0].ToString());
                        values.RemoveAt(0);
                        break;
                    }
                default:
                    {
                        valuesFields[valueIndex].SetValue(entity, values[0]);
                        values.RemoveAt(0);
                        return;
                    }
            }
        }
    }
}
