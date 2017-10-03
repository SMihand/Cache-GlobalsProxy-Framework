using InterSystems.Globals;
using System;
using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace CacheEXTREME2.WProxyGlobal
{
    public class CacheEXTREMEcontext
    {
        public Connection conn;
        protected string globalName = "";
        protected string globalMetaName = "";
        //
        protected GlobalMeta globalMeta;
        protected Dictionary<string, EntityMeta> entitiesMeta;
        protected List<IStructManager> structsManagers = new List<IStructManager>();
        //
        protected TrueNodeReference globalRef;
        //
        public CacheEXTREMEcontext(Connection conn, string globalName, string globalMetaName, IMetaReader metaReader)
        {
            this.globalName = globalName;
            this.globalMetaName = globalMetaName;
            this.conn = conn;
            this.globalMeta = metaReader.GetMeta(this.globalMetaName);
            this.globalName = this.globalMeta.GlobalName;
            this.globalRef = new TrueNodeReference(conn, globalName);
            this.getEntities();
        }

        public CacheEXTREMEcontext(Connection conn, string globalName, string globalMetaName)
            : this(conn, globalName, globalMetaName, new MetaReaderWriter(conn))
        {
        }

        public CacheEXTREMEcontext(Connection conn, string globalMetaName)
            :this(conn,"",globalMetaName)
        {
        }

        public CacheEXTREMEcontext(Connection conn, GlobalMeta globalMeta)
        {
            this.globalName = globalMeta.GlobalName;
            this.globalMetaName = globalMeta.GlobalMetaName;
            this.conn = conn;
            this.globalMeta = globalMeta;
            this.globalRef = new TrueNodeReference(conn, globalName);
            this.getEntities();
        }
        private void getEntities()
        {
            entitiesMeta = new Dictionary<string, EntityMeta>();
            for (int i = 1; i <= globalMeta.KeysCount; i++)
            {
                if (globalMeta.GetEntityMeta(i).ValuesMeta.Count > 0)
                {
                    string entityName = globalMeta.GetNodeMeta(i - 1).Key + "Proxy";
                    entitiesMeta.Add(entityName, globalMeta.GetEntityMeta(i));
                }
            }
        }
        //
        public bool HasEntity(object entity)
        {
            if (entitiesMeta.ContainsKey(entity.GetType().Name))
            {
                return true;
            }
            return false;
        }
        public bool HasEntity(Type entityType)
        {
            if (entitiesMeta.ContainsKey(entityType.Name))
            {
                return true;
            }
            return false;
        }
        public TrueNodeReference GetNodeReference()
        {
            return new TrueNodeReference(conn, globalName);
        }
        //
        public object GetEntity(Type entityType, object key)
        {
            object entity = null;
            string entityName = entityType.Name;
            if (HasEntity(entityType))
            {
                FieldInfo[] keyFields = key.GetType().GetFields();
                ArrayList keys = new ArrayList();
                for (int i = 0; i < entitiesMeta[entityName].KyesMeta.Count; i++)
                {
                    entitiesMeta[entityName].KeysValidator[i].ValidateKey(keyFields[i].GetValue(key));
                    keys.Add(keyFields[i].GetValue(key));
                }
                List<ValueMeta> valuesMeta = entitiesMeta[entityName].ValuesMeta;
                List<ValueMeta> keysMeta = entitiesMeta[entityName].KyesMeta;
                globalRef.SetSubscripts(keys);
                if (globalRef.HasValues())
                {
                    ArrayList values = globalRef.GetValues(valuesMeta);
                    //
                    keys.AddRange(values);
                    entity = CreateEntity(entityType, keys.ToArray());
                }
            }
            return entity;
        }
        public object GetEntity(Type entityType, ArrayList keys)
        {
            object entity = null;
            string entityName = entityType.Name;
            if (HasEntity(entityType))
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    this.globalMeta[i].Validate(keys[i]);
                }
                List<ValueMeta> valuesMeta = entitiesMeta[entityName].ValuesMeta;
                List<ValueMeta> keysMeta = entitiesMeta[entityName].KyesMeta;
                globalRef.SetSubscripts(keys);
                if (globalRef.HasValues())
                {
                    ArrayList values = globalRef.GetValues(valuesMeta);
                    keys.AddRange(values);
                    //
                    entity = CreateEntity(entityType, keys.ToArray());
                }
            }
            return entity;
        }
        public ArrayList TrueGetEntities(Type entityType, object key)
        {
            ArrayList entities = new ArrayList();
            if (HasEntity(entityType))
            {
                int keyCount = entitiesMeta[entityType.Name].KyesMeta.Count;
                FieldInfo[] keyFields = key.GetType().GetFields();
                ArrayList keys = new ArrayList();
                for (int i = 0; i < keyCount; i++)
                {
                    //entitiesMeta[entityType.Name].keysValidator[i].ValidateKey( keyFields[i].GetValue(key));
                    keys.Add(keyFields[i].GetValue(key));
                }
                keyFields = null;
                globalRef.Reset();
                ArrayList subsList = new ArrayList();
                treeWalkForKeys(globalRef, keys, subsList);
                ArrayList tempVals;
                foreach (ArrayList nkey in subsList)
                {
                    globalRef.SetSubscripts(nkey);
                    tempVals = globalRef.GetValues(entitiesMeta[entityType.Name].ValuesMeta);
                    nkey.AddRange(tempVals);
                    entities.Add(CreateEntity(entityType, nkey.ToArray()));
                }
                subsList = null;
            }
            return entities;
        }
        public void SaveEntity(object entity)
        {
            if(HasEntity(entity))
            {
                Type entityType = entity.GetType();
                FieldInfo[] fields = entityType.GetFields();
                ArrayList keys = new ArrayList();
                ArrayList values = new ArrayList();
                int keysCount = entitiesMeta[entityType.Name].KyesMeta.Count;
                for (int i = 0; i < keysCount; i++ )
                {
                    keys.Add(fields[i].GetValue(entity));
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    this.globalMeta[i].Validate(keys[i]);
                }
                for (int i = keysCount, j = 0; j < globalMeta.GetNodeMeta(keysCount).Value.Count; i++, j++)
                {
                    values.Add(fields[i].GetValue(entity));
                }
                for (int j = 0; j < values.Count; j++)
                {
                    this.globalMeta[keys.Count - 1, j].Validate(values[j]);
                }
                globalRef.SetValues(keys, values);
            }
        }
        public void KillEntity(Type entityType, object key)
        {
            if(HasEntity(entityType))
            {
                FieldInfo[] keyFields = key.GetType().GetFields();
                ArrayList keys = new ArrayList();
                for (int i = 0; i < entitiesMeta[entityType.Name].KyesMeta.Count; i++)
                {
                    entitiesMeta[entityType.Name].KeysValidator[i].ValidateKey(keyFields[i].GetValue(key));
                    keys.Add(keyFields[i].GetValue(key));
                }
                globalRef.SetSubscripts(keys);
                globalRef.Kill();
            }
        }
        public ArrayList TrueGetKeys(ArrayList baseSubscripts)
        {
            ArrayList subs = new ArrayList();
            treeWalkForKeys(globalRef, baseSubscripts, subs);
            return subs;
        }
        public void ReloadGlobalMeta(GlobalMeta newMeta)
        {
            try
            {
                globalMeta.ResetRestrictionsOnly(newMeta);
            }
            catch(Exception ex)
            {
                throw new ArgumentException("newMeta has diferent structure or/and semantics. " + ex.Message, "newMeta", ex);
            }
        }
        //
        private object CreateEntity(Type entityType, object[] args)
        {
            object entity = null;
            if(HasEntity(entityType))
            {
                ConstructorInfo[] constructor = entityType.GetConstructors();
                entity = constructor[0].Invoke(args);
            }
            return entity;
        }
        //
        private void treeWalkForKeys(TrueNodeReference glNode, ArrayList baseSubscripts, ArrayList subscriptsList)
        {
            glNode.AppendSubscript("");
            if (baseSubscripts[glNode.SubsCount - 1] != null && baseSubscripts[glNode.SubsCount - 1].ToString() != "")
            {
                glNode.SetSubscript(glNode.SubsCount, baseSubscripts[glNode.SubsCount - 1]);
                if (glNode.SubsCount == baseSubscripts.Count)
                {
                    if (glNode.HasValues())
                    {
                        subscriptsList.Add(glNode.GetSubscripts());
                        return;
                    }
                    return;
                }
                treeWalkForKeys(glNode, baseSubscripts, subscriptsList);
                glNode.GoParentNodeSubscripts();
                return;
            }
            while (glNode.NextSubscript() != "")
            {
                glNode.GoNextSubscript();
                if (glNode.SubsCount == baseSubscripts.Count)
                {
                    subscriptsList.Add(glNode.GetSubscripts());
                    continue;
                }
                if (glNode.HasSubnodes())
                {
                    treeWalkForKeys(glNode, baseSubscripts, subscriptsList);
                    glNode.GoParentNodeSubscripts();
                }
            }
        }
    }
}
