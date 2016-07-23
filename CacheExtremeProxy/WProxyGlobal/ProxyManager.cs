using InterSystems.Globals;
using System;
using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CacheEXTREME2.WProxyGlobal
{
    public class ProxyManager<ProxyT, ProxyKeyT> where ProxyT : class where ProxyKeyT : class
    {
        public bool Validate = true;
        private EntityMeta meta;
        private List<ValueMeta> keysMetaAsVectorList;
        private TrueNodeReference globalRef;
        private ArrayList keysHolder;
        private ArrayList valuesHolder;
        //
        //
        private Type proxyType;
        private Type proxyKeyType;
        private FieldInfo[] ProxyTfields;
        private FieldInfo[] ProxyKeyTfields;
        private ConstructorInfo fullConstructor;
        private ConstructorInfo defaultConstructor;
        //
        private List<IStructManager> structsManagers;
        //
        private FieldInfo[] keysFields;
        private FieldInfo[] valuesFields;
        private MethodInfo customValidator;
        private ProxyT[] methodParam = new ProxyT[1];
        //
        private CacheProxySerializer ProxySerializer;
        private CacheProxySerializer<ProxyT> newProxySerializer;
        private CacheProxySerializer ProxyKeysSerializer;
        private CacheProxySerializer<ProxyT> newProxyKeySerializer;
        //
        //
        public ProxyManager(EntityMeta meta, TrueNodeReference globalRef, List<IStructManager> structsManagers = null)
        {
            this.meta = meta;
            keysMetaAsVectorList = new List<ValueMeta>();
            vectorizeKey(keysMetaAsVectorList, meta.KyesMeta);
            this.globalRef = new TrueNodeReference(globalRef);
            proxyType = typeof(ProxyT);
            proxyKeyType = typeof(ProxyKeyT);
            ProxyTfields = proxyType.GetFields();
            fullConstructor = proxyType.GetConstructors()[0];
            defaultConstructor = proxyType.GetConstructors()[1];
            customValidator = proxyType.GetMethod(this.meta.EntityName + "Validator");
            initKeysValuesFields();
            initKeysValuesHolders();
            this.structsManagers = structsManagers;
            List<ValueMeta> keysValuesMeta = new List<ValueMeta>(meta.KyesMeta);
            this.ProxySerializer = new CacheProxySerializer(keysFields, valuesFields
                , meta.KyesMeta, meta.ValuesMeta
                , structsManagers);
            this.ProxyKeysSerializer = new CacheProxySerializer(ProxyKeyTfields, ProxyKeyTfields
                , meta.KyesMeta, meta.KyesMeta
                , structsManagers);
            //
            this.newProxySerializer = new CacheProxySerializer<ProxyT>(meta.ValuesMeta, structsManagers);
            this.newProxyKeySerializer = new CacheProxySerializer<ProxyT>(meta.KyesMeta, structsManagers);
        }
        private void initKeysValuesFields()
        {
            keysFields = new FieldInfo[meta.KyesMeta.Count];
            ProxyKeyTfields = new FieldInfo[meta.KyesMeta.Count];
            for (int i = 0; i < meta.KyesMeta.Count; i++)
            {
                keysFields[i] = proxyType.GetField(meta.KyesMeta[i].SemanticName);
                ProxyKeyTfields[i] = proxyKeyType.GetField(meta.KyesMeta[i].SemanticName);
            }
            valuesFields = new FieldInfo[meta.ValuesMeta.Count];
            for (int i = 0; i < meta.ValuesMeta.Count; i++)
            {
                valuesFields[i] = proxyType.GetField(meta.ValuesMeta[i].SemanticName);
            }
        }
        private void initKeysValuesHolders()
        {
            keysHolder = new ArrayList(meta.KyesMeta.Count);
            for (int i = 0; i < meta.KyesMeta.Count; i++)
            {
                keysHolder.Add(new object());
            }
            valuesHolder = new ArrayList(meta.ValuesMeta.Count);
            for (int i = 0; i < meta.ValuesMeta.Count; i++)
            {
                valuesHolder.Add(new object());
            }
        }
        private void vectorizeKey(List<ValueMeta> keysVector, List<ValueMeta> keysMeta)
        {
            foreach (ValueMeta keyMeta in keysMeta)
            {
                switch (keyMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_INT:
                        keysVector.Add(keyMeta);
                        break;
                    case ExtremeTypes.EXTREME_STRING:
                        keysVector.Add(keyMeta);
                        break;
                    case ExtremeTypes.EXTREME_DOUBLE:
                        keysVector.Add(keyMeta);
                        break;
                    case ExtremeTypes.EXTREME_STRUCT:
                        vectorizeKey(keysVector, (keyMeta as StructValMeta).elementsMeta);
                        break;
                }
            }

        }
        //
        public void Save(ProxyT entity)
        {
            if (Validate)
            {
                methodParam[0] = entity;
                if (!(bool)customValidator.Invoke(entity, methodParam))
                {
                    throw new Exception("CustomValidationException! in method " + meta.EntityName + "Validator");
                }
                try
                {
                    for (int i = 0; i < keysFields.Length; i++)
                    {
                        meta.KeysValidator[i].ValidateKey(keysFields[i].GetValue(entity));
                    }
                    for (int i = 0; i < valuesFields.Length; i++)
                    {
                        meta.ValuesMeta[i].Validate(valuesFields[i].GetValue(entity));
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("In " + this.meta.EntityName + ":\n" + ex.Message, ex);
                }
            }
            globalRef.Reset();
            //SLOW VERSION:
            //globalRef.SetSubscripts(this.ProxySerializer.SerializeStructedKey(entity));
            //globalRef.SetValues(this.ProxySerializer.SerializeValues(entity));

            //STABLE VERSION FASTER:
            //globalRef.AppendSubscripsts(this.ProxySerializer.SerializeStructedKey(entity),meta.KyesMeta);
            //globalRef.SetValues(this.ProxySerializer.SerializeValues(entity));
            
            //STABLE VERSION MORE FACSTER:
            //globalRef.AppendSubscripsts(this.ProxySerializer.SerializeKeyWithStructsAsValues(entity), meta.KyesMeta);
            //globalRef.SetValuesTyped(this.ProxySerializer.SerializeValues(entity), this.meta.ValuesMeta);

            //STABLE BETA VERSION (perfomance winner)
            globalRef.AppendSubscripsts(newProxyKeySerializer.SerializeAsKeys(entity), keysMetaAsVectorList);
            globalRef.SetValues(newProxySerializer.Serialize(entity, globalRef.Conn));
        }
        //
        public ProxyT Get(object key)
        {
            ProxyT entity = null;
            FieldInfo[] keyFields = key.GetType().GetFields();
            //vvvvvvvvvvvvvv VERY SLOW SOLUTION!!!! vvvvvvvvvvvvvvv//
            ArrayList kkkKeys = new CacheProxySerializer(keysFields, valuesFields
                , meta.KyesMeta, meta.ValuesMeta
                , structsManagers)
                .SerializeStructedKey(key);
            //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
            globalRef.SetSubscripts(kkkKeys);
            if (globalRef.HasValues())
            {
                ArrayList values = globalRef.GetValues(meta.ValuesMeta);
                //
                entity = CreateEntity(kkkKeys, values);
            }
            return entity;
        }
        public ProxyT Get(ProxyKeyT key)
        {
            ProxyT entity = null;
            ArrayList kkkKeys = ProxyKeysSerializer.SerializeStructedKey(key);
            globalRef.SetSubscripts(kkkKeys);
            if (globalRef.HasValues())
            {
                ArrayList values = globalRef.GetValues(meta.ValuesMeta);
                //
                entity = CreateEntity(kkkKeys, values);
            }
            return entity;
        }
        public ProxyT Get(ProxyT key)
        {
            ProxyT entity = null;
            ArrayList kkkKeys = newProxyKeySerializer.SerializeAsKeys(key);
            globalRef.SetSubscripts(kkkKeys);
            if (globalRef.HasValues())
            {
                ArrayList values = globalRef.GetValues(meta.ValuesMeta);
                //
                entity = CreateEntity(kkkKeys, values);
            }
            kkkKeys = null;
            return entity;
        }
        public ProxyT Get(ArrayList keys)
        {
            ProxyT entity = null;
            for (int i = 0; i < meta.KyesMeta.Count; i++)
            {
                ((IKeyValidator)this.keysMetaAsVectorList[i]).ValidateKey(keys[i]);
            }
            globalRef.SetSubscripts(keys);
            if (globalRef.HasValues())
            {
                ArrayList values = globalRef.GetValues(meta.ValuesMeta);
                //
                entity = CreateEntity(keys, values);
            }
            return entity;
        }
        //
        public List<ProxyT> GetByKeyMask(object key)
        {
            List<ProxyT> entities = new List<ProxyT>();
            FieldInfo[] keyFields = key.GetType().GetFields();
            ArrayList keys = new ArrayList();
            for (int i = 0; i < meta.KyesMeta.Count; i++)
            {
                object keyValue = keyFields[i].GetValue(key);
                if (keyValue != null)
                {
                    meta.KeysValidator[i].ValidateKey(keyValue);
                }
                keys.Add(keyValue);
            }
            globalRef.Reset();
            treeWalkForEntities(globalRef, keys, entities);
            return entities;
        }
        public List<ProxyT> GetByKeyMask(ProxyKeyT key)
        {
            List<ProxyT> entities = new List<ProxyT>();
            ArrayList keys = ProxyKeysSerializer.SerializeStructedKey(key);
            globalRef.Reset();
            treeWalkForEntities(globalRef, keys, entities);
            return entities;
        }
        public List<ProxyT> GetAll()
        {
            List<ProxyT> entities = new List<ProxyT>();
            globalRef.Reset();

            treeWalkForEntities(globalRef,entities);
            return entities;
        }
        //
        public void Delete(object key)
        {
            FieldInfo[] keyFields = key.GetType().GetFields();
            ArrayList keys = new ArrayList();
            for (int i = 0; i < meta.KyesMeta.Count; i++)
            {
                meta.KeysValidator[i].ValidateKey(keyFields[i].GetValue(key));
                keys.Add(keyFields[i].GetValue(key));
            }
            globalRef.SetSubscripts(keys);
            globalRef.Kill();
        }
        public void Delete(ProxyKeyT key)
        {
            globalRef.SetSubscripts(ProxyKeysSerializer.SerializeStructedKey(key));
            globalRef.Kill();
        }
        public void Delete(ProxyT key)
        {
            globalRef.SetSubscripts(newProxyKeySerializer.SerializeAsKeys(key));
            globalRef.Kill();
        }
        //

        //
        private ProxyT CreateEntity(ArrayList keysValues)
        {
            ProxyT entity = (ProxyT)defaultConstructor.Invoke(new object[] { });
            ProxySerializer.Deserialize(entity, keysValues);
            return entity;
        }
        private ProxyT CreateEntity(ArrayList keysValues, ArrayList valuesValues)
        {
            ProxyT entity = (ProxyT)defaultConstructor.Invoke(new object[] { });
            ProxySerializer.DeserializeStructedKeys(entity, new Queue(keysValues));
            ProxySerializer.DeserializeValues(entity, valuesValues);
            return entity;
        }
        //
        private void treeWalkForEntities(TrueNodeReference glNode, List<ProxyT> entities)
        {
            glNode.AppendSubscript("");
            while (glNode.NextSubscript() != "")
            {
                glNode.GoNextSubscript();
                if (glNode.SubsCount == this.keysMetaAsVectorList.Count && glNode.HasValues())
                {
                    entities.Add(
                        CreateEntity(glNode.GetSubscripts(), glNode.GetValues(meta.ValuesMeta))
                    );
                    continue;
                }
                if (glNode.HasSubnodes() && glNode.SubsCount < this.keysMetaAsVectorList.Count)
                {
                    treeWalkForEntities(glNode, entities);
                    glNode.GoParentNodeSubscripts();
                }
            }
        }
        private void treeWalkForEntities(TrueNodeReference glNode, ArrayList baseSubscripts, List<ProxyT> entities)
        {
            glNode.AppendSubscript("");
            if (baseSubscripts[glNode.SubsCount - 1] != null)
            {
                if (!baseSubscripts[glNode.SubsCount - 1].ToString().Equals(String.Empty))
                {
                    glNode.SetSubscript(glNode.SubsCount, baseSubscripts[glNode.SubsCount - 1]);
                    if (glNode.SubsCount == baseSubscripts.Count)
                    {
                        if (glNode.HasValues())
                        {
                            entities.Add(CreateEntity(glNode.GetSubscripts(), glNode.GetValues(meta.ValuesMeta)));
                        }
                        return;
                    }
                    treeWalkForEntities(glNode, baseSubscripts, entities);
                    glNode.GoParentNodeSubscripts();
                    return;
                }
            }
            while (glNode.NextSubscript() != "")
            {
                glNode.GoNextSubscript();
                if (glNode.SubsCount == baseSubscripts.Count && glNode.HasValues())
                {
                    entities.Add(CreateEntity(glNode.GetSubscripts(), glNode.GetValues(meta.ValuesMeta)));
                    continue;
                }
                if (glNode.SubsCount < baseSubscripts.Count && glNode.HasSubnodes())
                {
                    treeWalkForEntities(glNode, baseSubscripts, entities);
                    glNode.GoParentNodeSubscripts();
                }
            }
        }
        private void treeWalkForKeys(TrueNodeReference glNode, ArrayList baseSubscripts, ArrayList subscriptsList)
        {
            glNode.AppendSubscript("");
            if (baseSubscripts[glNode.SubsCount - 1] != null)
            {
                if (!baseSubscripts[glNode.SubsCount - 1].ToString().Equals(String.Empty))
                {
                    glNode.SetSubscript(glNode.SubsCount, baseSubscripts[glNode.SubsCount - 1]);
                    if (glNode.SubsCount == baseSubscripts.Count)
                    {
                        if (glNode.HasValues())
                        {
                            subscriptsList.Add(glNode.GetSubscripts());
                        }
                        return;
                    }
                    treeWalkForKeys(glNode, baseSubscripts, subscriptsList);
                    glNode.GoParentNodeSubscripts();
                    return;
                }
            }
            while (glNode.NextSubscript() != "")
            {
                glNode.GoNextSubscript();
                if (glNode.SubsCount == baseSubscripts.Count && glNode.HasValues())
                {
                    subscriptsList.Add(glNode.GetSubscripts());
                    continue;
                }
                if (glNode.SubsCount < baseSubscripts.Count && glNode.HasSubnodes())
                {
                    treeWalkForKeys(glNode, baseSubscripts, subscriptsList);
                    glNode.GoParentNodeSubscripts();
                }
            }
        }
    }

    public interface IStructManager
    {
        Type GetStructType();
        CacheProxySerializer GetSerializer();
        object CreateStructEntity();
        object CreateStructEntity(IList values);
        ArrayList Serialize(object structEntity);
        //
        //for new serializer
        ArrayList SerializeAsKeys(object entity);
        ValueList SerializeAsComplexVList(object entity, Connection conn);
        object CreateEntity(IList values);
        object CreateEntityFromVector(Queue values);

    }

    public class StructManager<StructT> : IStructManager where StructT : class
    {
        public bool Validate = true;
        //
        private StructValMeta meta;
        private Type structType;
        public Type StructType { get { return structType; } }
        //
        private ConstructorInfo fullConstructor;
        private ConstructorInfo defaultContructor;
        private object[] defaultConstructorParameter;
        //
        private FieldInfo[] valuesFields;
        //
        private List<IStructManager> structsManagers;
        //
        private CacheProxySerializer structSerializer;
        private CacheProxySerializer<StructT> newStructSerializer;
        //
        //
        public StructManager(StructValMeta structMeta,List<IStructManager> structsManagers, Connection conn = null)
        {
            structType = typeof(StructT);
            this.structsManagers = structsManagers;
            this.meta = structMeta;
            defaultContructor = structType.GetConstructors()[0];
            defaultConstructorParameter = new object[] { };
            fullConstructor = structType.GetConstructors()[1];
            initValuesFieldsInfo();
            //scary changes
            /*structSerializer = new CacheProxySerializer(valuesFields
                , new EntityMeta { KyesMeta = new List<ValueMeta>(), ValuesMeta = meta.elementsMeta }
                , structsManagers);*/
            structSerializer = new CacheProxySerializer(valuesFields, valuesFields
                , meta.elementsMeta, meta.elementsMeta 
                , structsManagers);
            //
            newStructSerializer = new CacheProxySerializer<StructT>(meta.elementsMeta,structsManagers);
        }
        private void initValuesFieldsInfo()
        {
            valuesFields = new FieldInfo[meta.elementsMeta.Count];
            for (int i = 0; i < meta.elementsMeta.Count; i++)
            {
                valuesFields[i] = structType.GetField(meta.elementsMeta[i].SemanticName);
            }
        }
        //
        public object CreateStructEntity()
        {
            return (StructT)defaultContructor.Invoke(defaultConstructorParameter);
        }

        public object CreateStructEntity(IList values)
        {
            StructT obj = (StructT)defaultContructor.Invoke(defaultConstructorParameter);
            structSerializer.DeserializeValues(obj, values);
            return obj;
        }
        public object CreateKeyStructEntity(Queue values)
        {
            StructT obj = (StructT)defaultContructor.Invoke(defaultConstructorParameter);
            structSerializer.DeserializeStructedKeys(obj, values);
            return obj;
        }
        public ArrayList Serialize(object structEntity)
        {
            return structSerializer.SerializeValues(structEntity);
        }
        //
        public Type GetStructType()
        {
            return StructType;
        }
        public CacheProxySerializer GetSerializer()
        {
            return structSerializer;
        }
        //
        //IStructManager.
        public ArrayList SerializeAsKeys(object entity)
        {
            return newStructSerializer.SerializeAsKeys(entity);
        }
        public ValueList SerializeAsComplexVList(object entity, Connection conn)
        {
            return newStructSerializer.Serialize(entity,conn);
        }
        public object CreateEntity(IList values)
        {
            return newStructSerializer.Deserialize(values);
        }
        public object CreateEntityFromVector(Queue values)
        {
            return newStructSerializer.DeserializeAsVectorPresented(values);
        }
    }
}
