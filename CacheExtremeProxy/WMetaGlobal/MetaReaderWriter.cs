using CacheEXTREME2.WDirectGlobal;
using InterSystems.Globals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CacheEXTREME2.WMetaGlobal
{
    public interface IMetaReader
    {
        GlobalMeta GetMeta(string metaName);
    }

    public class MetaReaderWriter : IMetaReader
    {
        //
        /*private static Dictionary<string, ExtremeTypes> types = new Dictionary<string, ExtremeTypes>{
            {"string", ExtremeTypes.EXTREME_STRING},
            {"integer", ExtremeTypes.EXTREME_INT},
            {"double", ExtremeTypes.EXTREME_DOUBLE},
            {"bytes",ExtremeTypes.EXTREME_BYTES},
            {"list", ExtremeTypes.EXTREME_LIST},
            {"struct",ExtremeTypes.EXTREME_STRUCT}
        };*/
        //
        //readPatterns
        private List<ValueMeta> strMeta_pattern ;
        private List<ValueMeta> intMeta_pattern ;
        private List<ValueMeta> dblMeta_pattern ;
        private List<ValueMeta> btsMeta_pattern ;
        private List<ValueMeta> stcMeta_pattern ;
        private List<ValueMeta> lstMeta_pattern ;
        private List<ValueMeta> str_str_pattern;
        private List<ValueMeta> int_str_pattern;
        private List<ValueMeta> int_pattern;
        private List<ValueMeta> int_int_pattern;
        private StructByIdComparator structByIdComparator = new StructByIdComparator();
        //
        private Connection linkToConn;

        private TrueNodeReference metaGlob;
        
        //
        private GlobalMeta gmToReturn;

        //
        public MetaReaderWriter(Connection conn)
        {
            this.linkToConn = conn;
            initPattens();
        }

        public MetaReaderWriter(Connection conn, string metaGlobalName, out GlobalMeta meta)
            : this(conn)
        {
            meta = GetMeta(metaGlobalName);
        }
        private void initPattens()
        {
            StringValMeta s = new StringValMeta();
            IntValMeta i = new IntValMeta();
            DoubleValMeta d = new DoubleValMeta();
            strMeta_pattern = new List<ValueMeta>() { s, s, i, i, s };
            intMeta_pattern = new List<ValueMeta>() { s, s, i, i, i };
            dblMeta_pattern = new List<ValueMeta>() { s, s, d, d, d };
            btsMeta_pattern = new List<ValueMeta>() { s, s, i, i };
            stcMeta_pattern = new List<ValueMeta>() { s, s, s};
            lstMeta_pattern = new List<ValueMeta>() { s, s, i, i, s};
            
            str_str_pattern = new List<ValueMeta>() { s, s };
            int_str_pattern = new List<ValueMeta>() { i, s };
            int_pattern = new List<ValueMeta>() { i };
            int_int_pattern = new List<ValueMeta> { i, i };
        }
        
 
        //Read meta works:
        public GlobalMeta GetMeta(string metaGlobalName)
        {
            metaGlob = new TrueNodeReference(linkToConn, metaGlobalName);
            this.gmToReturn = new GlobalMeta();
            this.gmToReturn.GlobalMetaName = metaGlobalName;
            metaGlob = new TrueNodeReference(linkToConn, metaGlobalName);
            if (metaGlob.HasSubnodes())
            {
                getGLobalInfo(gmToReturn);
                getLocalStructs(gmToReturn);
                getKeysMeta(gmToReturn);
                getValuesMeta(gmToReturn);
                return gmToReturn;
            }
            throw new UnsuportedMetaGlobalException(metaGlobalName);
        }
        //
        private void getGLobalInfo(GlobalMeta gm)
        {
            metaGlob.Reset();
            //
            //^nMeta 	= 	(«n», «Navies») 
            ArrayList globalInfoTry = metaGlob.TryGetValues();
            ArrayList globalInfo = metaGlob.GetValues(str_str_pattern);
            gm.GlobalName = (string)globalInfo[0];
            gm.GlobalSemantic = (string)globalInfo[1];
        }

        private void getLocalStructs(GlobalMeta gm)
        {
            metaGlob.Reset();
            metaGlob.SetSubscripts(new ArrayList() { "Structs" });
            List<StructDefinition> structsDefinitions; 
            if(metaGlob.HasValues())
            {
                structsDefinitions = new List<StructDefinition>();
                //^nMeta(«Structs») 	= 	$lb(2)
                int localStructsCount = (int)metaGlob.GetAtomValue(new IntValMeta());
                ArrayList nextStructInfoKey = new ArrayList() { "Structs", ""};
                ArrayList nextStructValuesKey = new ArrayList() { "Structs", "", 0};
                metaGlob.SetSubscripts(nextStructInfoKey);
                for(int i = 1; i <= localStructsCount; i++)
                {
                    metaGlob.GoNextSubscript();
                    nextStructInfoKey = metaGlob.GetSubscripts();
                    nextStructValuesKey[1] = nextStructInfoKey[1].ToString();
                    string structTypeName = nextStructInfoKey[1].ToString();
                    //^nMeta(«Structs», «Classification») 	= 	$lb(2,2)
                    ArrayList structInfo = metaGlob.GetValues(int_int_pattern);
                    int valuesCount = (int)structInfo[0];
                    int structId = (int)structInfo[1];
                    List<ValueMeta> structsValuesMeta = new List<ValueMeta>();
                    for (int j = 1; j <= valuesCount; j++)
                    {
                        nextStructValuesKey[2] = j;
                        metaGlob.SetSubscripts(nextStructValuesKey);
                        //^nMeta(«Structs»,«ContactInfo»,1) 	= 	$lb(«Name»,«string»,0,255,"")
                        ArrayList values = readValueMeta();
                        structsValuesMeta.Add(createValueMeta(values));
                    }
                    metaGlob.SetSubscripts(nextStructInfoKey);
                    //
                    StructDefinition newStructDef = new StructDefinition();
                    newStructDef.StructTypeName = structTypeName;
                    newStructDef.elementsMeta = structsValuesMeta;
                    newStructDef.StructId = structId;
                    structsDefinitions.Add(newStructDef);
                }
                structsDefinitions.Sort(structByIdComparator);
                for (int i = 0; i < structsDefinitions.Count; i++)
                {
                    gm.AddStruct(structsDefinitions[i]);                    
                }
            }
        }
        private class StructByIdComparator : IComparer<StructDefinition>
        {
            public int Compare(StructDefinition x, StructDefinition y)
            {
                if (x.StructId > y.StructId)
                    return 1;
                if (x.StructId < y.StructId)
                    return -1;
                else return 0;
            }
        }
        //
        private void getKeysMeta(GlobalMeta gm)
        {
            ArrayList key = new ArrayList(2){"Indexes",1};
            metaGlob.Reset();
            //
            metaGlob.SetSubscripts(new ArrayList { "Indexes" });
            //^nMeta(«Indexes») 	= 	$lb(3)
            ArrayList keysInfo = metaGlob.GetValues(int_pattern);
            int indexesCount = (int)keysInfo[0];
            //
            for (int i = 1; i <= indexesCount; i++)
            {
                key[1] = i;
                metaGlob.SetSubscripts(key);
                //^nMeta(«Indexes»,1) 	= 	$lb(«Country»,«string»,0,255," Country ")
                ArrayList keyMeta = readValueMeta();
                //
                gm.AddKeyMeta(getKeyMeta(keyMeta));
            }
        }

        private ValueMeta getKeyMeta(ArrayList keyNodeList)
        {
            //keyNodeList = $lb(<KeyName>,<<TypeDeclaration>>)
            string indextype = keyNodeList[1].ToString();
            switch (indextype)
            {
                case "string":
                {  
                    return new StringValMeta(keyNodeList);
                }
                case "integer":
                {
                    return new IntValMeta(keyNodeList);
                }
                case "Int32":
                {
                    return new IntValMeta(keyNodeList);
                }
                case "double":
                {
                    return new DoubleValMeta(keyNodeList);
                }
                case "struct":
                {
                    //return new StructValMeta(keyNodeList[0].ToString(),curentLocalStructs[keyNodeList[2].ToString()]);
                    StructValMeta toReturn = new StructValMeta(keyNodeList[0].ToString(), keyNodeList[2].ToString());
                    return toReturn;
                }
            }
            return null;
        }
        //
        private void getValuesMeta(GlobalMeta gm)
        {
            ArrayList valuesMetaKey = new ArrayList() {"Values", 0 };
            ArrayList subValuesMetaKey = new ArrayList(){"Values", 0, 0};
            for (int i = 1; i <= gm.KeysCount; i++)
            {
                valuesMetaKey[1] = i;
                subValuesMetaKey[1] = i;
                metaGlob.SetSubscripts(valuesMetaKey);
                if (metaGlob.HasValues())
                {
                    //^nMeta(«Values»,1) 	= 	$lb(2, «Manufacturer»)
                    ArrayList curentNodeInfo = metaGlob.GetValues(int_str_pattern);
                    int curentNodeValuesCount = (int)curentNodeInfo[0];
                    string curentNodeName = curentNodeInfo[1] != null ? curentNodeInfo[1].ToString() : "";
                    List<ValueMeta> valuesMeta = new List<ValueMeta>(curentNodeValuesCount);
                    for (int j = 1; j <= curentNodeValuesCount; j++)
                    {
                        //{"Values", i...curentKeysCount , j...curentNodeValuesCount}
                        subValuesMetaKey[2] = j;
                        metaGlob.SetSubscripts(subValuesMetaKey);
                        //ArrayList valueMeta = metaGlob.TryGetValues();
                        ArrayList valueMeta = readValueMeta();
                        valuesMeta.Add(createValueMeta(valueMeta));
                    }
                    //
                    gm.SetValuesMeta(i, valuesMeta, curentNodeName);
                }
            }
        }

        private ValueMeta createValueMeta(ArrayList valMetaList)
        {
            //keyNodeList = $lb(<ValueName>,<<TypeDeclaration>>)
            string valyeType = valMetaList[1].ToString();
            switch (valyeType)
            {
                case "string":
                {//return new StringValMeta(valMetaList[0].ToString(), (ArrayList)valMetaList[1]);
                    return new StringValMeta(valMetaList);
                }
                case "integer":
                {
                    return new IntValMeta(valMetaList);
                }
                case "Int32":
                {
                    return new IntValMeta(valMetaList);
                }
                case "byte[]":
                {
                    return new BytesValMeta(valMetaList);
                }
                case "double":
                {
                    return new DoubleValMeta(valMetaList);
                }
                case "bytes":
                {
                    return new BytesValMeta(valMetaList);
                }
                
                case "list":
                {
                    //string listSemanticName = valMetaList[0].ToString();
                    //ValueMeta elemMeta = getValueMeta(valMetaList.GetRange(
                    return new ListValMeta(valMetaList);
                }
                case "struct":
                {
                    return new StructValMeta(valMetaList[0].ToString(),valMetaList[2].ToString());
                }
            }
            return null;
        }

        private ArrayList readValueMeta()
        {
            string type = metaGlob.GetValues(str_str_pattern)[1].ToString();
            switch (type)
            {
                case "string":
                    {
                        return metaGlob.GetValues(strMeta_pattern);
                    }
                case "integer":
                    {
                        return metaGlob.GetValues(intMeta_pattern);
                    }
                case "Int32":
                    {
                        return metaGlob.GetValues(intMeta_pattern);
                    }
                case "byte[]":
                    {
                        return metaGlob.GetValues(btsMeta_pattern);
                    }
                case "double":
                    {
                        return metaGlob.GetValues(dblMeta_pattern);
                    }
                case "bytes":
                    {
                        return metaGlob.GetValues(btsMeta_pattern);
                    }

                case "list":
                    {
                        return metaGlob.GetValues(new StringValMeta());
                    }
                case "struct":
                    {
                        return metaGlob.GetValues(stcMeta_pattern);
                    }
            }
            return null;
        }
       

        //Save meta works:
        public void SaveMeta(GlobalMeta meta)
        {
            metaGlob = new TrueNodeReference(linkToConn, meta.GlobalMetaName);
            saveGLobalInfo(meta);
            saveLocalStructs(meta);
            saveKeysMeta(meta);
            saveNodesMeta(meta);
            metaGlob.Reset();
        }
        //
        private void saveGLobalInfo(GlobalMeta meta)
        {
            //^nMeta 	= 	(«n», «Navies») 
            metaGlob.Reset();
            metaGlob.SetValuesTyped(new ArrayList { meta.GlobalName, meta.GlobalSemantic }, str_str_pattern);
        }

        private void saveLocalStructs(GlobalMeta meta)
        {
            metaGlob.Reset();
            metaGlob.AppendSubscript("Structs");
            List<StructDefinition> localStructs = meta.GetLocalStructs();
            //^nMeta(«Structs») 	= 	$lb(2)
            metaGlob.SetAtomValue(localStructs.Count);
            Queue<string> structsQueue = new Queue<string>(meta.StructSequence);
            for (int i = 0; i < localStructs.Count; i++)
            {
                StructDefinition structDef = meta.GetStructDefinition(structsQueue.Dequeue()); 
                //       indicator    name          countelements id
                //^nMeta(«Structs», «Classification») 	= 	$lb(2,2)
                metaGlob.SetValues(new ArrayList() { "Structs", structDef.StructTypeName }
                    , new ArrayList() { structDef.elementsMeta.Count , i}); // +- 1                
                for (int j = 0; j < structDef.elementsMeta.Count; j++)
                {
                    //^nMeta(«Structs»,«Classification»,1) 	= 	$lb(«ClassType»,«string»,0,255," ClassType")
                    metaGlob.SetValues(new ArrayList() { "Structs", structDef.StructTypeName, j + 1 }
                        , structDef.elementsMeta[j].Serialize());
                }
            }
        }

        private void saveKeysMeta(GlobalMeta meta)
        {
            metaGlob.Reset();
            //^nMeta(«Indexes») 	= 	$lb(3)
            metaGlob.SetValues(new ArrayList() { "Indexes" }, new ArrayList { meta.KeysCount });
            for (int i = 0; i < meta.KeysCount; i++)
            {
                //^nMeta(«Indexes»,1) 	= 	$lb(«Country», «string», 0, 255, "Country")
                metaGlob.SetValues(new ArrayList() { "Indexes", i + 1 }, meta[i].Serialize());
            }
        }

        private void saveNodesMeta(GlobalMeta meta)
        {
            metaGlob.Reset();
            ArrayList nodeInfoKey = new ArrayList { "Values", 0};
            ArrayList nodeInfoValue = new ArrayList { 0 , ""};
            ArrayList emptyNodeInfo = new ArrayList {0 , ""};
            ArrayList nodeValueKey = new ArrayList { "Values", 0, 1 };
            for (int i = 0; i < meta.NodesWithMetaCount; i++)
            {
                KeyValuePair<string, List<ValueMeta>> nodeMeta = meta.GetNodeMeta(i);
                nodeInfoValue[0] = nodeMeta.Value.Count;
                nodeInfoValue[1] = nodeMeta.Key;
                //^nMeta(«Values»,1) 	= 	$lb(2, «Manufacturer»)
                nodeInfoKey[1] = i + 1;
                metaGlob.SetValues(nodeInfoKey, nodeInfoValue);// +- 1
                if (nodeMeta.Value.Count != 0)
                {
                    nodeValueKey[1] = i + 1;
                    nodeValueKey[2] = 1;
                    for (int j = 0; j < nodeMeta.Value.Count; j++)
                    {
                        //^nMeta(«Values»,1,1) 	= 	$lb(«Charge»,«struct»,«ContactInfo») 
                        metaGlob.SetValues(nodeValueKey, nodeMeta.Value[j].Serialize());
                        nodeValueKey[2] = (int)nodeValueKey[2] + 1;
                    }
                }
                else if(nodeMeta.Value.Count == 0)
                {
                    metaGlob.SetValues(nodeInfoKey, emptyNodeInfo);
                }
            }
        }
    }

    class UnsuportedMetaGlobalException : Exception
    {
        public string globalMetaName;
        public UnsuportedMetaGlobalException(string globalMetaName)
            : base("Unsuported Meta specification in " + globalMetaName + "!")
        {
            this.globalMetaName = globalMetaName;
        }
    }


}
