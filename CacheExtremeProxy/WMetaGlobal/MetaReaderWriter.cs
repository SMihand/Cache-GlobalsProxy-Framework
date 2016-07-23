using CacheEXTREME2.WDirectGlobal;
using InterSystems.Globals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CacheEXTREME2.WMetaGlobal
{

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
        private List<ValueMeta> str_pattern ;
        private List<ValueMeta> int_pattern ;
        private List<ValueMeta> dbl_pattern ;
        private List<ValueMeta> bts_pattern ;
        private List<ValueMeta> stc_pattern ;
        private List<ValueMeta> lst_pattern ;
        private List<ValueMeta> str_str_;
        private List<ValueMeta> int_str_;
        private List<ValueMeta> int_;
        //
        private Connection linkToConn;
        private TrueNodeReference metaGlob;
        //
        private GlobalMeta gmToReturn;
        //
        private int curentKeysCount;
        private string curentMetaGlobalName;
        private string curentDataGlobalName;
        private List<IKeyValidator> curentKeysMeta;
        private List<KeyValuePair<string, List<ValueMeta>>> curentNodesMeta;
        private Dictionary<string, StructValMeta> curentLocalStructs;
        //
        public MetaReaderWriter(Connection conn)
        {
            this.linkToConn = conn;
            initPattens();
        }
        public MetaReaderWriter(Connection conn, string metaGlobalName, out GlobalMeta meta)
            : this(conn)
        {
            curentMetaGlobalName = metaGlobalName;
            meta = GetMeta(metaGlobalName);
        }
        private void initPattens()
        {
            StringValMeta s = new StringValMeta();
            IntValMeta i = new IntValMeta();
            DoubleValMeta d = new DoubleValMeta();
            str_pattern = new List<ValueMeta>() { s, s, i, i, s };
            int_pattern = new List<ValueMeta>() { s, s, i, i, i };
            dbl_pattern = new List<ValueMeta>() { s, s, d, d, d };
            bts_pattern = new List<ValueMeta>() { s, s, i };
            stc_pattern = new List<ValueMeta>() { s, s, s};
            lst_pattern = new List<ValueMeta>() { s, s, i, i, s};
            
            str_str_ = new List<ValueMeta>() { s, s };
            int_str_ = new List<ValueMeta>() { i, s };
            int_ = new List<ValueMeta>() { i };
        }
        //
        private void getGLobalInfo()
        {
            metaGlob.Reset();
            //
            ValueMeta globalMeta = new StringValMeta();
            object globalName = metaGlob.GetAtomValue(globalMeta);
            curentDataGlobalName = globalName.ToString();
            //
            metaGlob.SetSubscripts(new ArrayList() { "Indexes", 0 });
            ArrayList lst = metaGlob.GetValues(int_str_);
            curentKeysCount = (int)lst[0];
            curentMetaGlobalName = (string)lst[1];
        }
        private void getLocalStructs()
        {
            metaGlob.Reset();
            curentLocalStructs = new Dictionary<string, StructValMeta>();
            metaGlob.SetSubscripts(new ArrayList() { "Structs" });
            if(metaGlob.HasValues())
            {
                int localStructsCount = (int)metaGlob.GetAtomValue(new IntValMeta());
                ArrayList structsInfoKey = new ArrayList() { "Structs", ""};
                ArrayList structsValuesKey = new ArrayList() { "Structs", "", 0};
                metaGlob.SetSubscripts(structsInfoKey);
                for(int i = 1; i<=localStructsCount; i++)
                {
                    metaGlob.GoNextSubscript();
                    structsInfoKey = metaGlob.GetSubscripts();
                    structsValuesKey[1] = structsInfoKey[1];
                    string structName = structsInfoKey[1].ToString();
                    ArrayList structInfo = metaGlob.GetValues(int_);
                    int valuesCount = (int)structInfo[0];
                    List<ValueMeta> structsValuesMeta = new List<ValueMeta>();
                    for (int j = 1; j <= valuesCount; j++)
                    {
                        structsValuesKey[2] = j;
                        metaGlob.SetSubscripts(structsValuesKey);
                        //ArrayList values = metaGlob.TryGetValues();
                        ArrayList values = readValueMeta();
                        structsValuesMeta.Add(getValueMeta(values));
                    }
                    metaGlob.SetSubscripts(structsInfoKey);
                    //
                    gmToReturn.AddStruct(new StructValMeta(structName, structsValuesMeta));
                    //
                    curentLocalStructs.Add(structName, new StructValMeta(structName, structsValuesMeta));
                }
            }
        }
        //
        private void getKeysMeta()
        {
            curentKeysMeta = new List<IKeyValidator>(curentKeysCount);
            ArrayList key = new ArrayList(2){"Indexes",0};
            for (int i = 1; i <= curentKeysCount; i++)
            {
                key[1] = i;
                metaGlob.SetSubscripts(key);
                //ArrayList keyMeta = metaGlob.TryGetValues();
                ArrayList keyMeta = readValueMeta();
                //
                gmToReturn.AddKeyMeta(getKeyMeta(keyMeta));
                //
                curentKeysMeta.Add(getKeyMeta(keyMeta));
            }
        }
        private IKeyValidator getKeyMeta(ArrayList keyNodeList)
        {
            //keyNodeList = $lb(<KeyName>,<<TypeDeclaration>>)
            string indextype = keyNodeList[1].ToString();
            switch (indextype)
            {
                case "string":
                {   //return new StringValMeta(keyNodeList[0].ToString(),(ArrayList)keyNodeList[1]);
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
                    return new StructValMeta(keyNodeList[0].ToString(), keyNodeList[2].ToString());
                }
            }
            return null;
        }
        //
        private void getValuesMeta()
        {
            curentNodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>(curentKeysCount);
            ArrayList valuesMetaKey = new ArrayList() {"Values", 0, 0};
            ArrayList subValuesMetaKey = new ArrayList(){"Values",0,0};
            for (int i = 1; i <= curentKeysCount; i++)
            {
                //{"Values", i...curentKeysCount , 0}
                valuesMetaKey[1] = i;
                metaGlob.SetSubscripts(valuesMetaKey);
                ArrayList curentNodeValues = metaGlob.GetValues(int_str_);
                int curentNodeValuesCount = (int)curentNodeValues[0];
                string curentNodeName = curentNodeValues[1]!=null?curentNodeValues[1].ToString():"";
                List<ValueMeta> valuesMeta = new List<ValueMeta>(curentNodeValuesCount);
                for (int j = 1; j <= curentNodeValuesCount; j++)
                {
                    //{"Values", i...curentKeysCount , j...curentNodeValuesCount}
                    subValuesMetaKey[1] = i;
                    subValuesMetaKey[2] = j;
                    metaGlob.SetSubscripts(subValuesMetaKey);
                    //ArrayList valueMeta = metaGlob.TryGetValues();
                    ArrayList valueMeta = readValueMeta();
                    valuesMeta.Add(getValueMeta(valueMeta));
                }
                KeyValuePair<string, List<ValueMeta>> kv
                    = new KeyValuePair<string, List<ValueMeta>>(curentNodeName, valuesMeta);
                //
                gmToReturn.SetValuesMeta(i, valuesMeta, curentNodeName);
                //
                curentNodesMeta.Add(kv);
            }
        }
        private ValueMeta getValueMeta(ArrayList valMetaList)
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
        //
        private ArrayList readValueMeta()
        {
            string type = metaGlob.GetValues(str_str_)[1].ToString();
            switch (type)
            {
                case "string":
                    {
                        return metaGlob.GetValues(str_pattern);
                    }
                case "integer":
                    {
                        return metaGlob.GetValues(int_pattern);
                    }
                case "Int32":
                    {
                        return metaGlob.GetValues(int_pattern);
                    }
                case "byte[]":
                    {
                        return metaGlob.GetValues(bts_pattern);
                    }
                case "double":
                    {
                        return metaGlob.GetValues(dbl_pattern);
                    }
                case "bytes":
                    {
                        return metaGlob.GetValues(bts_pattern);
                    }

                case "list":
                    {
                        return metaGlob.GetValues(new StringValMeta());
                    }
                case "struct":
                    {
                        return metaGlob.GetValues(stc_pattern);
                    }
            }
            return null;
        }
        //
        //
        public GlobalMeta GetMeta(string metaName)
        {
            metaGlob = new TrueNodeReference(linkToConn, metaName);
            if(metaGlob.HasSubnodes())
            {
                getGLobalInfo();
                //
                gmToReturn = new GlobalMeta(curentDataGlobalName, curentMetaGlobalName);
                //
                getLocalStructs();
                getKeysMeta();
                getValuesMeta();
                return gmToReturn;
            }
            throw new UnsuportedMetaGlobalExceptionA2(metaName);
        }
        public void SaveMeta(GlobalMeta meta)
        {
            initCurent(meta);
            save();
        }
        public void SaveMeta(string globalMetaName, GlobalMeta meta)
        {
            initCurent(meta);
            curentMetaGlobalName = globalMetaName;
            save();
        }
        //
        private void initCurent(GlobalMeta meta)
        {
            curentMetaGlobalName = meta.GlobalMetaName;
            curentDataGlobalName = meta.GlobalName;
            curentKeysCount = meta.KeysCount;
            curentKeysMeta = meta.GetKeysValidator();
            curentNodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>();
            for (int i = 1; i <= meta.KeysCount; i++)
            {
                curentNodesMeta.Add(meta.GetEntityValuesMeta(i));
            }
            curentLocalStructs = new Dictionary<string, StructValMeta>();
            List<StructValMeta> localStructs = meta.GetLocalStructs();
            for (int i = 0; i < localStructs.Count; i++)
            {
                curentLocalStructs.Add(localStructs[i].StructTypeName, localStructs[i]);
            }
        }
        private void save()
        {
            metaGlob = new TrueNodeReference(linkToConn, curentMetaGlobalName);
            SaveGLobalInfo();
            SaveKeysMeta();
            SaveValuesMeta();
            SetLocalStructs();
            metaGlob.Reset();
        }
        //
        private void SetLocalStructs()
        {
            metaGlob.Reset();
            metaGlob.AppendSubscript("Structs");
            metaGlob.SetAtomValue(curentLocalStructs.Count);
            List<string> structsNames = new List<string>(curentLocalStructs.Keys);
            foreach (string svmName in structsNames)
            {
                metaGlob.SetValues(new ArrayList() { "Structs", svmName }, new ArrayList() { curentLocalStructs[svmName].elementsMeta.Count }); // +- 1                
                for (int j = 0; j < curentLocalStructs[svmName].elementsMeta.Count; j++)
                {
                    metaGlob.SetValues(new ArrayList() { "Structs", svmName, j + 1 }, curentLocalStructs[svmName].elementsMeta[j].Serialize());
                }
            }
        }

        private void SaveGLobalInfo()
        {
            metaGlob.Reset();
            metaGlob.SetAtomValue(curentDataGlobalName);
        }

        private void SaveKeysMeta()
        {
            metaGlob.Reset();
            metaGlob.SetValues(new ArrayList() { "Indexes", 0 }, new ArrayList { curentKeysCount, curentMetaGlobalName });
            for (int i = 0; i < curentKeysCount; i++)
            {
                metaGlob.SetValues(new ArrayList() { "Indexes", i + 1 }, ((ValueMeta)curentKeysMeta[i]).Serialize());
            }
        }

        private void SaveValuesMeta()
        {
            metaGlob.Reset();
            //metaGlob.SetValues(new ArrayList() { "Values", 0 }, new ArrayList() { curentNodesMeta.Count, "ValueDefinition" });
            for (int i = 0; i < curentKeysCount; i++)
            {
                ArrayList counter = new ArrayList { curentNodesMeta[i].Value.Count, curentNodesMeta[i].Key };
                metaGlob.SetValues(new ArrayList() { "Values", i + 1, 0 }, counter);// +- 1
                if (curentNodesMeta[i].Value.Count != 0)
                {
                    KeyValuePair<string, List<ValueMeta>> kvp = curentNodesMeta[i];
                    ArrayList valueKey = new ArrayList() { "Values", i + 1, 1 };
                    for (int j = 0; j < kvp.Value.Count; j++)
                    {
                        metaGlob.SetValues(valueKey, kvp.Value[j].Serialize());
                        valueKey[2] = (int)valueKey[2] + 1;
                    }
                }
            }
        }
    }

    public class MetaReaderA2_Deprecated : IMetaReader
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
        private List<ValueMeta> str_pattern;
        private List<ValueMeta> int_pattern;
        private List<ValueMeta> dbl_pattern;
        private List<ValueMeta> bts_pattern;
        private List<ValueMeta> stc_pattern;
        private List<ValueMeta> lst_pattern;
        private List<ValueMeta> str_str_;
        private List<ValueMeta> int_str_;
        private List<ValueMeta> int_;
        //
        private Connection linkToConn;
        private TrueNodeReference metaGlob;
        //
        private int curentKeysCount;
        private string curentMetaGlobalName;
        private string curentDataGlobalName;
        private List<IKeyValidator> curentKeysMeta;
        private List<KeyValuePair<string, List<ValueMeta>>> curentNodesMeta;
        private Dictionary<string, StructValMeta> curentLocalStructs;
        //
        public MetaReaderA2_Deprecated(Connection conn)
        {
            this.linkToConn = conn;
            initPattens();
        }
        public MetaReaderA2_Deprecated(Connection conn, string metaGlobalName, out GlobalMeta meta)
            : this(conn)
        {
            curentMetaGlobalName = metaGlobalName;
            meta = GetMeta(metaGlobalName);
        }
        private void initPattens()
        {
            StringValMeta s = new StringValMeta();
            IntValMeta i = new IntValMeta();
            DoubleValMeta d = new DoubleValMeta();
            str_pattern = new List<ValueMeta>() { s, s, i, i, s };
            int_pattern = new List<ValueMeta>() { s, s, i, i, i };
            dbl_pattern = new List<ValueMeta>() { s, s, d, d, d };
            bts_pattern = new List<ValueMeta>() { s, s, i };
            stc_pattern = new List<ValueMeta>() { s, s, s };
            lst_pattern = new List<ValueMeta>() { s, s, i, i, s };

            str_str_ = new List<ValueMeta>() { s, s };
            int_str_ = new List<ValueMeta>() { i, s };
            int_ = new List<ValueMeta>() { i };
        }
        //
        private void getGLobalInfo()
        {
            metaGlob.Reset();
            //
            ValueMeta globalMeta = new StringValMeta();
            object globalName = metaGlob.GetAtomValue(globalMeta);
            curentDataGlobalName = globalName.ToString();
            //
            metaGlob.SetSubscripts(new ArrayList() { "Indexes", 0 });
            ArrayList lst = metaGlob.GetValues(int_str_);
            curentKeysCount = (int)lst[0];
            curentMetaGlobalName = (string)lst[1];
        }
        private void getLocalStructs()
        {
            metaGlob.Reset();
            curentLocalStructs = new Dictionary<string, StructValMeta>();
            metaGlob.SetSubscripts(new ArrayList() { "Structs" });
            if (metaGlob.HasValues())
            {
                int localStructsCount = (int)metaGlob.GetAtomValue(new IntValMeta());
                ArrayList structsInfoKey = new ArrayList() { "Structs", "" };
                ArrayList structsValuesKey = new ArrayList() { "Structs", "", 0 };
                metaGlob.SetSubscripts(structsInfoKey);
                for (int i = 1; i <= localStructsCount; i++)
                {
                    metaGlob.GoNextSubscript();
                    structsInfoKey = metaGlob.GetSubscripts();
                    structsValuesKey[1] = structsInfoKey[1];
                    string structName = structsInfoKey[1].ToString();
                    ArrayList structInfo = metaGlob.GetValues(int_);
                    int valuesCount = (int)structInfo[0];
                    List<ValueMeta> structsValuesMeta = new List<ValueMeta>();
                    for (int j = 1; j <= valuesCount; j++)
                    {
                        structsValuesKey[2] = j;
                        metaGlob.SetSubscripts(structsValuesKey);
                        //ArrayList values = metaGlob.TryGetValues();
                        ArrayList values = readValueMeta();
                        structsValuesMeta.Add(getValueMeta(values));
                    }
                    metaGlob.SetSubscripts(structsInfoKey);
                    curentLocalStructs.Add(structName, new StructValMeta(structName, structsValuesMeta));
                }
            }
        }
        //
        private void getKeysMeta()
        {
            curentKeysMeta = new List<IKeyValidator>(curentKeysCount);
            ArrayList key = new ArrayList(2) { "Indexes", 0 };
            for (int i = 1; i <= curentKeysCount; i++)
            {
                key[1] = i;
                metaGlob.SetSubscripts(key);
                //ArrayList keyMeta = metaGlob.TryGetValues();
                ArrayList keyMeta = readValueMeta();
                curentKeysMeta.Add(getKeyMeta(keyMeta));
            }
        }
        private IKeyValidator getKeyMeta(ArrayList keyNodeList)
        {
            //keyNodeList = $lb(<KeyName>,<<TypeDeclaration>>)
            string indextype = keyNodeList[1].ToString();
            switch (indextype)
            {
                case "string":
                    {   //return new StringValMeta(keyNodeList[0].ToString(),(ArrayList)keyNodeList[1]);
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
                        return new StructValMeta(keyNodeList[0].ToString(), keyNodeList[2].ToString());
                    }
            }
            return null;
        }
        //
        private void getValuesMeta()
        {
            curentNodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>(curentKeysCount);
            ArrayList valuesMetaKey = new ArrayList() { "Values", 0, 0 };
            ArrayList subValuesMetaKey = new ArrayList() { "Values", 0, 0 };
            for (int i = 1; i <= curentKeysCount; i++)
            {
                //{"Values", i...curentKeysCount , 0}
                valuesMetaKey[1] = i;
                metaGlob.SetSubscripts(valuesMetaKey);
                ArrayList curentNodeValues = metaGlob.GetValues(int_str_);
                int curentNodeValuesCount = (int)curentNodeValues[0];
                string curentNodeName = curentNodeValues[1] != null ? curentNodeValues[1].ToString() : "";
                List<ValueMeta> valuesMeta = new List<ValueMeta>(curentNodeValuesCount);
                for (int j = 1; j <= curentNodeValuesCount; j++)
                {
                    //{"Values", i...curentKeysCount , j...curentNodeValuesCount}
                    subValuesMetaKey[1] = i;
                    subValuesMetaKey[2] = j;
                    metaGlob.SetSubscripts(subValuesMetaKey);
                    //ArrayList valueMeta = metaGlob.TryGetValues();
                    ArrayList valueMeta = readValueMeta();
                    valuesMeta.Add(getValueMeta(valueMeta));
                }
                KeyValuePair<string, List<ValueMeta>> kv
                    = new KeyValuePair<string, List<ValueMeta>>(curentNodeName, valuesMeta);
                curentNodesMeta.Add(kv);
            }
        }
        private ValueMeta getValueMeta(ArrayList valMetaList)
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
                        return new StructValMeta(valMetaList[0].ToString(), valMetaList[2].ToString());
                    }
            }
            return null;
        }
        //
        private ArrayList readValueMeta()
        {
            string type = metaGlob.GetValues(str_str_)[1].ToString();
            switch (type)
            {
                case "string":
                    {
                        return metaGlob.GetValues(str_pattern);
                    }
                case "integer":
                    {
                        return metaGlob.GetValues(int_pattern);
                    }
                case "Int32":
                    {
                        return metaGlob.GetValues(int_pattern);
                    }
                case "byte[]":
                    {
                        return metaGlob.GetValues(bts_pattern);
                    }
                case "double":
                    {
                        return metaGlob.GetValues(dbl_pattern);
                    }
                case "bytes":
                    {
                        return metaGlob.GetValues(bts_pattern);
                    }

                case "list":
                    {
                        return metaGlob.GetValues(new StringValMeta());
                    }
                case "struct":
                    {
                        return metaGlob.GetValues(stc_pattern);
                    }
            }
            return null;
        }
        //
        //
        public GlobalMeta GetMeta(string metaName)
        {
            metaGlob = new TrueNodeReference(linkToConn, metaName);
            if (metaGlob.HasSubnodes())
            {
                getGLobalInfo();
                getLocalStructs();
                getKeysMeta();
                getValuesMeta();
                GlobalMeta gm = new GlobalMeta(curentMetaGlobalName, curentDataGlobalName, curentKeysMeta, curentNodesMeta);
                foreach (KeyValuePair<string, StructValMeta> _struct in curentLocalStructs)
                {
                    gm.AddStruct(_struct.Key, _struct.Value);
                }
                return gm;
            }
            throw new UnsuportedMetaGlobalExceptionA2(metaName);
        }
        public void SaveMeta(GlobalMeta meta)
        {
            initCurent(meta);
            save();
        }
        public void SaveMeta(string globalMetaName, GlobalMeta meta)
        {
            initCurent(meta);
            curentMetaGlobalName = globalMetaName;
            save();
        }
        //
        private void initCurent(GlobalMeta meta)
        {
            curentMetaGlobalName = meta.GlobalMetaName;
            curentDataGlobalName = meta.GlobalName;
            curentKeysCount = meta.KeysCount;
            curentKeysMeta = meta.GetKeysValidator();
            curentNodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>();
            for (int i = 1; i <= meta.KeysCount; i++)
            {
                curentNodesMeta.Add(meta.GetEntityValuesMeta(i));
            }
            curentLocalStructs = new Dictionary<string, StructValMeta>();
            List<StructValMeta> localStructs = meta.GetLocalStructs();
            for (int i = 0; i < localStructs.Count; i++)
            {
                curentLocalStructs.Add(localStructs[i].StructTypeName, localStructs[i]);
            }
        }
        private void save()
        {
            metaGlob = new TrueNodeReference(linkToConn, curentMetaGlobalName);
            SaveGLobalInfo();
            SaveKeysMeta();
            SaveValuesMeta();
            SetLocalStructs();
            metaGlob.Reset();
        }
        //
        private void SetLocalStructs()
        {
            metaGlob.Reset();
            metaGlob.AppendSubscript("Structs");
            metaGlob.SetAtomValue(curentLocalStructs.Count);
            List<string> structsNames = new List<string>(curentLocalStructs.Keys);
            foreach (string svmName in structsNames)
            {
                metaGlob.SetValues(new ArrayList() { "Structs", svmName }, new ArrayList() { curentLocalStructs[svmName].elementsMeta.Count }); // +- 1                
                for (int j = 0; j < curentLocalStructs[svmName].elementsMeta.Count; j++)
                {
                    metaGlob.SetValues(new ArrayList() { "Structs", svmName, j + 1 }, curentLocalStructs[svmName].elementsMeta[j].Serialize());
                }
            }
        }

        private void SaveGLobalInfo()
        {
            metaGlob.Reset();
            metaGlob.SetAtomValue(curentDataGlobalName);
        }

        private void SaveKeysMeta()
        {
            metaGlob.Reset();
            metaGlob.SetValues(new ArrayList() { "Indexes", 0 }, new ArrayList { curentKeysCount, curentMetaGlobalName });
            for (int i = 0; i < curentKeysCount; i++)
            {
                metaGlob.SetValues(new ArrayList() { "Indexes", i + 1 }, ((ValueMeta)curentKeysMeta[i]).Serialize());
            }
        }

        private void SaveValuesMeta()
        {
            metaGlob.Reset();
            //metaGlob.SetValues(new ArrayList() { "Values", 0 }, new ArrayList() { curentNodesMeta.Count, "ValueDefinition" });
            for (int i = 0; i < curentKeysCount; i++)
            {
                ArrayList counter = new ArrayList { curentNodesMeta[i].Value.Count, curentNodesMeta[i].Key };
                metaGlob.SetValues(new ArrayList() { "Values", i + 1, 0 }, counter);// +- 1
                if (curentNodesMeta[i].Value.Count != 0)
                {
                    KeyValuePair<string, List<ValueMeta>> kvp = curentNodesMeta[i];
                    ArrayList valueKey = new ArrayList() { "Values", i + 1, 1 };
                    for (int j = 0; j < kvp.Value.Count; j++)
                    {
                        metaGlob.SetValues(valueKey, kvp.Value[j].Serialize());
                        valueKey[2] = (int)valueKey[2] + 1;
                    }
                }
            }
        }
    }

    class UnsuportedMetaGlobalExceptionA2 : Exception
    {
        public string globalMetaName;
        public UnsuportedMetaGlobalExceptionA2(string globalMetaName)
            : base("Unsupoted Meta specification in " + globalMetaName + "!")
        {
            this.globalMetaName = globalMetaName;
        }
    }

}
