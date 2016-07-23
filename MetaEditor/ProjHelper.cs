using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Globals;
using System.Collections;
using CacheEXTREME2.WMetaGlobal;
using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WProxyGlobal;

///v2.0
namespace MetaCache_v3
{
    class ProjHelper
    {
        public const string _globalName = "ProjWithMeta";
        public const string _MetaSufix = "Meta";
        public const string _ProjPrefix = "ProjInfo";
        public const string _AllFreeGlobs = "AllFreeGlobs";
        public const string _AllGlobsWithMeta = "AllGlobsWithMeta";
        Connection _myConn;

        public ProjHelper(Connection myConn) { this._myConn = myConn; }

        /// <summary>
        /// Проверяет, существует ли глобал с данными проектов.
        /// (проверяет на соответствие с _globalName)
        /// </summary>
        /// <returns></returns>
        public bool IsProjGlobExist()
        {
            GlobalsDirectory gdir = _myConn.CreateGlobalsDirectory();
            string globName = gdir.NextGlobalName();
            while (!globName.Equals(""))
            {
                if (globName.Equals(_globalName))
                    return true;
                globName = gdir.NextGlobalName();
            }
            return false;
        }
        
        /// <summary>
        /// Получить список ВСЕХ глобалов, в том числе и метаглобов.
        /// </summary>
        /// <returns></returns>
        private ArrayList GetAllGlobals()
        {
            GlobalsDirectory gdir = _myConn.CreateGlobalsDirectory();
            ArrayList myList = new ArrayList();
            string globName = gdir.NextGlobalName();
            while (!globName.Equals(""))
            {
                myList.Add(globName);
                globName = gdir.NextGlobalName();
            }
            myList.Remove(_globalName);
            return myList;
        }
        /// <summary>
        /// Получить список глобалов без Меты
        /// </summary>
        /// <returns></returns>
        public  ArrayList GetAllFreeGlobals()
        {
            //ValueList myList = _myConn.CreateList();
                ArrayList allglobs = GetAllGlobals();
                ArrayList globswithmeta = GetAllGlobWithMeta();
            foreach  (String str in globswithmeta)
            {           
                allglobs.Remove(str);
                allglobs.Remove(str + _MetaSufix);
            }
            return allglobs;
        }
        /// <summary>
        /// Получить все глобалы с метой
        /// </summary>
        /// <returns></returns>
        public  ArrayList GetAllGlobWithMeta()
        {
            ArrayList myList = new ArrayList();
            ArrayList allglobs = GetAllGlobals();
            foreach (String str in allglobs)
            {
                if (allglobs.Contains(str + _MetaSufix))
                {
                    myList.Add(str);
                }
            }
            return myList;
        }
        /// <summary>
        /// Добавить в проект новый глобал
        /// </summary>
        /// <param name="prjName">Имя проекта</param>
        /// <param name="globname">Имя Глоба</param>
        public void AddNewGlobToProj(string prjName, string globname)
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);

            ValueList vl = node.GetList(prjName);
            ArrayList gl = new ArrayList();
            if (vl.Length != 0)
                gl.AddRange(vl.GetAll());
            if (!gl.Contains(globname))
            {
                vl.Append(globname);
                node.SetSubscript(1, prjName);
                node.Set(vl);
            }
            vl.Close();
        }

        public void SetProjData(string projName, object[] listOfGlobs)
        {
            NodeReference node = _myConn.CreateNodeReference(ProjHelper._globalName);
            ValueList myList = _myConn.CreateList();
            //if (!projName.Equals(""))
            //{
            //    node.SetSubscript(1, projName);
            //    node.Kill();
            //}
            node.SetSubscript(1, projName);
            foreach (string str in listOfGlobs)
                myList.Append(str);
            node.Set(myList);
            myList.Close();
        }

        public void CreateProjGlobal()
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            ValueList myList = _myConn.CreateList();
            node.Set("root of proj tree");
            myList.Close();
        }
        /// <summary>
        /// Создаем заготовку метаглоба по имени дата глоба 
        /// </summary>
        /// <param name="DataGlobName"></param>
        public void CreateMetaGlob(string DataGlobName)
        {
            NodeReference node = _myConn.CreateNodeReference(DataGlobName + _MetaSufix);
            node.Set(DataGlobName);
            ValueList myList = _myConn.CreateList();

            myList.Append(0);
            myList.Append(DataGlobName+"Meta");           
            node.SetSubscript(1, "Indexes");
            node.SetSubscript(2, 0);
            node.Set(myList);
            myList.Clear();
            //
            node.SetSubscriptCount(0);
            myList.Append(0);
            node.SetSubscript(1, "Structs");
            node.Set(myList);

            myList.Close();

        }
        /// <summary>
        /// list of proj?
        /// </summary>
        /// <returns></returns>
        public object[] ReadProjGlobal()
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            List<String> myList = new List<String>();

            String subscr = node.NextSubscript("");  
            while (!subscr.Equals(""))
            {
                myList.Add(subscr);
                subscr = node.NextSubscript(subscr);
            }

            return myList.ToArray();
        }
        /// <summary>
        /// read proj value - retur list of globs
        /// </summary>
        /// <param name="prjName"></param>
        /// <returns></returns>
        public object[] ReadProjGlobalsValue(string prjName)
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            List<object> myList = new List<object>();
            if (prjName.Trim().Length != 0 && node.HasData())
            
                switch (prjName)
                {
                    case (_AllFreeGlobs): return GetAllFreeGlobals().ToArray();
                    case (_AllGlobsWithMeta): return GetAllGlobWithMeta().ToArray();
                    case (""): break;
                    default:
                        ValueList vl = node.GetList(prjName);
                        if (vl!= null && vl.Length != 0)
                            myList.AddRange(vl.GetAll());
                        break;
                }
            
            return myList.ToArray();
        }

        public ArrayList ValueListToArrayList(ValueList vl)
        {
            ArrayList myList = new ArrayList(vl.Length);
            for (int i = 0; i < vl.Length; i++)
            {
                myList.Add(vl.GetNextString());
            }
            return myList;
        }

        public ValueList ArrayListToValueList(ArrayList ar)
        {
            ValueList vl = _myConn.CreateList();
            foreach (String str in ar)
            {
                vl.Append(str);
            }
            return vl;
        }
    }
}

