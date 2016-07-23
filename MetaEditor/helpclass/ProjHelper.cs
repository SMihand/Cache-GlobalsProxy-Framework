using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Globals;
using System.Collections;

///v2.0
namespace MetaCache_v3
{
    class ProjHelper
    {
        public static string _globalName = "ProjWithMeta";
        public static string _MetaSufix = "Meta";
        public static string _ProjPrefix = "ProjInfo";
        public static string _AllFreeGlobs = "AllFreeGlobs";
        public static string _AllGlobsWithMeta = "AllGlobsWithMeta";
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
        /// Получить список всех глобалов
        /// </summary>
        /// <returns></returns>
        private ArrayList GetAllGlobals()
        {
            GlobalsDirectory gdir = _myConn.CreateGlobalsDirectory();
            ArrayList myList = new ArrayList();
            string globName = gdir.NextGlobalName();
            while (!globName.Equals(""))
            {
                if (!globName.Equals(_globalName))
                    myList.Add(globName);
                globName = gdir.NextGlobalName();
            }
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
                { myList.Add(str); }
            }
            return myList;
        }
        /// <summary>
        /// Добавить в проект новый глобал
        /// </summary>
        /// <param name="prjName">Имя проекта</param>
        /// <param name="globname">Имя Глоба</param>
        public void AddNewGlob( string prjName, string globname)
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            /* добавить обработку :
             * + выбран пункт "без меты" = > AllMeta
             * + выбран пункт "c метой"   = > AllMeta
             * + выбран пункт уже содержащий такое название 
            */

            if (!prjName.Equals(_AllGlobsWithMeta) || prjName.Equals(_AllFreeGlobs))
            {
                AddNewGlob(_AllGlobsWithMeta, globname);
                if (prjName.Equals(_AllFreeGlobs) )
                {
                    ValueList tvl = node.GetList(prjName);
                    if (tvl.Length == 0) return;
                    System.Collections.ArrayList tgl = new System.Collections.ArrayList(tvl.GetAll());
                    tgl.Remove(globname);
                    tvl.Clear();
                    //обработка пустого списка
                    tvl.Append(tgl);
                    node.SetSubscript(1, prjName);
                    node.Set(tvl);
                    tvl.Close();
                    return;
                }
            }

            ValueList vl = node.GetList(prjName);
            System.Collections.ArrayList gl = new System.Collections.ArrayList();
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

        public void CreateProjGlobal()
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            ValueList myList = _myConn.CreateList();
            //нужны обработки пустых списков проектов
            node.Set("root of proj tree");
            node.SetSubscript(1, _AllFreeGlobs);
            node.Set(ArrayListToValueList(GetAllFreeGlobals()));
            node.SetSubscript(1, _AllGlobsWithMeta);
            node.Set(ArrayListToValueList(GetAllGlobWithMeta()));

            myList.Close();
        }

        public void CreateMetaGlob(string globName)
        {
            NodeReference node = _myConn.CreateNodeReference(globName + _MetaSufix);
            node.Set(globName);
            ValueList myList = _myConn.CreateList();

            myList.Append(0);
            myList.Append("IndexDefinition");           
            node.SetSubscript(1, "Indexes");
            node.SetSubscript(2, 0);
            node.Set(myList);
            myList.Clear();

            myList.Append(0);
            myList.Append("ValueDefinition");            
            node.SetSubscript(1, "Values");
            node.SetSubscript(2, 0);
            node.Set(myList);
            myList.Clear();
            
            myList.Append(0);
            myList.Append("StructDefinition");
            node.SetSubscript(1, "Structs");
            node.SetSubscript(2, 0);
            node.Set(myList);

            myList.Close();

        }
        public object[] ReadProjGlobal()
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            List<String> myList = new List<String>();

            String subscr = node.NextSubscript("");  // start before first node
            while (!subscr.Equals(""))
            {
                myList.Add(subscr);
                subscr = node.NextSubscript(subscr);
            }

            return myList.ToArray();
        }

        public  object[] ReadProjGlobalsValue(string prjName)
        {
            NodeReference node = _myConn.CreateNodeReference(_globalName);
            List<object> myList = new List<object>();

            ValueList vl = node.GetList(prjName);
            if (vl.Length!=0)
                  myList.AddRange(vl.GetAll());
              
            return myList.ToArray() ;
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

