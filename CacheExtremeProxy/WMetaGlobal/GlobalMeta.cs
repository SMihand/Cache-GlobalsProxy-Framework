using System;
using System.Collections;
using System.Collections.Generic;

namespace CacheEXTREME2.WMetaGlobal
{

    public class GlobalMeta
    {
        private string globalMetaName;
        public string GlobalMetaName { get { return globalMetaName; } }

        private string globalName;
        public string GlobalName { get { return globalName; } }

        private List<IKeyValidator> keys;
        public int KeysCount
        {
            get { return keys.Count; }
        }

        private List<KeyValuePair<string, List<ValueMeta>>> nodesMeta;
        public KeyValuePair<string, List<ValueMeta>> this[int subsCount] { get { return GetEntityValuesMeta(subsCount); } }

        private Dictionary<string,StructValMeta> localStructs;
        //
        public GlobalMeta(string globalMetaName, List<IKeyValidator> keysMeta, List<KeyValuePair<string, List<ValueMeta>>> nodesMeta)
        {
            this.globalMetaName = globalMetaName;
            this.keys = new List<IKeyValidator>(keysMeta);
            this.nodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>(nodesMeta);
            this.localStructs = new Dictionary<string, StructValMeta>();
        }
        public GlobalMeta(string globalMetaName, string globalName, List<IKeyValidator> keysMeta, List<KeyValuePair<string, List<ValueMeta>>> nodesMeta)
            : this(globalMetaName, keysMeta, nodesMeta)
        {
            this.globalName = globalName;
        }
        public GlobalMeta(string dataGlobalName, string metaGlobalName)
        {
            this.globalName = dataGlobalName;
            this.globalMetaName = metaGlobalName;
            this.keys = new List<IKeyValidator>();
            this.nodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>();
            this.localStructs = new Dictionary<string, StructValMeta>();
        }
        //
        public void ResetRestrictionsOnly(GlobalMeta resetFrom)
        {
            string curentSemantic="";
            string sampleSemantic=""; 
            try
            {
                foreach(string key in resetFrom.localStructs.Keys)
                {
                    sampleSemantic = key;
                    curentSemantic = "sample structure is not found";
                    try
                    {
                        localStructs[key].SetRestrictionsMeta(resetFrom.localStructs[key]);
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic +"\n"+ex.Message);
                    }
                }
                for (int i = 0; i < resetFrom.keys.Count; i++)
                {
                    curentSemantic = "sample semantic is not found";
                    sampleSemantic = ((ValueMeta)resetFrom.keys[i]).SemanticName;
                    try
                    {
                        curentSemantic = ((ValueMeta)keys[i]).SemanticName;
                        if (!sampleSemantic.Equals(curentSemantic))
                            throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n");
                    }
                    catch { throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n"); }
                    (keys[i] as ValueMeta).SetRestrictionsMeta(resetFrom.keys[i] as ValueMeta);
                }
                for (int i = 0; i < resetFrom.nodesMeta.Count; i++)
                {
                    curentSemantic = "sample semantic is not found";
                    sampleSemantic = resetFrom.nodesMeta[i].Key;
                    try
                    {
                        curentSemantic = this.nodesMeta[i].Key;
                        if (!sampleSemantic.Equals(curentSemantic))
                            throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n");
                    }
                    catch { throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n"); }
                    for (int j = 0; j < resetFrom.nodesMeta[i].Value.Count; j++)
                    {
                        curentSemantic = "sample semantic is not found";
                        sampleSemantic = resetFrom.nodesMeta[i].Value[j].SemanticName;
                        try
                        {
                            curentSemantic = nodesMeta[i].Value[j].SemanticName;
                            if (!sampleSemantic.Equals(curentSemantic))
                                throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n");
                        }
                        catch { throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n"); }
                        nodesMeta[i].Value[j].SetRestrictionsMeta(resetFrom.nodesMeta[i].Value[j]);
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw new ArgumentException("mismatch between curent structure or/and semantic of global and sample\n" + ex.Message, "resetFrom", ex);
            }
        }
        //
        //Editing
        //  keys works:
        public void AddKeyMeta(IKeyValidator key, string className = "null")
        {
            if (!this.keys.Exists(e => (key as ValueMeta).SemanticName == (e as ValueMeta).SemanticName))
            {
                //костыль
                if (((ValueMeta)key).ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    if (this.localStructs.ContainsKey((key as StructValMeta).StructTypeName))
                    {
                        this.keys.Add(new StructValMeta((key as ValueMeta).SemanticName, localStructs[(key as StructValMeta).StructTypeName]));
                    }
                    else
                    {
                        throw new ArgumentException("Struct type not found: '" + (key as StructValMeta).StructTypeName + "'!");
                    }
                }
                else
                {
                    this.keys.Add(key);
                }
                nodesMeta.Add(new KeyValuePair<string, List<ValueMeta>>(className, new List<ValueMeta>()));
            }
        }

        public void ResetKeyMeta(ValueMeta oldKey, ValueMeta newKey, string className)
        {
            List<ValueMeta> keysMeta = this.GetKeysMeta();
            int oldIdx = keysMeta.FindIndex(e => e.SemanticName == oldKey.SemanticName);
            keys[oldIdx] = (IKeyValidator)newKey;
            KeyValuePair<string, List<ValueMeta>> newNode 
                = new KeyValuePair<string,List<ValueMeta>>(className,nodesMeta[0].Value);
            nodesMeta[oldIdx] = newNode;
        }

        public void RemoveKey(int keyIndex)
        {
            if (this.keys.Count > 0)
            {
                this.keys.RemoveAt(keyIndex);
                this.nodesMeta.RemoveAt(keyIndex);
            }
        }
        
        //  structs works
        public void AddStruct(string structName, StructValMeta structMeta)
        {
            if (!localStructs.ContainsKey(structName))
            {
                structMeta.StructId = localStructs.Count;//Trying to normilize structs Identifiers
                //
                for (int i = 0; i < structMeta.elementsMeta.Count; i++)
                {
                    if (structMeta.elementsMeta[i].ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                    {
                        if (localStructs.ContainsKey((structMeta.elementsMeta[i] as StructValMeta).StructTypeName))
                        {
                            (structMeta.elementsMeta[i] as StructValMeta).elementsMeta = localStructs[(structMeta.elementsMeta[i] as StructValMeta).StructTypeName].elementsMeta;
                            continue;
                        }
                        throw new ArgumentException("Struct type not found: '" + (structMeta.elementsMeta[i] as StructValMeta).StructTypeName + "'!");
                    }
                }
                //
                localStructs.Add(structName, structMeta);
            }
        }
        public void AddStruct(StructValMeta structMeta)
        {
            AddStruct(structMeta.StructTypeName, structMeta);
        }

        /// <summary>
        /// NOT TESTED!
        /// i dont know how will result corelate with all globalMeta work
        /// </summary>
        /// <param name="oldTypeName"></param>
        /// <param name="newTypeName"></param>
        public void RenameStruct(string oldTypeName, string newTypeName)
        {
            StructValMeta old = localStructs[oldTypeName];
            localStructs.Remove(old.StructTypeName);
            old.StructTypeName = newTypeName;
            localStructs.Add(newTypeName, old);
        }

        public void EditStruct(string structTypeName, StructValMeta structMeta)
        {
            localStructs[structTypeName] = new StructValMeta(structTypeName, structMeta);
        }

        /// <summary>
        /// NEED TO REWRITE THIS SHIT!
        /// need to recoursivily delete all struct invasions!
        /// </summary>
        /// <param name="structTypeName"></param>
        public void RemoveStruct(string structTypeName)
        {
            localStructs.Remove(structTypeName);
            while (keys.Exists(e => ((ValueMeta)e).GetCSharpTypeName() == structTypeName))
            {
                int ki = keys.FindIndex(e => ((ValueMeta)e).GetCSharpTypeName() == structTypeName);
                keys.RemoveAt(ki);
                nodesMeta.RemoveAt(ki);
            }
            for (int i = 0; i < nodesMeta.Count; i++)
            {
                nodesMeta[i].Value.RemoveAll(e => ((ValueMeta)e).GetCSharpTypeName() == structTypeName);
            }
        }

        //  nodes works
        public void SetValuesMeta(int forIndex, List<ValueMeta> valuesMeta)
        {
            if (forIndex <= KeysCount)
            {
                nodesMeta[forIndex - 1].Value.Clear();
                nodesMeta[forIndex - 1].Value.AddRange(valuesMeta);
            }
        }
        public void SetValuesMeta(int forIndex, List<ValueMeta> valuesMeta, string nodeSemantic)
        {
            if (forIndex <= KeysCount)
            {
                nodesMeta[forIndex - 1] = new KeyValuePair<string, List<ValueMeta>>(nodeSemantic, valuesMeta);
            }
        }
        //
        //Getters
        public List<IKeyValidator> GetKeysValidator(int keysCount = 0) 
        { 
            return (keysCount==0)?keys:keys.GetRange(0,keysCount); 
        }

        public List<ValueMeta> GetKeysMeta(int keysCount = 0)
        {
            List<ValueMeta> keysMeta = new List<ValueMeta>();
            for (int i = 0; i < keys.Count; i++)
            {
                switch(((ValueMeta)keys[i]).ExtremeType){
                    case ExtremeTypes.EXTREME_DOUBLE:
                    {
                        keysMeta.Add((DoubleValMeta)keys[i]);
                        break;
                    }
                        case ExtremeTypes.EXTREME_INT:
                    {
                        keysMeta.Add((IntValMeta)keys[i]);
                        break;
                    }
                        case ExtremeTypes.EXTREME_STRING:
                    {
                        keysMeta.Add((StringValMeta)keys[i]);
                        break;
                    }
                        case ExtremeTypes.EXTREME_STRUCT:
                    {
                        keysMeta.Add(new StructValMeta(((ValueMeta)keys[i]).SemanticName,localStructs[((StructValMeta)keys[i]).StructTypeName]));
                        break;
                    }
                }
            }
            return (keysCount==0)?keysMeta:keysMeta.GetRange(0,keysCount);
        }

        public KeyValuePair<string, List<ValueMeta>> GetEntityValuesMeta(int indexCount)
        {
            KeyValuePair<string, List<ValueMeta>> entityValuesMeta =  nodesMeta[indexCount - 1];
            for (int i = 0; i < entityValuesMeta.Value.Count; i++)
            {
                completeStructMeta(entityValuesMeta.Value[i]);
            }
            return entityValuesMeta;
        }

        public List<StructValMeta> GetLocalStructs()
        {
            List<StructValMeta> structsMeta = new List<StructValMeta>(localStructs.Count);
            foreach (var structMeta in localStructs)
            {
                structsMeta.Add(structMeta.Value);
            }
            return structsMeta;
        }

        public StructValMeta GetStruct(string structTypeName)
        {
            return localStructs[structTypeName];
        }

        //
        private void completeStructMeta(ValueMeta valueMeta)
        {
            switch(valueMeta.ExtremeType){
                case ExtremeTypes.EXTREME_STRUCT:
                {
                    StructValMeta structMeta = (valueMeta as StructValMeta);
                    structMeta.elementsMeta = new List<ValueMeta>(localStructs[structMeta.StructTypeName].elementsMeta);
                    for (int i = 0; i < structMeta.elementsMeta.Count; i++)
                    {
                        //trying to normilize structs id
                        structMeta.StructId = localStructs[structMeta.StructTypeName].StructId;
                        //
                        completeStructMeta(structMeta.elementsMeta[i]);
                    }
                    return;
                }
                case ExtremeTypes.EXTREME_LIST:
                {
                    ListValMeta listMeta = (valueMeta as ListValMeta);
                    completeStructMeta(listMeta.ElemMeta);
                    break;
                }
            }
        }

        //validators
        public bool ValidateKeys(ArrayList subscripts)
        {
            if (subscripts.Count > KeysCount)
            {
                throw new ArgumentOutOfRangeException("subscript.Count"
                    , subscripts.Count
                    , globalMetaName + " has " + KeysCount + " subscripts;");
            }
            for (int i = 0; i < subscripts.Count; i++)
			{
                if (!keys[i].ValidateKey(subscripts[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool ValidateValues(ArrayList subscripts, ArrayList values)
        {
            if (!ValidateKeys(subscripts))
            {
                return false;
            }
            List<ValueMeta> valMeta = nodesMeta[subscripts.Count-1].Value;
            for (int i = 0; i < values.Count; i++)
            {
                if (!valMeta[i].Validate(values[i]))
                {
                    return false;
                }
            }
            return true;
        }
        //
        //
        public EntityMeta GetEntityMeta(int indexCount)
        {
            EntityMeta meta;
            meta.EntityName = this[indexCount].Key;
            meta.KeysValidator = this.GetKeysValidator(indexCount);
            meta.NormilizedLocalStructs = new List<StructValMeta>(this.localStructs.Count);
            //
            for (int i = 0; i < localStructs.Count; i++) { meta.NormilizedLocalStructs.Add(new StructValMeta()); }
            //
            foreach (StructValMeta structMeta in localStructs.Values)
            {
                meta.NormilizedLocalStructs[structMeta.StructId] = structMeta;
            }
            meta.ValuesMeta = this.GetEntityValuesMeta(indexCount).Value;
            meta.KyesMeta = GetKeysMeta(indexCount);
            meta.NormilizedLocalStructs = null;
            return meta;
        }

    }


    public struct EntityMeta
    {
        public string EntityName;
        public List<IKeyValidator> KeysValidator;
        public List<ValueMeta> KyesMeta;
        public List<ValueMeta> ValuesMeta;
        public List<StructValMeta> NormilizedLocalStructs;
    }


    /*class GlobalMetaException : Exception
    {
    }
    class SubscriptValidationException : GlobalMetaException
    {
    }
    class ValueValidationException : GlobalMeta
    {
        public ValueValidationException():base("") { }
    }*/

}
