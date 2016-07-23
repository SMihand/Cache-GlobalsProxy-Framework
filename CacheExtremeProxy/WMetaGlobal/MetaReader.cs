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

    public class MetaReader:IMetaReader
    {
        private Connection linkToConn;
        private TrueNodeReference metaGlob;
        //
        private int curentKeysCount;
        private string curentMetaName;
        private string curentGlobalName;
        private List<IKeyValidator> curentKeysMeta;
        private List<KeyValuePair<string, List<ValueMeta>>> curentNodesMeta;
        //
        public MetaReader(Connection conn)
        {
            this.linkToConn = conn;
        }
        public MetaReader(Connection conn, string metaGlobalName, out GlobalMeta meta)
            : this(conn)
        {
            curentMetaName = metaGlobalName;
            meta = GetMeta(metaGlobalName);
        }
        //
        private void getGLobalInfo()
        {
            metaGlob.Reset();
            ArrayList data = metaGlob.TryGetValues();
            curentGlobalName = data[0].ToString();
            metaGlob.AppendSubscript("");
            metaGlob.SetSubscript(1, 0);
            ArrayList lst = metaGlob.TryGetValues();
            curentKeysCount = (int)lst[0];
            curentMetaName = (string)lst[1];
        }
        private void getKeysMeta()
        {
            curentKeysMeta = new List<IKeyValidator>(curentKeysCount);
            ArrayList nodeValues;
            for (int i = 1; i <= curentKeysCount; i++)
            {
                metaGlob.GoNextSubscript();
                //metaGlob.SetSubscripts(new ArrayList() { "index", i });
                nodeValues = metaGlob.TryGetValues();
                curentKeysMeta.Add(getKeyMeta(nodeValues));
            }
        }
        private IKeyValidator getKeyMeta(ArrayList keyNodeList)
        {
            IKeyValidator keyMeta = null;
            if (keyNodeList[1].ToString().Equals("string"))
            {
                return new StringValMeta((ArrayList)keyNodeList);
            }
            if (keyNodeList[1].ToString().Equals("integer"))
            {
                return new IntValMeta((ArrayList)keyNodeList);
            }
            if (keyNodeList[1].ToString().Equals("double"))
            {
                return new DoubleValMeta((ArrayList)keyNodeList);
            }
            return keyMeta;
        }
        private void getValuesMeta()
        {
            curentNodesMeta = new List<KeyValuePair<string, List<ValueMeta>>>(curentKeysCount);
            for (int i = 0; i < curentKeysCount; i++)
            {
                metaGlob.SetSubscripts(new ArrayList() {i + 1, 0 });
                ArrayList curentNodeValues = metaGlob.TryGetValues();
                int curentNodeValuesCount = (int)curentNodeValues[0];
                string curentNodeName = curentNodeValues[1].ToString();
                List<ValueMeta> valuesMeta = new List<ValueMeta>(curentNodeValuesCount);
                for (int j = 1; j <= curentNodeValuesCount; j++)
                {
                    metaGlob.GoNextSubscript();
                    ArrayList valueMeta = metaGlob.TryGetValues();
                    valuesMeta.Add(getValueMeta(valueMeta));
                }
                KeyValuePair<string, List<ValueMeta>> kv
                    = new KeyValuePair<string, List<ValueMeta>>(curentNodeName, valuesMeta);
                curentNodesMeta.Add(kv);
            }
        }
        private ValueMeta getValueMeta(ArrayList valMetaList)
        {
            ValueMeta valueMeta = null;
            if (valMetaList[1].ToString().Equals("string"))
            {
                valueMeta = new StringValMeta((ArrayList)valMetaList);
            }
            if (valMetaList[1].ToString().Equals("integer"))
            {
                valueMeta = new IntValMeta((ArrayList)valMetaList);
            }
            if (valMetaList[1].ToString().Equals("double"))
            {
                valueMeta = new DoubleValMeta((ArrayList)valMetaList);
            }
            if (valMetaList[1].ToString().Equals("bytes"))
            {
                valueMeta = new BytesValMeta((ArrayList)valMetaList);
            }
            if (valMetaList[1].ToString().Equals("list"))
            {
                //^meta(2,1)=$lb("workers","list",0,100,"string",0,255,0)
                ValueMeta subvalMeta
                    = getValueMeta(valMetaList.GetRange(3, valMetaList.Count - 3));//4 is actual but compare with [1]
                valueMeta = new ListValMeta((ArrayList)valMetaList, subvalMeta);
            }
            return valueMeta;
        }
        //
        public GlobalMeta GetMeta(string metaName)
        {
            metaGlob = new TrueNodeReference(linkToConn, metaName);
            if(metaGlob.HasSubnodes())
            {
                getGLobalInfo();
                getKeysMeta();
                getValuesMeta();
                GlobalMeta gm = new GlobalMeta(curentMetaName, curentGlobalName,curentKeysMeta, curentNodesMeta);
                return gm;
            }
            throw new UnsuportedMetaGlobalException(metaName);
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
