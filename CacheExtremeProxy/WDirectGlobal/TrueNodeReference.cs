using InterSystems.Globals;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using CacheEXTREME2.WMetaGlobal;
using System.Collections.Generic;

namespace CacheEXTREME2.WDirectGlobal
{    

    public class TrueNodeReference
    {
        private string globalName;
        public string GlobalName { get { return globalName; } }
        //
        private Connection linkToConn;
        public Connection Conn { get { return linkToConn; } }
        //
        private ArrayList subscripts;

        public int SubsCount { get { return subscripts.Count; } }

        public object this[int index]{get{return subscripts[index];}}
        //
        private NodeReference DirectReference;
        
      //CONSTRUCTORS
        public TrueNodeReference(Connection conn, string globalName)
        {
            this.globalName = globalName;
            DirectReference = conn.CreateNodeReference(globalName);
            DirectReference.SetName(globalName);
            subscripts = new ArrayList(100);
            linkToConn = conn;
        }

        public TrueNodeReference(Connection conn, string globalName, ArrayList subscriptsTyped):this(conn,globalName)
        {
            SetSubscripts(subscriptsTyped);
        }

        public TrueNodeReference(Connection conn, NodeReference reference)
        {
            this.linkToConn = conn;
            DirectReference = conn.CreateNodeReference(reference.GetName());
            DirectReference.SetName(reference.GetName());
            subscripts = TryGetTypedSubscripts(reference);
            SetSubscripts(DirectReference,subscripts);
            //WHY ONLY NOW????
            this.globalName = reference.GetName();
        }

        public TrueNodeReference(TrueNodeReference reference, ArrayList subscripts = null) 
            : this(reference.Conn, reference.GetName()) 
        {
            if (subscripts != null)
            {
                SetSubscripts(subscripts);
            }
        }

      //SUBSCRIPTS WORKS
        public void SetSubscripts(ArrayList subscriptsTyped)
        {
            try
            {
                TrueNodeReference.SetSubscripts(DirectReference, subscriptsTyped);
            }
            catch (SubscriptTypeException ex)
            {
                this.subscripts = new ArrayList(subscriptsTyped.GetRange(0,ex.OnSubscriptIndex-1));//MUST BE OPTIMIZED
                throw ex;
            }
            this.subscripts = new ArrayList(subscriptsTyped.ToArray());//MUST BE OPTIMIZED
        }

        public void SetSubscripts(ArrayList subscriptsTyped, List<ValueMeta> subscriptsMeta)
        {
            try
            {
                TrueNodeReference.SetSubscripts(DirectReference, subscriptsTyped, subscriptsMeta);
            }
            catch (SubscriptTypeException ex)
            {
                this.subscripts = new ArrayList(subscriptsTyped.GetRange(0, ex.OnSubscriptIndex - 1));//MUST BE OPTIMIZED
                throw ex;
            }
            this.subscripts = new ArrayList(subscriptsTyped.ToArray());//MUST BE OPTIMIZED
        }

        public void AppendSubscripsts(IList subscripts, IList<ValueMeta> subscriptsMeta)
        {
            for (int i = 0; i < subscriptsMeta.Count; i++)
            {
                switch (subscriptsMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_DOUBLE:
                        {
                            DirectReference.AppendSubscript(CacheDouble.DoubleToStringSubscriptKey((double)subscripts[i]));
                            this.subscripts.Add(subscripts[i]);
                            continue;
                        }
                    case ExtremeTypes.EXTREME_INT:
                        {
                            DirectReference.AppendSubscript((int)subscripts[i]);
                            this.subscripts.Add(subscripts[i]);
                            continue;
                        }
                    case ExtremeTypes.EXTREME_STRING:
                        {
                            DirectReference.AppendSubscript(subscripts[i].ToString());
                            this.subscripts.Add(subscripts[i]);
                            continue;
                        }
                    case ExtremeTypes.EXTREME_STRUCT:
                        {
                            this.AppendSubscripsts((IList)subscripts[i], ((StructValMeta)subscriptsMeta[i]).structDefinition.elementsMeta);
                            continue;
                        }
                    /*case ExtremeTypes.EXTREME_INT:
                        {
                            reference.SetSubscript(position, (long)subscript);
                            return;
                        }*/
                }
            }
            //TrueNodeReference.AppendSubscripts(this.DirectReference, subscripts, subscriptsMeta);
        }

        public void SetSubscript(int position, object subscript)
        {
            try
            {
                if (position <= subscripts.Count)
                {
                    SetSubscript(this.DirectReference, position, subscript);
                    subscripts[position - 1] = subscript;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("position"
                        , position
                        , "Curent subscript count is " + SubsCount); 
                }
            }
            catch (Exception ex) 
            { 
                throw ex; 
            }
        }

        public ArrayList GetSubscripts()
        {
            return new ArrayList(subscripts.ToArray());
        }

        public void Reset()
        {
            subscripts.Clear();
            DirectReference.SetSubscriptCount(0);
        }
               
      //VALUES WORKS
        //definitely works
        public void SetValues(IList values)
        {
            ValueList list = TrueValueList.CreateValueList(linkToConn, values);
            DirectReference.Set(list);
            list.Close();
        }

        public void SetValuesTyped(IList values, IList<ValueMeta> valuesMeta)
        {
            ValueList list = TrueValueList.CreateValueList(linkToConn, values, valuesMeta);
            DirectReference.Set(list);
            list.Close();
        }

        public void SetValues(ArrayList subscripts, IList values)
        {
            SetSubscripts(DirectReference, subscripts);
            this.subscripts = new ArrayList(subscripts.ToArray());//MUST BE OPTIMIZED
            ValueList list = TrueValueList.CreateValueList(linkToConn, values);
            DirectReference.Set(list);
            list.Close();
            //DirectReference.Set(list, subscripts.ToArray()); 
                //oh intersystems; after kill subsc count != 0 
                //NodeRefObl.Set() - appends subscripts instead(vmesto) reseting
        }

        public void SetValues(object[] subscripts, IList values)
        {
            ValueList list = TrueValueList.CreateValueList(linkToConn, values);
            DirectReference.Set(list, subscripts);
            list.Close();
        }

        public void SetValues(ValueList values)
        {
            DirectReference.Set(values);
        }

        public void SetValues(TrueValueList trueList)
        {
            throw new NotImplementedException();
        }

        public ArrayList GetValues(IList<ValueMeta> meta, bool returnTypedLists = false)
        {
            ArrayList values = null;
            if (HasValues())
            {
                values = TrueValueList.GetValues(DirectReference.GetList(), meta, returnTypedLists);
            }
            return values; 
        }

        public ArrayList GetValues(ValueMeta meta, bool returnTypedLists = false)
        {
            ArrayList values = null;
            if (HasValues())
            {
                values = TrueValueList.GetValues(DirectReference.GetList(), meta, returnTypedLists);
            }
            return values;
        }

        public void SetAtomValue(object value)
        {
            TrueNodeReference.SetAtomValue(DirectReference, value);
        }

        public object GetAtomValue(ValueMeta valueMeta)
        {
            return TrueNodeReference.GetAtomValue(DirectReference, valueMeta);
        }

        //indefinitely works
        public ArrayList TryGetValues()
        {
            return TryGetValues(this.DirectReference);
        }

      //Standart NodeReference method calls:
        public string GetName() 
        {
            return DirectReference.GetName();
        }

        public void AppendSubscript(object subscript)
        {
            DirectReference.AppendSubscript("");
            SetSubscript(DirectReference,DirectReference.GetSubscriptCount(), subscript);
            subscripts.Add(subscript);
        }

        public string NextSubscript()
        {
            return DirectReference.NextSubscript();
        }

        public void GoNextSubscript()
        {
            DirectReference.SetSubscript(SubsCount, NextSubscript());
            subscripts = TryGetTypedSubscripts(DirectReference);
        }

        public bool HasSubnodes()
        {
            return DirectReference.HasSubnodes();
        }

        public bool HasSubnode(object subnode)
        {
            return TrueNodeReference.HasSubnode(DirectReference, subnode);
        }

        public bool HasValues()
        {
            bool hasData = false;
            try
            {
                //hasData = DirectReference.GetList();
                hasData = DirectReference.HasData();
                return hasData;
            }
            catch
            {
                return hasData;
            }
        }

        public bool HasValues(ArrayList subscripts)
        {
            bool hasData = false;
            SetSubscripts(subscripts);
            try
            {
                //hasData = DirectReference.GetList();
                hasData = DirectReference.HasData();
                return hasData;
            }
            catch
            {
                return hasData;
            }
        }

        public void GoParentNodeSubscripts()
        {
            if (subscripts.Count > 0)
            {
                DirectReference.SetSubscriptCount(subscripts.Count - 1);
                subscripts.RemoveAt(subscripts.Count - 1);
            }

        }

        public void Kill()
        {
            DirectReference.Kill();
        }

        public void KillSubNodes()
        {
            //Kill();
            killSubnodes();
        }

        private void killSubnodes()
        {
            DirectReference.AppendSubscript("");
            subscripts.Add("");
            while (DirectReference.NextSubscript() != "")
            {
                GoNextSubscript();
                if (DirectReference.HasSubnodes())
                {
                    killSubnodes();
                    GoParentNodeSubscripts();
                }
                Kill();
            }
        }

     //Base work methods
        public static object[] GetNodeReferenceSubscripts(NodeReference reference)
        {
            object[] subs = new object[reference.GetSubscriptCount()];
            for (int i = 1; i <= subs.Length; i++)
            {
                subs[i - 1] = reference.GetObjectSubscript(i);
            }
            return subs;
        }
        
        /// <summary>
        /// Method gets subscripts witn !SUPPOSED! types 
        /// </summary>
        /// <param name="nodeRef">Node Reference</param>
        /// <returns>IList of subscripts with !SUPPOSED! types</returns>
        public static ArrayList TryGetTypedSubscripts(NodeReference nodeRef)
        {
            int count = nodeRef.GetSubscriptCount();
            ArrayList subs = new ArrayList();
            String str;
            int i;
            double d;
            for (int c = 1; c <= count; c++)
            {
                str = nodeRef.GetObjectSubscript(c).ToString();
                if ((str.Contains('.') || str.Contains(',')))
                {
                    if (str.Contains('.'))
                        CacheDouble.TryDouble(str, out d, ".");
                    else
                        CacheDouble.TryDouble(str, out d, ",");
                    subs.Add(d);
                }
                else if (int.TryParse(str, out i))
                {
                    subs.Add(i);
                }
                else
                {
                    subs.Add(str);
                }
            }
            return subs;
        }
        
        /// <summary>
        /// Method gets values witn !SUPPOSED! types 
        /// </summary>
        /// <param name="TrueNodeReference">Node Reference</param>
        /// <returns>IList of values with !SUPPOSED! types</returns>
        public static ArrayList TryGetValues(NodeReference reference)
        {
            ArrayList values = new ArrayList();
            if (reference.HasData())
            {
                ValueList valueList;
                try
                {
                    try//if node contains ValueList
                    {
                        valueList = reference.GetList();
                        return TrueValueList.TryGetValues(valueList);
                    }
                    catch
                    {
                        values.Capacity = 1;
                    }
                    //if single value
                    values.Add(reference.GetObject());
                    return values;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return values;
        }

        //Try set subscripts
        public static void SetSubscripts(NodeReference nodeRef, ArrayList subscriptsTyped)
        {
            int c = 0;
            try
            {
                for (; c < subscriptsTyped.Count; c++)
                {
                    SetSubscript(nodeRef, c + 1, subscriptsTyped[c]);
                }
            }
            catch (SubscriptTypeException stex)
            {
                throw stex;
            }
            finally
            {
                nodeRef.SetSubscriptCount(c);
            }
        }
        public static void SetSubscript(NodeReference reference, int position, object subscript)
        {
            Type type = subscript.GetType();
            if (type.Equals(typeof(double)))
            {
                reference.SetSubscript(position, CacheDouble.DoubleToStringSubscriptKey((double)subscript));
                return;
            }
            if (type.Equals(typeof(int)))
            {
                reference.SetSubscript(position, (int)subscript);
                return;
            }
            if (type.Equals(typeof(string)))
            {
                reference.SetSubscript(position, subscript.ToString());
                return;
            }
            if (type.Equals(typeof(long)))
            {
                reference.SetSubscript(position, (long)subscript);
                return;
            }
            throw new SubscriptTypeException(position, subscript);
        }

        //Set defined subscripts
        public static void SetSubscripts(NodeReference nodeRef, ArrayList subscriptsTyped, List<ValueMeta> subscriptsMeta)
        {
            int c = 0;
            try
            {
                for (; c < subscriptsTyped.Count; c++)
                {
                    SetSubscript(nodeRef, c + 1, subscriptsTyped[c], subscriptsMeta[c]);
                }
            }
            catch (SubscriptTypeException stex)
            {
                throw stex;
            }
            finally
            {
                nodeRef.SetSubscriptCount(c);
            }
        }
        public static void SetSubscript(NodeReference reference, int position, object subscript, ValueMeta subscriptMeta)
        {
            switch (subscriptMeta.ExtremeType)
            {
                case ExtremeTypes.EXTREME_DOUBLE:
                    {
                        reference.SetSubscript(position, CacheDouble.DoubleToStringSubscriptKey((double)subscript));
                        return;
                    }
                case ExtremeTypes.EXTREME_INT:
                    {
                        reference.SetSubscript(position, (int)subscript);
                        return;
                    }
                case ExtremeTypes.EXTREME_STRING:
                    {
                        reference.SetSubscript(position, subscript.ToString());
                        return;
                    }
                /*case ExtremeTypes.EXTREME_STRUCT:
                    {
                        TrueNodeReference.AppendSubscripts(reference,(IList)subscript,((StructValMeta)subscriptMeta).structDefinition.elementsMeta);
                        return;
                    }*/
                /*case ExtremeTypes.EXTREME_INT:
                    {
                        reference.SetSubscript(position, (long)subscript);
                        return;
                    }*/
            }
            throw new SubscriptTypeException(position, subscript);
        }

        //Append defined subscripts
        public static void AppendSubscripts(NodeReference reference, IList subscripts, IList<ValueMeta> subscriptsMeta)
        {
            for (int i = 0; i < subscriptsMeta.Count; i++)
            {
                TrueNodeReference.AppendSubscript(reference, subscripts[i], subscriptsMeta[i]);
            }
        }
        public static void AppendSubscript(NodeReference reference, object subscript, ValueMeta subscriptMeta)
        {
            switch (subscriptMeta.ExtremeType)
            {
                case ExtremeTypes.EXTREME_DOUBLE:
                    {
                        reference.AppendSubscript(CacheDouble.DoubleToStringSubscriptKey((double)subscript));
                        return;
                    }
                case ExtremeTypes.EXTREME_INT:
                    {
                        reference.AppendSubscript((int)subscript);
                        return;
                    }
                case ExtremeTypes.EXTREME_STRING:
                    {
                        reference.AppendSubscript(subscript.ToString());
                        return;
                    }
                case ExtremeTypes.EXTREME_STRUCT:
                    {
                        TrueNodeReference.AppendSubscripts(reference
                            , (IList)subscript, ((StructValMeta)subscriptMeta).structDefinition.elementsMeta);
                        return;
                    }
                /*case ExtremeTypes.EXTREME_INT:
                    {
                        reference.SetSubscript(position, (long)subscript);
                        return;
                    }*/
            }
        }

        //where is i use it?
        public static bool HasSubnode(NodeReference reference, object subnode)
        {
            int previousCount = reference.GetSubscriptCount();
            reference.AppendSubscript("");
            string nextSub;
            do
            {
                nextSub = reference.NextSubscript();
                if (nextSub != "")
                {
                    if (nextSub == subnode.ToString())
                    {
                        return true;
                    }
                    reference.SetSubscript(previousCount + 1, nextSub);
                }
            } while (nextSub != "");
            reference.SetSubscriptCount(previousCount);
            return false;
        }

        //what the fuck?!?!?!?!
        public static void SetAtomValue(NodeReference reference, object value)
        {
            if (value.GetType().Equals(typeof(double)))
            {
                reference.Set((double)value);
                return;
            }
            if (value.GetType().Equals(typeof(int)))
            {
                reference.Set((int)value);
                return;
            }
            if (value.GetType().Equals(typeof(string)))
            {
                reference.Set((string)value);
                return;
            }
            if (value.GetType().Equals(typeof(byte[])))
            {
                reference.Set((byte[])value);
                return;
            }
            if (value.GetType().Equals(typeof(ValueList)))
            {
                reference.Set((ValueList)value);
            }
        }

        public static object GetAtomValue(NodeReference reference, ValueMeta valueMeta)
        {
            switch (valueMeta.ExtremeType){
                case ExtremeTypes.EXTREME_DOUBLE:
                {
                    return reference.GetDouble();
                }
                    case ExtremeTypes.EXTREME_INT:
                {
                    return reference.GetInt();
                }
                    case ExtremeTypes.EXTREME_STRING:
                {
                    return reference.GetString();
                }
                    case ExtremeTypes.EXTREME_BYTES:
                {
                    return reference.GetBytes();
                }
                    case ExtremeTypes.EXTREME_LIST:
                {
                    return reference.GetList();
                }
            }
            return null;
        }
    }


    public static class CacheDouble
    {
        private static CultureInfo appCulture;
        private static char appDoubleSeparator;
        //
        private static CultureInfo cacheCulture;
        private static NumberStyles cacheNumberStyle;
        private static NumberFormatInfo cacheNumberFormatInfo;
        //
        static CacheDouble()
        {
            appCulture = CultureInfo.CurrentCulture;
            appDoubleSeparator = appCulture.NumberFormat.NumberDecimalSeparator[0];
            //
            cacheCulture = CultureInfo.CreateSpecificCulture("en-GB");
            cacheNumberStyle = NumberStyles.Number;
            cacheNumberFormatInfo = new NumberFormatInfo();
            cacheNumberFormatInfo.NumberDecimalSeparator = ".";
            cacheCulture.NumberFormat = cacheNumberFormatInfo;
        }
        public static string DoubleToStringSubscriptKey(double value)
        {
            //gets string value of double with curentCultureInfo
            string dstring = value.ToString();
            //replace curent decimalDelimiter to '.' to provide cache capability
            dstring = dstring.Replace(appDoubleSeparator, '.');
            //cache cannot set zero double value so s
            if (dstring.StartsWith("0"))
            {
                if (value == 0)
                {
                    return dstring;
                }
                return dstring.Substring(1);
            }
            return dstring;
        }

        public static bool TryDouble(string str, out double d, string decimalDelimiter = ".")
        {
            return Double.TryParse(str, cacheNumberStyle, cacheCulture, out d);
        }
    }


    public class TNRException:Exception
    {
        public ArrayList subscripts;
        public TNRException(string message) : base(message) { }
        public TNRException(string message, ArrayList subscripts) : base(message) 
        { 
            this.subscripts = new ArrayList(subscripts);
        }
    }


    public class SubscriptTypeException : Exception
    {
        public object founded;
        public int OnSubscriptIndex;
        public SubscriptTypeException()
            :base("Subscripts must be the follow types: string, double, integer;")
        {
        }
        public SubscriptTypeException(int onSubscriptIndex, object founded)
            : base("Subscripts must be the follow types: string, double, integer; founded: "+founded.GetType().ToString())
        {
            this.OnSubscriptIndex = onSubscriptIndex;
            this.founded = founded;
        }
    }

}
