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
        //
        //
        private Type proxyT;
        private Type proxyKeyT;
        private ConstructorInfo fullConstructor;
        private ConstructorInfo defaultConstructor;
        private object[] defaultConstructorParam = new object[0];
        //
        private List<IStructManager> structsManagers;
        //
        private FieldInfo[] proxyKeyTkeysFields;
        private FieldInfo[] proxyTkeysFields;
        private FieldInfo[] proxyTvaluesFields;
        private MethodInfo customValidator;
        private ProxyT[] methodParam = new ProxyT[1];
        
        //
        //private CacheProxySerializer ProxySerializer;
        //private CacheProxySerializer ProxyKeysSerializer;

        private CacheProxySerializer newProxyTSerializer;
        private CacheProxySerializer newProxyTAsValuesSerializer;
        private CacheProxySerializer newProxyTasKeySerializer;

        private CacheProxySerializer newProxyKeyTSerializer;        


        //
        //
        public ProxyManager(EntityMeta meta, TrueNodeReference globalRef, List<IStructManager> structsManagers = null)
        {
            this.meta = meta;
            keysMetaAsVectorList = new List<ValueMeta>();
            vectorizeKey(keysMetaAsVectorList, meta.KyesMeta);
            this.globalRef = new TrueNodeReference(globalRef);
            proxyT = typeof(ProxyT);
            proxyKeyT = typeof(ProxyKeyT);
            fullConstructor = proxyT.GetConstructors()[0];//specification of generated file
            defaultConstructor = proxyT.GetConstructors()[1];//specification of generated file
            customValidator = proxyT.GetMethod(this.meta.EntityName + "Validator");//-||-   -||-   
            initKeysValuesFields();
            this.structsManagers = structsManagers;
            List<ValueMeta> keysValuesMeta = new List<ValueMeta>(meta.KyesMeta);
            /*this.ProxySerializer = new CacheProxySerializer(proxyTkeysFields, proxyTvaluesFields
                , meta.KyesMeta, meta.ValuesMeta
                , structsManagers);
            this.ProxyKeysSerializer = new CacheProxySerializer(proxyKeyTkeysFields, proxyKeyTkeysFields
                , meta.KyesMeta, meta.KyesMeta
                , structsManagers);*/

            //
            List<ValueMeta> proxyTFieldsMeta = new List<ValueMeta>(meta.KyesMeta);
            proxyTFieldsMeta.AddRange(meta.ValuesMeta);
            this.newProxyTSerializer = new CacheProxySerializer(typeof(ProxyT), meta.KyesMeta, meta.ValuesMeta, structsManagers, globalRef.Conn);
            this.newProxyTasKeySerializer = new CacheProxySerializer(typeof(ProxyT), meta.KyesMeta, new List<ValueMeta>(), structsManagers, globalRef.Conn);
            this.newProxyTAsValuesSerializer = new CacheProxySerializer(typeof(ProxyT), new List<ValueMeta>(), meta.ValuesMeta, structsManagers, globalRef.Conn);

            this.newProxyKeyTSerializer = new CacheProxySerializer(typeof(ProxyKeyT), meta.KyesMeta, new List<ValueMeta>(), structsManagers, globalRef.Conn);            
        }
        private void initKeysValuesFields()
        {
            proxyTkeysFields = new FieldInfo[meta.KyesMeta.Count];
            proxyKeyTkeysFields = new FieldInfo[meta.KyesMeta.Count];
            for (int i = 0; i < meta.KyesMeta.Count; i++)
            {
                proxyTkeysFields[i] = proxyT.GetField(meta.KyesMeta[i].SemanticName);
                proxyKeyTkeysFields[i] = proxyKeyT.GetField(meta.KyesMeta[i].SemanticName);
            }
            proxyTvaluesFields = new FieldInfo[meta.ValuesMeta.Count];
            for (int i = 0; i < meta.ValuesMeta.Count; i++)
            {
                proxyTvaluesFields[i] = proxyT.GetField(meta.ValuesMeta[i].SemanticName);
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
                        vectorizeKey(keysVector, (keyMeta as StructValMeta).structDefinition.elementsMeta);
                        break;
                }
            }
        }
        //
        public bool ValidateEntity(ProxyT entity)
        {
            for (int i = 0; i < proxyTkeysFields.Length; i++)
            {
                switch (this.meta.KyesMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_BYTES:
                        break;
                    case ExtremeTypes.EXTREME_DOUBLE:
                        break;
                }
            }
            return true;
        }
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
                    for (int i = 0; i < proxyTkeysFields.Length; i++)
                    {
                        meta.KeysValidator[i].ValidateKey(proxyTkeysFields[i].GetValue(entity));
                    }
                    for (int i = 0; i < proxyTvaluesFields.Length; i++)
                    {
                        meta.ValuesMeta[i].Validate(proxyTvaluesFields[i].GetValue(entity));
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
            
            //STABLE VERSION MORE FASTER:
            //globalRef.AppendSubscripsts(this.ProxySerializer.SerializeKeyWithStructsAsValues(entity), meta.KyesMeta);
            //globalRef.SetValuesTyped(this.ProxySerializer.SerializeValues(entity), this.meta.ValuesMeta);

            //STABLE BETA VERSION (perfomance winner)
            globalRef.AppendSubscripsts(newProxyTSerializer.SerializeKeysPart(entity), keysMetaAsVectorList);
            //globalRef.SetValues(newProxyTAsValuesSerializer.SerializeValues(entity, globalRef.Conn));
            globalRef.SetValues(newProxyTSerializer.SerializeValues(entity, globalRef.Conn));
            newProxyTSerializer.ClearValuesListHolders();
            newProxyTasKeySerializer.ClearValuesListHolders();
        }
        public void SaveFaster(ProxyT entity)
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
                    for (int i = 0; i < proxyTkeysFields.Length; i++)
                    {
                        switch(this.meta.KyesMeta[i].ExtremeType)
                        {
                            case ExtremeTypes.EXTREME_BYTES:
                                (meta.KyesMeta[i] as BytesValMeta).ValidateBytes((byte[])proxyTkeysFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_DOUBLE:
                                (meta.KyesMeta[i] as DoubleValMeta).ValidateDouble((double)proxyTkeysFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_INT:
                                (meta.KyesMeta[i] as IntValMeta).ValidateInt((int)proxyTkeysFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_LIST:
                                (meta.KyesMeta[i] as ListValMeta).ValidateILIst((IList)proxyTkeysFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_STRING:
                                (meta.KyesMeta[i] as StringValMeta).ValidateString((string)proxyTkeysFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_STRUCT:
                                (meta.KyesMeta[i] as StructValMeta).ValidateStruct(proxyTkeysFields[i].GetValue(entity));
                                continue;
                        }
                    }
                    for (int i = 0; i < proxyTvaluesFields.Length; i++)
                    {
                        switch (this.meta.ValuesMeta[i].ExtremeType)
                        {
                            case ExtremeTypes.EXTREME_BYTES:
                                (meta.ValuesMeta[i] as BytesValMeta).ValidateBytes((byte[])proxyTvaluesFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_DOUBLE:
                                (meta.ValuesMeta[i] as DoubleValMeta).ValidateDouble((double)proxyTvaluesFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_INT:
                                (meta.ValuesMeta[i] as IntValMeta).ValidateInt((int)proxyTvaluesFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_LIST:
                                (meta.ValuesMeta[i] as ListValMeta).ValidateILIst((IList)proxyTvaluesFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_STRING:
                                (meta.ValuesMeta[i] as StringValMeta).ValidateString((string)proxyTvaluesFields[i].GetValue(entity));
                                continue;
                            case ExtremeTypes.EXTREME_STRUCT:
                                (meta.ValuesMeta[i] as StructValMeta).ValidateStruct(proxyTvaluesFields[i].GetValue(entity));
                                continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("In " + this.meta.EntityName + ":\n" + ex.Message, ex);
                }
            }
            globalRef.Reset();
            globalRef.AppendSubscripsts(newProxyTSerializer.SerializeKeysPart(entity), keysMetaAsVectorList);
            ValueList values = newProxyTSerializer.SerializeValues(entity, globalRef.Conn);
            globalRef.SetValues(values);
            newProxyTSerializer.ClearValuesListHolders();
        }

        //
        public ProxyT Get(object key)
        {
            ProxyT entity = null;
            FieldInfo[] keyFields = key.GetType().GetFields();
            //vvvvvvvvvvvvvv VERY SLOW SOLUTION!!!! vvvvvvvvvvvvvvv//
            ArrayList kkkKeys = new CacheProxySerializer(key.GetType(), meta.KyesMeta, meta.ValuesMeta
                , structsManagers, globalRef.Conn)
                .SerializeKeysPart(key);
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
            ArrayList serializedKey = newProxyKeyTSerializer.SerializeKeysPart(key);
            globalRef.SetSubscripts(serializedKey);
            if (globalRef.HasValues())
            {
                ArrayList values = globalRef.GetValues(meta.ValuesMeta);
                //
                entity = CreateEntity(serializedKey, values);
            }
            return entity;
        }
        public ProxyT Get(ProxyT key)
        {
            ProxyT entity = null;
            ArrayList kkkKeys = newProxyTasKeySerializer.SerializeKeysPart(key);
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
            ArrayList serializedKey = newProxyKeyTSerializer.SerializeKeysPart(key);
            globalRef.Reset();
            treeWalkForEntities(globalRef, serializedKey, entities);
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
            //globalRef.SetSubscripts(ProxyKeysSerializer.SerializeStructedKey(key));
            globalRef.SetSubscripts(newProxyKeyTSerializer.SerializeKeysPart(key));
            globalRef.Kill();
        }
        public void Delete(ProxyT key)
        {
            globalRef.SetSubscripts(newProxyTasKeySerializer.SerializeKeysPart(key));
            globalRef.Kill();
        }
        //

        //
        private ProxyT CreateEntity(ArrayList keysValues)
        {
            
            return newProxyTSerializer.Deserialize(keysValues) as ProxyT;
            /*ProxyT entity = (ProxyT)defaultConstructor.Invoke(new object[] { });
            ProxySerializer.Deserialize(entity, keysValues);
            return entity;*/
        }
        private ProxyT CreateEntity(ArrayList keysValues, ArrayList valuesValues)
        {
            
            ProxyT entity = (ProxyT)defaultConstructor.Invoke(defaultConstructorParam);
            Int32 i = 0;
            newProxyTSerializer.DeserializeKeys(entity, ref keysValues, ref i);
            newProxyTSerializer.DeserializeValues(entity, valuesValues);
            return entity;
            /*keysValues.AddRange(valuesValues);
            return newProxyTSerializer.Deserialize(keysValues) as ProxyT;*/
            /*ProxyT entity = (ProxyT)defaultConstructor.Invoke(new object[] { });
            ProxySerializer.DeserializeStructedKeys(entity, new Queue(keysValues));
            ProxySerializer.DeserializeValues(entity, valuesValues);
            return entity;*/
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
        CacheProxySerializer GetNewSerializer();

        object CreateStructEntity();
        object CreateStructEntity(IList values);
        ArrayList Serialize(object structEntity);
        //
        //for new serializer
        ArrayList SerializeAsKeys(object entity);
        ValueList SerializeAsComplexVList(object entity, Connection conn);
        object CreateEntity(IList values);

    }

    public class StructManager<StructT> : IStructManager where StructT : class
    {
        public bool Validate = true;
        //
        public int StructId;
        private StructDefinition meta;
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
        private Connection conn;
        //
        //private CacheProxySerializer structSerializer;
        private CacheProxySerializer newStructSerializer;
        //
        //
        public StructManager(StructDefinition structMeta,List<IStructManager> structsManagers, Connection conn = null)
        {
            this.structType = typeof(StructT);
            this.conn = conn;
            this.structsManagers = structsManagers;
            this.meta = structMeta;
            this.meta.structObjectType = typeof(StructT);
            this.meta.structFields = typeof(StructT).GetFields();
            defaultContructor = structType.GetConstructors()[0];
            defaultConstructorParameter = new object[] { };
            fullConstructor = structType.GetConstructors()[1];
            initValuesFieldsInfo();
            //scary changes
            /*structSerializer = new CacheProxySerializer(valuesFields
                , new EntityMeta { KyesMeta = new List<ValueMeta>(), ValuesMeta = meta.structDefinition.elementsMeta }
                , structsManagers);*/
            /*structSerializer = new CacheProxySerializer(valuesFields, valuesFields
                , meta.elementsMeta, meta.elementsMeta 
                , structsManagers);*/
            //
            newStructSerializer = new CacheProxySerializer(typeof(StructT), meta.elementsMeta, meta.elementsMeta, structsManagers, conn);
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
            newStructSerializer.DeserializeValues(obj, values);
            return obj;
        }
        public object CreateKeyStructEntity(Queue values)
        {
            StructT obj = (StructT)defaultContructor.Invoke(defaultConstructorParameter);
            //newStructSerializer.DeserializeKeys(obj, values.ToArray());
            return obj;
        }
        public ArrayList Serialize(object structEntity)
        {
            throw new NotImplementedException();
            //return newStructSerializer.SerializeValues(structEntity, conn);
        }
        //
        public Type GetStructType()
        {
            return StructType;
        }
        public CacheProxySerializer GetNewSerializer()
        {
            return newStructSerializer;
        }
        //
        //IStructManager.
        public ArrayList SerializeAsKeys(object entity)
        {
            return newStructSerializer.SerializeKeysPart(entity);
        }
        public ValueList SerializeAsComplexVList(object entity, Connection conn)
        {
            return newStructSerializer.SerializeValues(entity,conn);
        }
        public object CreateEntity(IList values)
        {
            return newStructSerializer.Deserialize(values);
        }
    }
}