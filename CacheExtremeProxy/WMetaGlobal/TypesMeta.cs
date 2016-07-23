using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CacheEXTREME2.WMetaGlobal
{
    public enum ExtremeTypes {
        EXTREME_STRING = 0,
        EXTREME_INT = 1,
        EXTREME_DOUBLE = 2,
        EXTREME_BYTES = 3,
        EXTREME_LIST = 13,
        EXTREME_STRUCT = 666
    };

    public interface IValidator
    {
        bool Validate(object value);
    }

    public interface IKeyValidator
    {
        bool ValidateKey(object keyValue);
    }


    public class ValueMeta : IValidator
    {
        public ExtremeTypes ExtremeType { get; private set; }
        protected string semanticName;
        public string SemanticName { get { return semanticName; } }
        public virtual bool Validate(object value) { return false; }
        public virtual object GetDefaultValue() { return null; }
        public virtual string GetCSharpTypeName() { return null; }
        public virtual void SetRestrictionsMeta(ValueMeta from) { }
        public virtual ArrayList Serialize(){ return new ArrayList{SemanticName,GetCSharpTypeName()};}
        //
        public ValueMeta(string caption, ExtremeTypes extremeType)
        {
            this.semanticName = caption;
            this.ExtremeType = extremeType;
        }
    }


    public class DoubleValMeta : ValueMeta, IKeyValidator
    {
        private double minimum;
        public double Minimum { get { return minimum; } }
        private double maximum;
        public double Maximum { get { return maximum; } }
        private double?[] _default;
        //
        public DoubleValMeta(ArrayList meta)
            : base(meta[0].ToString(), ExtremeTypes.EXTREME_DOUBLE)
        {
            _default = new double?[1];
            //
            //$lb([0]"key/param name", [1]"type", [2]"minval", [3]"maxval", [4]"default")
            minimum = (meta[2].ToString().Trim().Equals("")) ? 0 : double.Parse(meta[2].ToString());
            maximum = (meta[3].ToString().Trim().Equals("")) ? 0xFFFF : double.Parse(meta[3].ToString());
            try
            {
                _default[0] = (meta[4].ToString().Equals("")) ? 0 : double.Parse(meta[4].ToString());
            }
            catch
            {
                meta[4] = meta[4].ToString().Replace(".", ",");
                _default[0] = (meta[4].ToString().Trim().Equals("")) ? 0 : double.Parse(meta[4].ToString());
            }
        }
        public DoubleValMeta(string name, ArrayList typePermissions)
            : base(name, ExtremeTypes.EXTREME_DOUBLE)
        {
            _default = new double?[1];
            //
            //$lb([1]"type", [2]"minval", [3]"maxval", [4]"default")
            minimum = double.Parse(typePermissions[1].ToString());
            maximum = double.Parse(typePermissions[2].ToString());
            _default[0] = double.Parse(typePermissions[3].ToString());
        }
        public DoubleValMeta()
            : base("", ExtremeTypes.EXTREME_DOUBLE)
        {
        }
        public DoubleValMeta(string name)
            :this(name, new ArrayList(){"",0,0,0})
        {
        }
        //
        public override void SetRestrictionsMeta(ValueMeta copyFrom)
        {
            DoubleValMeta sample = copyFrom as DoubleValMeta;
            if (sample != null)
            {
                this.semanticName = sample.SemanticName;
                this.minimum = sample.minimum;
                this.maximum = sample.maximum;
                this._default = sample._default;
                return;
            }
            throw new ArrayTypeMismatchException("parameter type 'copyFrom' in '"+
                sample.GetType().Name + ".CopyFrom'  must be a '" + sample.GetType().Name + "')");
        }
        //
        public override bool Validate(object value)
        {
            return validate(value);
        }
        public bool ValidateKey(object keyValue)
        {
            return validate(keyValue);
        }
        private bool validate(object value)
        {
            if (!typeof(System.Double).Equals(value.GetType()))
            {
                throw new ArgumentException("Must be double: ",SemanticName);
            }
            if ((double)value > maximum || (double)value < minimum)
            {
                throw new ArgumentOutOfRangeException(SemanticName, value, " must be in [" + minimum + " ; " + maximum + "]");
            }
            return true;
        }
        //
        public override object GetDefaultValue()
        {
            return _default[0];
        }
        public override string ToString()
        {
            return "name: " + base.SemanticName + "(" + _default[0].GetType().Name + ")"
                + ", min: " + minimum + ", max: " + maximum + ", def: " + _default[0];
        }
        //
        public override string GetCSharpTypeName()
        {
            return typeof(System.Double).Name;
        }
        //
        public override ArrayList Serialize()
        {
            ArrayList ar = new ArrayList(5);
            ar.Add(base.SemanticName);
            //ar.Add(_default[0].GetType().Name.ToLower());
            ar.Add("double");
            ar.Add(minimum);
            ar.Add(maximum);
            ar.Add(GetDefaultValue());
            return ar;
        }
    }


    public class IntValMeta : ValueMeta, IKeyValidator
    {
        public const ExtremeTypes extremeType = ExtremeTypes.EXTREME_DOUBLE;
        private int minimum;
        public int Minimum { get { return minimum; } }
        private int maximum;
        public int Maximum { get { return maximum; } }
        private int?[] _default;
        //
        public IntValMeta(ArrayList meta)
            : base(meta[0].ToString(), ExtremeTypes.EXTREME_INT)
        {
            _default = new int?[1];
            //
            //$lb([0]"key/param name", [1]"type", [2]"minval", [3]"maxval", [4]"default")
            minimum = (meta[2].ToString().Trim().Equals("")) ? 0 : (int.Parse(meta[2].ToString()));
            maximum = (meta[3].ToString().Trim().Equals("")) ? 0xFFFF : (int.Parse(meta[3].ToString()));
            _default[0] = (meta[4].ToString().Trim().Equals("")) ? 0 : (int.Parse(meta[4].ToString()));
        }
        public IntValMeta(string name, ArrayList typePermissions)
            : base(name, ExtremeTypes.EXTREME_INT)
        {
            _default = new int?[1];
            //
            //$lb([1]"type", [2]"minval", [3]"maxval", [4]"default")
            minimum = int.Parse(typePermissions[1].ToString());
            maximum = int.Parse(typePermissions[2].ToString());
            _default[0] = int.Parse(typePermissions[3].ToString());
        }
        public IntValMeta()
            : base("", ExtremeTypes.EXTREME_INT)
        {
        }
        public IntValMeta(string name)
            : this(name, new ArrayList() { "", 0, 0, 0 })
        {
        }
        //
        public override void SetRestrictionsMeta(ValueMeta copyFrom)
        {
            IntValMeta sample = copyFrom as IntValMeta;
            if (sample!=null)
            {
                this.semanticName = sample.SemanticName;
                this.minimum = sample.minimum;
                this.maximum = sample.maximum;
                this._default = sample._default;
                return;
            }
            throw new ArrayTypeMismatchException("parameter type 'copyFrom' in '" +
                sample.GetType().Name + ".CopyFrom'  must be a '" + sample.GetType().Name + "')");
        }
        //
        public override bool Validate(object value)
        {
            return validate(value);
        }
        public bool ValidateKey(object keyValue)
        {
            return validate(keyValue);
        }
        private bool validate(object value)
        {
            if (!typeof(int).Equals(value.GetType()))
            {
                throw new ArgumentException("expected Integer value of: ", SemanticName);
            }
            if ((int)value > maximum || (int)value < minimum)
            {
                throw new ArgumentOutOfRangeException(this.SemanticName,value,"must be in ["+minimum+";"+maximum+"]");
            }
            return true;
        }
        //
        public override object GetDefaultValue()
        {
            return _default[0];
        }
        public override string ToString()
        {
            return "name: " + base.SemanticName + "(" + _default[0].GetType().Name + ")"
                + ", min: " + minimum + ", max: " + maximum + ", def: " + _default[0];
        }
        //
        public override string GetCSharpTypeName()
        {
            return typeof(int).Name;
        }
        //
        public override ArrayList Serialize()
        {
            ArrayList ar = new ArrayList(5);
            ar.Add(base.SemanticName);
            //ar.Add(_default[0].GetType().Name.ToLower());
            ar.Add("integer");
            ar.Add(minimum);
            ar.Add(maximum);
            ar.Add(GetDefaultValue());
            return ar;
        }
    }


    public class StringValMeta : ValueMeta, IKeyValidator
    {
        private int minlength;
        public int MinLenght { get { return minlength; } }
        private int maxlength;
        public int MaxLength { get { return maxlength; } }
        private string[] _default;
        //
        public StringValMeta(ArrayList meta)
            : base(meta[0].ToString(), ExtremeTypes.EXTREME_STRING)
        {
            _default = new string[1];
            //
            //$lb([0]"key/param name", [1]"type", [2]"minlen", [3]"maxlen", [4]"default")
            minlength = (meta[2].ToString().Trim().Equals("")) ? 0 : (int.Parse(meta[2].ToString()));
            maxlength = (meta[3].ToString().Trim().Equals("")) ? 0xff : (int.Parse(meta[3].ToString()));
            _default[0] = (meta[4] == null) ? "" : meta[4].ToString();
        }
        public StringValMeta(string name, ArrayList typePermissions)
            : base(name, ExtremeTypes.EXTREME_STRING)
        {
            _default = new string[1];
            //
            //$lb([1]"type", [2]"minlen", [3]"maxlen", [4]"default")
            minlength = int.Parse(typePermissions[1].ToString());
            maxlength = int.Parse(typePermissions[2].ToString());
            _default[0] = typePermissions[3].ToString();
        }
        public StringValMeta()
            : base("", ExtremeTypes.EXTREME_STRING)
        {
        }
        public StringValMeta(string name)
            : this(name, new ArrayList() { "", 0, 0, "" })
        {
        }
        //
        public override void SetRestrictionsMeta(ValueMeta copyFrom)
        {
            StringValMeta sample = copyFrom as StringValMeta;
            if (sample != null)
            {
                this.semanticName = sample.SemanticName;
                this.minlength = sample.minlength;
                this.maxlength = sample.maxlength;
                this._default = sample._default;
                return;
            }
            throw new ArrayTypeMismatchException("parameter type 'copyFrom' in '" +
                sample.GetType().Name + ".CopyFrom'  must be a '" + sample.GetType().Name + "')");
        }
        //
        public override bool Validate(object value)
        {
            return validate(value);
        }
        public bool ValidateKey(object keyValue)
        {
            return validate(keyValue);
        }
        private bool validate(object keyValue)
        {
            if (!typeof(string).Equals(keyValue.GetType()))
            {
                throw new ArgumentException("expected string value: ", SemanticName);
            }
            string key = keyValue as string;
            if (key.Length > maxlength || key.Length < minlength)
            {
                throw new ArgumentException("string length must be in [" + minlength + " ; " + maxlength + "]",SemanticName);
            }
            return true;
        }
        //
        public override object GetDefaultValue()
        {
            return _default[0];
        }
        public override string ToString()
        {
            return "name: " + base.SemanticName + "(" + _default[0].GetType().Name + ")" 
                + ", min: " + minlength + ", max: " + maxlength + ", def: " + _default[0];
        }
        //
        public override string GetCSharpTypeName()
        {
            return typeof(string).Name;
        }
        //
        public override ArrayList Serialize()
        {
            ArrayList ar = new ArrayList(5);
            ar.Add(base.SemanticName);
            //ar.Add(_default[0].GetType().Name.ToLower());
            ar.Add("string");
            ar.Add(minlength);
            ar.Add(maxlength);
            ar.Add(GetDefaultValue());
            return ar;
        }
    }


    public class BytesValMeta : ValueMeta
    {
        private int maxSize;
        public int MaxSize { get { return maxSize; } }
        //
        public BytesValMeta(ArrayList meta)
            : base(meta[0].ToString(), ExtremeTypes.EXTREME_BYTES)
        {
            //
            //$lb([0]"key/param name", [1]"type", [2]"maxlen")
            maxSize = (meta[2].ToString().Trim().Equals("")) ? 0xFFFF : (int.Parse(meta[2].ToString()));
        }
        public BytesValMeta(string name, ArrayList typePermissions)
            : base(name, ExtremeTypes.EXTREME_BYTES)
        {
            //
            //$lb([1]"type", [2]"maxlen")
            maxSize = typePermissions[1].ToString().Trim().Equals("") ? 0xFFFF : (int.Parse(typePermissions[1].ToString()));
        }
        public BytesValMeta()
            : base("", ExtremeTypes.EXTREME_BYTES)
        {
        }
        public BytesValMeta(string name)
            : this(name, new ArrayList() { "", 2 })
        {
        }
        //
        public override void SetRestrictionsMeta(ValueMeta copyFrom)
        {
            BytesValMeta sample = copyFrom as BytesValMeta;
            if (sample != null)
            {
                this.semanticName = sample.SemanticName;
                this.maxSize = sample.maxSize;
                return;
            }
            throw new ArrayTypeMismatchException("parameter type 'copyFrom' in '" +
                sample.GetType().Name + ".CopyFrom'  must be a '" + sample.GetType().Name + "')");
        }
        //
        public override bool Validate(object value)
        {
            if(!value.GetType().Equals(typeof(byte[])))
            {
                throw new ArgumentException("expected byte[]",SemanticName);
            }
            if ((value as byte[]).Length > maxSize)
            {
                throw new ArgumentException("too many bytes, must be less then "+ maxSize+";",SemanticName);
            }
            return true;
        }
        //
        public override object GetDefaultValue()
        {
            return base.GetDefaultValue();
        }
        public override string ToString()
        {
            return "name: " + base.SemanticName + "(byte[])"
                + ", max: " + maxSize;
        }
        //
        public override string GetCSharpTypeName()
        {
            return typeof(byte[]).Name;
        }
        //
        public override ArrayList Serialize()
        {
            return new ArrayList { base.SemanticName, "bytes", 0, maxSize, GetDefaultValue() };
        }
    }


    public class ListValMeta : ValueMeta
    {
        private ValueMeta elemMeta;
        public ValueMeta ElemMeta { get { return elemMeta; } }
        private int minlengh;
        private int maxlength;
        //
        public ListValMeta(ArrayList fullListMeta)
            : base(fullListMeta[0].ToString(), ExtremeTypes.EXTREME_LIST)
        {
            //=$lb("SemanticName","list",<min>,<max><<elemMeta>>
            minlengh = (fullListMeta[2].ToString().Trim().Equals("")) ? 0 : (int.Parse(fullListMeta[2].ToString()));
            maxlength = (fullListMeta[3].ToString().Trim().Equals("")) ? 0xFF : (int.Parse(fullListMeta[3].ToString()));
            ArrayList typePermission = fullListMeta.GetRange(4, fullListMeta.Count - 4);
            elemMeta = getElemMeta(typePermission);
        }

        public ListValMeta(ArrayList fullListMeta, ValueMeta elemMeta)
            : base(fullListMeta[0].ToString(), ExtremeTypes.EXTREME_LIST)
        {

            //$lb([0]"param name", [1]"list", [2]"minlen", [3]"maxlen", <type def>)
            minlengh = (fullListMeta[2].ToString().Trim().Equals("")) ? 0 : (int.Parse(fullListMeta[2].ToString()));
            maxlength = (fullListMeta[3].ToString().Trim().Equals("")) ? 0xFF : (int.Parse(fullListMeta[3].ToString()));
            this.elemMeta = elemMeta;
        }

        public ListValMeta(string name, ArrayList listPermissions, ValueMeta elemMeta)
            : base(name, ExtremeTypes.EXTREME_LIST)
        {

            //$lb([1]"list", [2]"minlen", [3]"maxlen", <type def>)
            minlengh = (listPermissions[1].ToString().Trim().Equals("")) ? 0 : (int.Parse(listPermissions[1].ToString()));
            maxlength = (listPermissions[2].ToString().Trim().Equals("")) ? 0xFF : (int.Parse(listPermissions[2].ToString()));
            this.elemMeta = elemMeta;
        }

        public ListValMeta(string name, ArrayList listDefinition)
            : base(name, ExtremeTypes.EXTREME_LIST)
        {

            //$lb([1]"list", [2]"minlen", [3]"maxlen", <type def>)
            minlengh = int.Parse(listDefinition[1].ToString());
            maxlength = int.Parse(listDefinition[2].ToString());
            ArrayList typePermissions = listDefinition.GetRange(3, listDefinition.Count - 3);
            elemMeta = getElemMeta(typePermissions);
            
        }

        public ListValMeta(ValueMeta elemMeta)
            : this()
        {
            this.elemMeta = elemMeta;
        }

        public ListValMeta()
            : base("", ExtremeTypes.EXTREME_LIST)
        {
        }
        //
        public override void SetRestrictionsMeta(ValueMeta copyFrom)
        {
            ListValMeta sample = copyFrom as ListValMeta;
            if (sample != null)
            {
                this.semanticName = sample.SemanticName;
                this.minlengh = sample.minlengh;
                this.maxlength = sample.maxlength;
                this.elemMeta.SetRestrictionsMeta(sample.elemMeta);
                return;
            }
            throw new ArrayTypeMismatchException("parameter type 'copyFrom' in '" +
                sample.GetType().Name + ".CopyFrom'  must be a '" + sample.GetType().Name + "')");
        }
        //
        private ValueMeta getElemMeta(ArrayList typeDefinition)
        {
            switch (typeDefinition[0].ToString())
            {
                case "string":
                {
                    return new StringValMeta(this.SemanticName + "Elem", typeDefinition);
                }
                case "integer":
                {
                    return new IntValMeta(this.SemanticName +"Elem", typeDefinition);
                }
                case "double":
                {
                    return new DoubleValMeta(this.SemanticName +"Elem", typeDefinition);
                }
                case "bytes":
                {
                    return new BytesValMeta(this.SemanticName + "Elem", typeDefinition);
                }
                case "list":
                {
                    return new ListValMeta(this.SemanticName + "Elem", typeDefinition);
                }
                case "struct":
                {
                    return new StructValMeta(typeDefinition[1].ToString());
                }
                case "Int32":
                {
                    return new IntValMeta(this.SemanticName + "Elem", typeDefinition);
                }
                case "byte[]":
                {
                    return new BytesValMeta(this.SemanticName + "Elem", typeDefinition);
                }
            }
            return null;
        }
        //
        public override bool Validate(object value)
        {
            if (!(value is IList))
            {
                throw new ArgumentException("expected IList value;", SemanticName);
            }
            IList values = value as IList;
            if (values.Count > maxlength)
            {
                throw new ArgumentException("error maxCount in list " + SemanticName + ", recieved: " + values.Count + ";",SemanticName);
            }
            for (int i = 0; i < values.Count; i++)
            {
                try
                {
                    elemMeta.Validate(values[i]);
                }
                catch(Exception ex)
                {
                    throw new ArgumentException("In " + this.semanticName + ":\n" + ex.Message, "<" + SemanticName + "_List>", ex);
                }
            }
            return true;
        }
        //
        public override object GetDefaultValue()
        {
            return new ArrayList();
        }
        public override string ToString()
        {
            return "name: " + base.SemanticName + "(List<"+elemMeta.ToString()+">)"
                + ", min: " + minlengh + ", max: " + maxlength;
        }
        //
        public override string GetCSharpTypeName()
        {
            return "List<" + this.elemMeta.GetCSharpTypeName() + ">";
        }
        //
        public override ArrayList Serialize()
        {
            ArrayList al = new ArrayList { base.SemanticName, "list", minlengh, maxlength };
            //if (ElemMeta.ExtremeType == ExtremeTypes.EXTREME_LIST)
            //{
            //    al.AddRange(new ArrayList(ElemMeta.().GetRange(1, ElemMeta.ToArrayList().Count - 1)));
            //}
            al.AddRange(new ArrayList(ElemMeta.Serialize().GetRange(1, ElemMeta.Serialize().Count - 1)));
            return al;
        }
    }


    public class StructValMeta : ValueMeta, /*TO THE NEW VERSION WITH STRUCTED KEY*/IKeyValidator
    {
        public int StructId;
        public string StructTypeName;
        public List<ValueMeta> elementsMeta;
        //
        public StructValMeta(string structName,string structTypeName, List<ValueMeta> elementsMeta)
            : base(structName, ExtremeTypes.EXTREME_STRUCT)
        {
            this.StructTypeName = structTypeName;
            this.elementsMeta = new List<ValueMeta>(elementsMeta);
        }

        public StructValMeta(string structTypeName, List<ValueMeta> elementsMeta)
            :this("",structTypeName,elementsMeta)
        {
        }

        public StructValMeta(string semanticName, string structTypeName)
            : base(semanticName, ExtremeTypes.EXTREME_STRUCT)
        {
            this.StructTypeName = structTypeName;
            this.elementsMeta = new List<ValueMeta>();
        }

        public StructValMeta(string structTypeName)
            :this("",structTypeName)
        {
        }

        public StructValMeta()
            : this("")
        {
        }

        public StructValMeta(string semanticName, StructValMeta structMeta)
            :this(semanticName,structMeta.StructTypeName,structMeta.elementsMeta)
        {
            this.StructId = structMeta.StructId;
        }
        //
        public override void SetRestrictionsMeta(ValueMeta copyFrom)
        {
            StructValMeta sample = copyFrom as StructValMeta;
            if (sample != null)
            {
                this.semanticName = sample.SemanticName;
                this.StructTypeName = sample.StructTypeName;
                string curentSemantic = "";
                try{
                    for (int i = 0; i < this.elementsMeta.Count; i++)
                    {
                        curentSemantic = elementsMeta[i].SemanticName;
                        elementsMeta[i].SetRestrictionsMeta(sample.elementsMeta[i]);
                    }
                }
                catch (Exception ex){
                    new ArgumentException("Missmathc attributes structure in structs: this "
                        + this.StructTypeName + "." + curentSemantic 
                        + ", sample " + sample.StructTypeName + ".???\n"+ex.Message, ex);
                }
                return;
            }
            throw new ArrayTypeMismatchException("parameter type 'copyFrom' in '" +
                sample.GetType().Name + ".CopyFrom'  must be a '" + sample.GetType().Name + "')");
        }
        //
        public override bool Validate(object value)
        {
            if (!(value is object))
            {
                throw new ArgumentException("expected Struct value;", SemanticName);
            }
            Type t = value.GetType();
            foreach (ValueMeta elementMeta in elementsMeta)
            {
                try
                {
                    elementMeta.Validate(t.GetField(elementMeta.SemanticName).GetValue(value));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("in " + this.semanticName + ":\n" + ex.Message, ex);
                }
            }
            return true;
        }
        //
        public override string ToString()
        {
            return "name: " + base.SemanticName + "("+this.StructTypeName+")"
                + "=> struct Id "+StructId+ ";";
        }
        //
        public override string GetCSharpTypeName()
        {
            return StructTypeName;
        }
        //
        public override ArrayList Serialize()
        {
            return new ArrayList { base.SemanticName, "struct", StructTypeName };
        }

        //TO THE NEW VERSION WITH STRUCTED KEYS
        public bool ValidateKey(object keyValue)
        {
            return Validate(keyValue);
        }
    }
}