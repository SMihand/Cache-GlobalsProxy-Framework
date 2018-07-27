using System;
using System.Collections;
using System.Collections.Generic;

namespace CacheEXTREME2.WMetaGlobal
{
    public class GlobalMeta
    {
        public string GlobalMetaName;
        public string GlobalName;
        public string GlobalSemantic;

        //<string (is semantic of key), IKeyValidator (is )
        private List<IKeyValidator> keys;
        public int KeysCount
        {
            get { return keys.Count; }
        }

        private List<KeyValuePair<string, List<ValueMeta>>> nodesMeta;
        public int NodesWithMetaCount { get { return nodesMeta.Count; } }

        //real sequence of meta in global
        //INDEX corresponds to subs count to acces its nodes
        //KEY corresponds to sematic of the holded entity
        //VALUE is the actualy node meta 
        public ValueMeta this[int subscriptIndexToGet] {
            get { return (ValueMeta)this.keys[subscriptIndexToGet]; } 
            set { } 
        }
        public ValueMeta this[int nodeIndexToGet, int valueIndexToGet] { 
            get { return nodesMeta[nodeIndexToGet].Value[valueIndexToGet]; }
            set { }
        }

        private Dictionary<string, StructDefinition> localStructs;
        private Queue<string> structsSequence = new Queue<string>();
        public Queue<string> StructSequence { get { return new Queue<string>(structsSequence); } }
        //
        public GlobalMeta(string globalMetaName, List<IKeyValidator> keysMeta, List<KeyValuePair<string, List<ValueMeta>>> nodesMeta)
        {
            this.GlobalMetaName = globalMetaName;
            this.keys = new List<IKeyValidator>(keysMeta);
            this.nodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>(nodesMeta);
            this.localStructs = new Dictionary<string, StructDefinition>();
        }
        public GlobalMeta(string globalMetaName, string globalName, List<IKeyValidator> keysMeta, List<KeyValuePair<string, List<ValueMeta>>> nodesMeta)
            : this(globalMetaName, keysMeta, nodesMeta)
        {
            this.GlobalName = globalName;
        }
        public GlobalMeta(string dataGlobalName, string metaGlobalName)
        {
            this.GlobalName = dataGlobalName;
            this.GlobalMetaName = metaGlobalName;
            this.keys = new List<IKeyValidator>();
            this.nodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>();
            this.localStructs = new Dictionary<string, StructDefinition>();
        }
        public GlobalMeta()
        {
            this.localStructs = new Dictionary<string, StructDefinition>();
            this.nodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>();
            this.keys = new List<IKeyValidator>();
        }
        //
        public KeyValuePair<string, List<ValueMeta>> GetNodeMeta(int nodeIndex)
        {
            if (nodeIndex >= 0 && nodeIndex < nodesMeta.Count)
            {
                return nodesMeta[nodeIndex];
            }
            throw new IndexOutOfRangeException("Index expected in range [0, " + (nodesMeta.Count - 1) + "] entered " + nodeIndex);
        }
        //
        public void ResetRestrictionsOnly(GlobalMeta resetFrom)
        {
            //debugging information
            string curentSemantic = "";
            string sampleSemantic = "";
            //
            try
            {
                foreach (string key in resetFrom.localStructs.Keys)
                {
                    sampleSemantic = key;
                    curentSemantic = "sample structure is not found";
                    try
                    {
                        throw new NotImplementedException("To Implement ResetRestrictionsOnly in GlobalMeta");
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException(" curent: " + curentSemantic + "\n sample: " + sampleSemantic + "\n" + ex.Message);
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


        //  STRUCT WORKS:
        public void AddStruct(StructDefinition structDef)
        {
            if (!localStructs.ContainsKey(structDef.StructTypeName))
            {
                for (int i = 0; i < structDef.elementsMeta.Count; i++)
                {
                    switch (structDef.elementsMeta[i].ExtremeType)
                    {
                        case ExtremeTypes.EXTREME_STRUCT:
                            if (localStructs.ContainsKey((structDef.elementsMeta[i] as StructValMeta).structDefinition.StructTypeName))
                            {
                                (structDef.elementsMeta[i] as StructValMeta).structDefinition = localStructs[(structDef.elementsMeta[i] as StructValMeta).structDefinition.StructTypeName];
                                continue;
                            }
                            throw new ArgumentException("Struct type not found: '" + (structDef.elementsMeta[i] as StructValMeta).structDefinition.StructTypeName + "'!");
                        case ExtremeTypes.EXTREME_LIST:
                            ValueMeta listElem = getListElemInRecursiveList(structDef.elementsMeta[i] as ListValMeta);
                            switch (listElem.ExtremeType)
                            {
                                case ExtremeTypes.EXTREME_STRUCT:
                                    if (localStructs.ContainsKey((structDef.elementsMeta[i] as StructValMeta).structDefinition.StructTypeName))
                                    {
                                        (structDef.elementsMeta[i] as StructValMeta).structDefinition = localStructs[(structDef.elementsMeta[i] as StructValMeta).structDefinition.StructTypeName];
                                    }
                                    else
                                    {
                                        throw new ArgumentException("This global meta has no struct: " + (listElem as StructValMeta).structDefinition.StructTypeName);
                                    }
                                    break;
                                default:
                                    continue;
                            }
                            break;
                    }
                }
                //
                structsSequence.Enqueue(structDef.StructTypeName);
                localStructs.Add(structDef.StructTypeName, structDef);
            }
        }

        public void EditStruct(string structTypeName, StructDefinition structDef)
        {
            foreach (ValueMeta valMeta in structDef.elementsMeta)
            {
                switch (valMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        if (!localStructs.ContainsKey((valMeta as StructValMeta).structDefinition.StructTypeName))
                        {
                            throw new ArgumentException("While editing struct. has no struct: " + structDef.StructTypeName);
                        }
                        break;
                }
            }
            localStructs[structTypeName] = structDef;
        }

        public void RemoveStruct(string structTypeName)
        {
            if(localStructs.ContainsKey(structTypeName)){
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
        }

        public StructDefinition GetStructDefinition(string structTypeName)
        {
            return localStructs[structTypeName];
        }

        private void deleteStructEntries(string structTypeName)
        {
            if (localStructs.ContainsKey(structTypeName))
            {
                localStructs.Remove(structTypeName);
            }
            foreach (KeyValuePair<string, StructDefinition> keyStruct in localStructs)
            {
                for (int i = 0; i < keyStruct.Value.elementsMeta.Count; i++)
                {
                    if (keyStruct.Value.elementsMeta[i].GetCSharpTypeName().Equals(structTypeName))
                    {
                        keyStruct.Value.elementsMeta.RemoveAt(i);
                    }
                }
            }
            for (int i = 0; i < nodesMeta.Count; i++)
            {
                for (int j = 0; j < nodesMeta[i].Value.Count; j++)
                {
                    if (nodesMeta[i].Value[j].GetCSharpTypeName().Equals(structTypeName))
                    {
                        nodesMeta[i].Value.RemoveAt(j);
                    }
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                if ((keys[i] as ValueMeta).GetCSharpTypeName().Equals(structTypeName))
                {
                    this.RemoveKey(i);
                }
            }
        }


        //  KEYS WORKS:
        public void AddKeyMeta(ValueMeta key)
        {
            if (!this.keys.Exists(e => (key as ValueMeta).SemanticName == (e as ValueMeta).SemanticName))
            {
                //костыль
                Boolean isAvailable = true;
                if (((ValueMeta)key).ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    isAvailableAsKeyStruct(((StructValMeta)key).structDefinition, ref isAvailable);
                    if (this.localStructs.ContainsKey((key as StructValMeta).structDefinition.StructTypeName))
                    {
                        this.keys.Add(new StructValMeta((key as ValueMeta).SemanticName, localStructs[(key as StructValMeta).structDefinition.StructTypeName]));
                    }
                    else
                    {
                        throw new ArgumentException("Struct type not found: '" + (key as StructValMeta).structDefinition.StructTypeName + "'!");
                    }
                }
                else
                {
                    this.keys.Add((IKeyValidator)key);
                }
                nodesMeta.Add(new KeyValuePair<string, List<ValueMeta>>("", new List<ValueMeta>()));

            }
        }
        public void AddKeyMeta(ValueMeta key, string entityName)
        {
            if (!this.keys.Exists(e => (key as ValueMeta).SemanticName == (e as ValueMeta).SemanticName))
            {
                //костыль
                Boolean isAvailable = true;
                if (key.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                {
                    isAvailableAsKeyStruct(((StructValMeta)key).structDefinition, ref isAvailable);
                    if (isAvailable)
                    {
                        if (this.localStructs.ContainsKey((key as StructValMeta).structDefinition.StructTypeName))
                        {
                            StructValMeta toSet = (key as StructValMeta);
                            StructValMeta newKey = new StructValMeta(toSet.SemanticName, this.localStructs[toSet.structDefinition.StructTypeName]);
                            this.keys.Add((IKeyValidator)newKey);
                        }
                        else
                        {
                            throw new ArgumentException("Struct type not found: '" + (key as StructValMeta).structDefinition.StructTypeName + "'!");
                        }
                    }
                }
                else
                {
                    this.keys.Add((IKeyValidator)key);
                }
                nodesMeta.Add(new KeyValuePair<string, List<ValueMeta>>(entityName, new List<ValueMeta>()));
            }
        }
        private void isAvailableAsKeyStruct(StructDefinition definition, ref Boolean isAvailable)
        {
            foreach (ValueMeta valMeta in definition.elementsMeta)
            {
                switch (valMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        isAvailableAsKeyStruct(((StructValMeta)valMeta).structDefinition, ref isAvailable);
                        break;
                    case ExtremeTypes.EXTREME_BYTES:
                        isAvailable = false;
                        break;
                    case ExtremeTypes.EXTREME_LIST:
                        isAvailable = false;
                        break;
                }
            }
        }


        public void ResetKeyMeta(ValueMeta oldKey, ValueMeta newKey, string entitySemantic)
        {
            int oldIdx = this.GetKeysMeta(this.KeysCount).FindIndex(e => e.SemanticName == oldKey.SemanticName);

            if (oldIdx >= 0 && oldIdx < KeysCount)
            {
                keys[oldIdx] = (IKeyValidator)newKey;
                KeyValuePair <string, List<ValueMeta>> newNode
                    = new KeyValuePair<string, List<ValueMeta>>(entitySemantic, nodesMeta[0].Value);
                nodesMeta[oldIdx] = newNode;
                return;
            }
            throw new DataMisalignedException("reseting key error:" + oldKey.SemanticName + " index does not exist.");
        }

        public void RemoveKey(int keyIndex)
        {
            if (this.keys.Count > 0 && keyIndex >= 0 && keyIndex < KeysCount)
            {
                this.keys.RemoveAt(keyIndex);
                this.nodesMeta.RemoveAt(keyIndex);
            }
        }


        //  NODES WORKS
        public void AddValueMeta(int keyIndex, ValueMeta valueMeta)
        {
            if (keyIndex >= 0 && keyIndex < KeysCount)
            {
                switch (valueMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_STRUCT:
                        try
                        {
                            nodesMeta[keyIndex].Value.Add(valueMeta);
                            if (localStructs.ContainsKey((valueMeta as StructValMeta).structDefinition.StructTypeName))
                            {
                                StructValMeta newStructVal = new StructValMeta(valueMeta.SemanticName, (valueMeta as StructValMeta).structDefinition);
                                nodesMeta[keyIndex].Value.Add(newStructVal);
                            }
                        }
                        catch
                        {
                            throw new MissingMemberException(this.GlobalMetaName + " has no declaration of struct: " + valueMeta.GetCSharpTypeName());
                        }
                        break;
                    case ExtremeTypes.EXTREME_LIST:
                        ValueMeta listElem = getListElemInRecursiveList(valueMeta as ListValMeta);
                        if (listElem.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                        {
                            if (!localStructs.ContainsKey((listElem as StructValMeta).structDefinition.StructTypeName))
                            {
                                throw new MissingMemberException(this.GlobalMetaName + " has no declaration of struct: " + valueMeta.GetCSharpTypeName());
                            }
                        }
                        break;
                    default:
                        nodesMeta[keyIndex].Value.Add(valueMeta);
                        break;
                }
                return;
            }
            throw new ArgumentException("You entered index number: " + keyIndex + " while this meta has: " + this.keys.Count + " indicies");
        }
        public void DeleteValueMeta(int forIndex, int valIndex)
        {
            if (forIndex >= 0 && forIndex < KeysCount)
            {
                if (valIndex >= 0 && valIndex < nodesMeta[forIndex].Value.Count)
                {
                    nodesMeta[forIndex].Value.RemoveAt(valIndex);
                }
                return;
            }
            throw new ArgumentException("You entered index number: " + forIndex + " while this meta has: " + this.keys.Count + " indicies");
        }

        public void AddValuesMeta(int forIndex, List<ValueMeta> valuesMeta)
        {
            if (forIndex <= KeysCount)
            {
                nodesMeta[forIndex - 1].Value.Clear();
                nodesMeta[forIndex - 1].Value.AddRange(valuesMeta);
                for (int i = 0; i < nodesMeta[forIndex - 1].Value.Count; i++)
                {
                    switch (nodesMeta[forIndex - 1].Value[i].ExtremeType)
                    {
                        case ExtremeTypes.EXTREME_STRUCT:
                            if (localStructs.ContainsKey((valuesMeta[i] as StructValMeta).structDefinition.StructTypeName))
                            {
                                StructValMeta structMeta = new StructValMeta(valuesMeta[i].SemanticName, localStructs[(valuesMeta[i] as StructValMeta).structDefinition.StructTypeName]);
                                nodesMeta[forIndex - 1].Value[i] = structMeta;
                            }
                            else
                            {
                                throw new ArgumentException("This global meta has no struct: " + (valuesMeta[i] as StructValMeta).structDefinition.StructTypeName);
                            }
                            break;
                        case ExtremeTypes.EXTREME_LIST:
                            ValueMeta listElem = getListElemInRecursiveList(nodesMeta[forIndex - 1].Value[i] as ListValMeta);
                            switch (listElem.ExtremeType)
                            {
                                case ExtremeTypes.EXTREME_STRUCT:
                                    StructValMeta structInListMeta = (listElem as StructValMeta);
                                    if (localStructs.ContainsKey(structInListMeta.structDefinition.StructTypeName))
                                    {
                                        structInListMeta.structDefinition = localStructs[structInListMeta.structDefinition.StructTypeName];
                                    }
                                    else
                                    {
                                        nodesMeta.RemoveAt(forIndex - 1);
                                        throw new Exception("Adding in list StructValMeta failed: " + structInListMeta.structDefinition.StructTypeName + " not founded");
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }
                }
            }
        }
        public void SetValuesMeta(int forIndex, List<ValueMeta> valuesMeta, string nodeSemantic)
        {
            if (forIndex <= KeysCount)
            {
                nodesMeta[forIndex - 1] = new KeyValuePair<string, List<ValueMeta>>(nodeSemantic, valuesMeta);
                for (int i = 0; i < nodesMeta[forIndex - 1].Value.Count; i++)
                {
                    switch (nodesMeta[forIndex - 1].Value[i].ExtremeType)
                    {
                        case ExtremeTypes.EXTREME_STRUCT:
                            StructValMeta structMeta = new StructValMeta(valuesMeta[i].SemanticName, localStructs[(valuesMeta[i] as StructValMeta).structDefinition.StructTypeName]);
                            nodesMeta[forIndex - 1].Value[i] = structMeta;
                            break;
                        case ExtremeTypes.EXTREME_LIST:
                            ValueMeta listElem = getListElemInRecursiveList(nodesMeta[forIndex - 1].Value[i] as ListValMeta);
                            switch (listElem.ExtremeType)
                            {
                                case ExtremeTypes.EXTREME_STRUCT:
                                    StructValMeta structInListMeta = listElem as StructValMeta;
                                    if (localStructs.ContainsKey(structInListMeta.structDefinition.StructTypeName))
                                    {
                                        structInListMeta.structDefinition = localStructs[structInListMeta.structDefinition.StructTypeName];
                                    }
                                    else
                                    {
                                        nodesMeta.RemoveAt(forIndex - 1);
                                        throw new Exception("Adding in list StructValMeta failed: " + structInListMeta.structDefinition.StructTypeName + " not founded");
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        //need to recursively check structs in lists/lists of lists.../
                        //atention! stackoverflow exception
                        //need to set limit of recursive level
                    }
                }
            }
        }
        public void EditValueMeta(int forIndex, int valIndex, ValueMeta valueMeta)
        {
            if(forIndex < 0)
            {
                throw new IndexOutOfRangeException("out while editing valuemeta");
            }
            Exception ex = new Exception("While editing node " + nodesMeta[forIndex].Key
                                + " ; value:" + nodesMeta[forIndex].Value[valIndex].SemanticName);
            switch (valueMeta.ExtremeType)
            {
                case ExtremeTypes.EXTREME_STRUCT:
                    if (!isStructExist(valueMeta))
                    {
                        throw ex;
                    }
                    break;
                case ExtremeTypes.EXTREME_LIST:
                    ValueMeta listElemMeta = getListElemInRecursiveList(valueMeta as ListValMeta);
                    if(listElemMeta.ExtremeType == ExtremeTypes.EXTREME_STRUCT)
                    {
                        if (!isStructExist(listElemMeta))
                        {
                            throw ex;
                        }
                    }
                    break;
            }
            nodesMeta[forIndex].Value[valIndex] = valueMeta;
        }
        public bool isStructExist(ValueMeta valueMeta)
        {
            if (localStructs != null)
            {
                return localStructs.ContainsKey((valueMeta as StructValMeta).structDefinition.StructTypeName);
            }
            else
            {
                throw new Exception("Strange.. While checking struct definition existing");
            }
        }
        public bool isStructExist(StructValMeta structValMeta)
        {
            return isStructExist(structValMeta as ValueMeta);
        }
        //
        public List<ValueMeta> GetKeysMeta(int keysCount)
        {
            if (keysCount >= 0 && keysCount <= this.KeysCount)
            {
                List<ValueMeta> keysMetaToReturn = new List<ValueMeta>();
                for (int i = 0; i < keysCount; i++)
                {
                    switch (((ValueMeta)keys[i]).ExtremeType)
                    {
                        case ExtremeTypes.EXTREME_DOUBLE:
                            {
                                keysMetaToReturn.Add((DoubleValMeta)keys[i]);
                                break;
                            }
                        case ExtremeTypes.EXTREME_INT:
                            {
                                keysMetaToReturn.Add((IntValMeta)keys[i]);
                                break;
                            }
                        case ExtremeTypes.EXTREME_STRING:
                            {
                                keysMetaToReturn.Add((StringValMeta)keys[i]);
                                break;
                            }
                        case ExtremeTypes.EXTREME_STRUCT:
                            {
                                keysMetaToReturn.Add((StructValMeta)keys[i]);
                                break;
                            }
                    }
                }
                return (keysCount == 0) ? keysMetaToReturn : keysMetaToReturn.GetRange(0, keysCount);
            }
            throw new Exception("keys count mismatch");
        }

        public List<StructDefinition> GetLocalStructs()
        {
            List<StructDefinition> structsMetaToReturn = new List<StructDefinition>(localStructs.Count);
            foreach (var structMeta in localStructs)
            {
                structsMetaToReturn.Add(structMeta.Value);
            }
            return structsMetaToReturn;
        }


        //
        public EntityMeta GetEntityMeta(int indexCount)
        {
            if (indexCount > 0 && indexCount <= this.NodesWithMetaCount)
            {
                EntityMeta meta;
                meta.EntityName = this.nodesMeta[indexCount - 1].Key;
                meta.KeysValidator = this.keys.GetRange(0, indexCount);
                meta.NormilizedLocalStructs = null;
                //
                meta.ValuesMeta = this.nodesMeta[indexCount - 1].Value;
                meta.KyesMeta = GetKeysMeta(indexCount);
                meta.NormilizedLocalStructs = null;
                return meta;
            }
            throw new IndexOutOfRangeException("index count out of range");
        }

        //util
        private ValueMeta getListElemInRecursiveList(ListValMeta listMeta)
        {
            ValueMeta recursiveListElemMeta;
            int maxRecursiveLevelList = 5;
            getListElem(listMeta, out recursiveListElemMeta, maxRecursiveLevelList);
            if (recursiveListElemMeta.ExtremeType == ExtremeTypes.EXTREME_LIST)
            {
                throw new Exception("To many list in list in list in list ....");
            }
            return recursiveListElemMeta;
        }
        private void getListElem(ListValMeta list, out ValueMeta listElem, int recLevel)
        {
            ValueMeta lstElem = list.ElemMeta;
            if (lstElem.ExtremeType == ExtremeTypes.EXTREME_LIST && recLevel > 0)
            {
                getListElem(list.ElemMeta as ListValMeta, out listElem, recLevel - 1);
            }
            else
            {
                listElem = list.ElemMeta;
            }
        }
    }


    public class ProxyValidator
    {
        public ProxyValidator()
        {
        }
    }


    /// <summary>
    /// Is a part of globalsMeta. contains links on values meta only
    /// all values must link to one globalsMeta
    /// </summary>
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