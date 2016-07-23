using InterSystems.Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using CacheEXTREME2.WMetaGlobal;

namespace CacheEXTREME2.WDirectGlobal
{

    public class TrueValueList
    {
        private Connection linkToConn;

        public ValueList valueList;
        //
        public TrueValueList(Connection conn)
        {
            this.linkToConn = conn;
            valueList = this.linkToConn.CreateList();
        }

        //
        public void SetValues(ArrayList values)
        {
            valueList = CreateValueList(linkToConn, values);
        }

        public void SetValues(TrueValueList list, List<ValueMeta> valuesMeta)
        {
            this.valueList = CreateValueList(linkToConn, list.GetValues(valuesMeta));
        }

        public void TrySetValues(TrueValueList list)
        {
            this.valueList = CreateValueList(linkToConn, TrueValueList.TryGetValues(list.valueList));
        }

        public static ValueList CreateValueList(Connection conn, IList values)
        {
            ValueList list = values.Count!=0?conn.CreateList(values.Count):conn.CreateList();
            for (int i = 0; i < values.Count; i++)
            {
                //Type valueType = values[i].GetType();
                //HACK must be replaced!!!
                //checking on GenericList<T>
                //Array implements ICloneable 
                //GenericList do not Imlements, but both is IList, so...
                bool isGenericList = (values[i] as IList) != null && (values[i] as ICloneable)==null;
                //if (valueType.Equals(typeof(ArrayList)) || isGenericList)
                //bool isIList = (values[i] as IList) != null;
                if (isGenericList || values[i].GetType().Equals(typeof(ArrayList)))
                {
                    //ArrayList iList = new ArrayList((values[i] as IList));
                    //ValueList valueList = CreateValueList(conn, iList);
                    ValueList valueList = CreateValueList(conn, values[i] as IList);
                    list.Append(valueList);
                    continue;
                }
                list.Append(values[i]);
            }
            return list;
        }

        public static ValueList CreateValueList(Connection conn, IList values, IList<ValueMeta> valuesMeta)
        {
            ValueList list = values.Count != 0 ? conn.CreateList(values.Count) : conn.CreateList();
            for (int i = 0; i < values.Count; i++)
            {
                switch(valuesMeta[i].ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                            ValueList valueList = CreateValueListTyped(conn, values[i] as IList, (valuesMeta[i] as ListValMeta).ElemMeta);
                            list.Append(valueList);
                            continue;
                    case ExtremeTypes.EXTREME_STRUCT:
                            ValueList valueListS = CreateValueList(conn, values[i] as IList, (valuesMeta[i] as StructValMeta).elementsMeta);
                            list.Append(valueListS);
                            continue;
                    default:
                        list.Append(values[i]);
                        continue;
                }
                throw new ArgumentException("Lists and primitives are available", "values");
            }
            return list;
        }

        public static ValueList CreateValueListTyped(Connection conn, IList values, ValueMeta valueMeta)
        {
            ValueList list = values.Count != 0 ? conn.CreateList(values.Count) : conn.CreateList();
            for (int i = 0; i < values.Count; i++)
            {
                switch (valueMeta.ExtremeType)
                {
                    case ExtremeTypes.EXTREME_LIST:
                        ValueList valueListT = CreateValueListTyped(conn, values[i] as IList, (valueMeta as ListValMeta).ElemMeta);
                        list.Append(valueListT);
                        continue;
                    case ExtremeTypes.EXTREME_STRUCT:
                        ValueList valueListS = CreateValueList(conn, values[i] as IList, (valueMeta as StructValMeta).elementsMeta);
                        list.Append(valueListS);
                        continue;
                    default:
                        list.Append(values[i]);
                        continue;
                }
                throw new ArgumentException("Lists and primitives are available", "values");
            }
            return list;
        }

        //
        public ArrayList GetValues(List<ValueMeta> meta)
        {
            return GetValues(valueList, meta);
        }

        public Dictionary<string, object> GetNamedValues(List<ValueMeta> valuesMeta)
        {
            Dictionary<string, object> namedValues = new Dictionary<string, object>(valuesMeta.Count);
            ArrayList values = GetValues(valueList, valuesMeta);
            for (int c = 0; c < values.Capacity; c++)
            {
                /*if (values is ArrayList)
                {
                    values.Add(GetValues((ValueList)obj, ((ListValMeta)valuesMeta[c]).ElemMeta));
                    continue;
                }*/
                namedValues.Add(valuesMeta[c].SemanticName, values[c]);
            }
            return namedValues;
        }

        public ArrayList GetValues(ValueMeta meta)
        {
            return GetValues(valueList, meta);
        }

        public static ArrayList GetValues(ValueList valueList, IList<ValueMeta> valuesMeta, bool returnTypedLists = false)
        {
            if (valuesMeta.Count > valueList.Length)
            {
                throw new UnexpectedNumberOfValuesException(valueList.Length, valuesMeta.Count);
            }
            ArrayList values = new ArrayList();
            values.Capacity = valuesMeta.Count;
            valueList.ResetToFirst();
            for (int c = 0; c < valuesMeta.Count; c++)
            {
                object obj = getNextValue(valueList, valuesMeta[c],returnTypedLists);
                values.Add(obj);
            }
            return values;
        }

        public static ArrayList GetValues(ValueList valueList, ValueMeta valueMeta, bool returnTypedLists = false)
        {
            ArrayList values = new ArrayList();
            values.Capacity = valueList.Length;
            valueList.ResetToFirst();
            for (int c = 0; c < values.Capacity; c++)
            {
                object obj = getNextValue(valueList, valueMeta,returnTypedLists);
                values.Add(obj);
            }
            return values;
        }

        private static object getNextValue(ValueList list, ValueMeta meta, bool returnTypedList = false)
        {
            switch (meta.ExtremeType)
            {
                case ExtremeTypes.EXTREME_DOUBLE:
                {
                    return list.GetNextDouble();
                }
                case ExtremeTypes.EXTREME_INT:
                {
                    return list.GetNextInt();
                }
                case ExtremeTypes.EXTREME_STRING:
                {
                    return list.GetNextString();
                }
                case ExtremeTypes.EXTREME_BYTES:
                {
                    return list.GetNextBytes();
                }
                case ExtremeTypes.EXTREME_LIST:
                {
                    return returnTypedList
                        ?GetTypedList(list.GetNextList(),meta as ListValMeta)
                        :GetValues(list.GetNextList(), (meta as ListValMeta).ElemMeta);
                }
                case ExtremeTypes.EXTREME_STRUCT:
                {
                    return GetValues(list.GetNextList(), (meta as StructValMeta).elementsMeta);
                }
            }
            throw new ValueListMetaException(list.GetNextObject());
        }

        public static IList GetTypedList(ValueList valueList, ListValMeta listMeta)
        {
            switch(listMeta.ElemMeta.ExtremeType){
                case ExtremeTypes.EXTREME_STRING:
                {
                    return GetTypedList<string>(valueList);
                }
                    case ExtremeTypes.EXTREME_INT:
                {
                    return GetTypedList<int>(valueList);
                }
                    case ExtremeTypes.EXTREME_DOUBLE:
                {
                    return GetTypedList<double>(valueList);
                }
                    case ExtremeTypes.EXTREME_BYTES:
                {
                    return GetTypedList<byte[]>(valueList);
                }
                    case ExtremeTypes.EXTREME_LIST:
                {
                    List<IList> toReturn = new List<IList>();
                    toReturn.Add(GetTypedList(valueList.GetNextList(), listMeta.ElemMeta as ListValMeta));
                    //return GetTypedList(valueList.GetNextList(), listMeta.ElemMeta as ListValMeta);
                    return toReturn;
                }
            }
            return null;
        }

        public static List<T> GetTypedList<T>(ValueList valueList)
        {
            List<T> values = new List<T>(valueList.Length);
            valueList.ResetToFirst();
            for (int c = 0; c < values.Capacity; c++)
            {
                object obj = valueList.GetNextObject();
                values.Add((T)obj);
            }
            return values;
        }

        //
        /// <summary>
        /// Method gets values witn !SUPPOSED! types 
        /// </summary>
        /// <param name="TrueNodeReference">Node Reference</param>
        /// <returns>IList of values with !SUPPOSED! types</returns>
        public static ArrayList TryGetValues(ValueList valueList)
        {
            ArrayList values = new ArrayList();
            values.Capacity = valueList.Length;
            valueList.ResetToFirst();
            for (int c = 0; c < values.Capacity; c++)
            {
                object obj = tryGetValueListValue(valueList, c + 1);
                if (obj != null)
                {
                    values.Add(TryGetValues((ValueList)obj)); continue;
                }
                else
                {
                    obj = valueList.GetNextObject();
                }
                if (obj == null)
                {
                    obj = "null";
                }
                values.Add(obj);
            }
            return values;
        }

        private static object tryGetValueListValue(ValueList list, int position)
        {
            object obj = null;
            try
            {
                obj = list.GetNextList();
                if (obj == null) { valueListResetAndSkip(list, position - 1); }
                return obj;
            }
            catch
            {
                valueListResetAndSkip(list, position - 1);
            }
            return obj;
        }

        private static void valueListResetAndSkip(ValueList list, int count)
        {
            list.ResetToFirst();
            list.SkipNext(count);
        }
    }


    class ValueListTypeException : Exception
    {
        private static string message 
            = "Values must be the follow types: ArrayList, byte[], string, double, integer; ";
        public object founded;
        public int OnSubscriptIndex;
        public ValueListTypeException()
            : base()
        {
        }
        public ValueListTypeException(int onValueIndex, object founded)
            : base(message+"founded: "+founded.GetType().ToString())
        {
            this.OnSubscriptIndex = onValueIndex;
            this.founded = founded;
        }
    }


    class ValueListMetaException : Exception
    {
        private static string messageDefault
            = "Unsuported type of value; ";
        private static string messageExtended
            = "Suported types is: ArrayList, byte[], string, double, integer; ";
        public object founded;
        public ValueListMetaException()
            : base(messageDefault)
        {
        }
        public ValueListMetaException(object founded)
            : base(messageExtended + " founded: "+ founded.GetType().ToString())
        {
            this.founded = founded;
        }
    }


    class UnexpectedNumberOfValuesException : Exception
    {
        public int inListCount;
        public int inMetaListCount;
        private static string messageDefault
            = "Mismatch between the number of values that are specified in the Meta and actual;";
        public UnexpectedNumberOfValuesException(int inList, int inMeta)
            : base(messageDefault + " expected: " + inMeta + ", recieved: " + inList + ";")
        {
            inListCount = inList;
            inMetaListCount = inMeta;
        }
    }

}
