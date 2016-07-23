using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CacheEXTREME2.WMetaGlobal;
using System.Collections;
using System.IO;

namespace MetaCache_v3.EditForms
{
    public partial class ValueEditForm : Form
    {
        public ValueMeta valueListElement;
        private ValueMeta srcVM ;
        internal List<string> StructVal;
        public ValueEditForm()
        {
            InitializeComponent();
            srcVM = new StringValMeta(new ArrayList( new object[] { "DefValName", "string", 0, 255, "" }));
        }

        public ValueEditForm(String Caption,ValueMeta vm)
        {
            InitializeComponent();
            this.Text = Caption;
            srcVM = vm;
            textBox1.Text = vm.SemanticName;
        }

        private void ValueEditForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = getMetaType(srcVM);
            switch (comboBox1.SelectedIndex)
            {
                case (6):
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                    textBox4.Text = ((StructValMeta)srcVM).GetCSharpTypeName();
                    break;
                default:
                    textBox1.Text = (srcVM.SemanticName);
                    ArrayList srcmm = srcVM.Serialize();
                    textBox2.Text = srcmm[2].ToString();
                    textBox3.Text = srcmm[3].ToString();
                    textBox4.Text = srcmm[4].ToString();
                    break;
            }
            //
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        }

        private int getMetaType(ValueMeta vm)
        {//            String,Int,Double,Bytes,List,LinkToGlob   
            if (vm.GetType().Equals(typeof(StructValMeta)))
            { return 6; }
            if (vm.GetType().Equals(typeof(ListValMeta)))
            { return 4; }
            if (vm.GetType().Equals(typeof(BytesValMeta)))
            { return 3; }
            if (vm.GetType().Equals(typeof(DoubleValMeta)))
            { return 2; }
            if (vm.GetType().Equals(typeof(StringValMeta)))
            { return 1; }
            if (vm.GetType().Equals(typeof(IntValMeta)))
            { return 0; }

            return 0;
        }

        public string ReturnValueString()
        {
            ValueVrapper vv = new ValueVrapper(
                textBox1.Text,
                comboBox1.Items[comboBox1.SelectedIndex].ToString(),
                int.Parse(textBox2.Text),
                int.Parse(textBox3.Text),
                textBox4.Text);
            return vv.toValueString();
        }

        public ValueMeta ReturnValueMeta()
        {
            ArrayList mm = new ArrayList() { 
                textBox1.Text,
                comboBox1.SelectedItem.ToString(),
                textBox2.Text,
                textBox3.Text, 
                textBox4.Text };


            switch (comboBox1.SelectedIndex)
            {
                //String,Int,Double
                case (0): return new IntValMeta(mm);
                case (1): return new StringValMeta(mm);                
                case (2): return new DoubleValMeta(mm);
                case (3): return new BytesValMeta(mm);
                case (4): return new ListValMeta(mm, valueListElement);
                case (5): return new StringValMeta(mm);//link to glob
                case (6): return new StructValMeta(textBox1.Text, textBox4.Text);
                //surprise ?
                default: return null;//return new MetaCounter(0,textBox1.Text);
            }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //String,Int,Double,Bytes,List,LinkToGlob

            //textBox2.Text = srcVM.Serialize()[2].ToString(); 
            //textBox3.Text = srcVM.Serialize()[3].ToString();
            textBox4.Enabled = true; textBox3.Enabled = true; textBox2.Enabled = true;
            switch (comboBox1.SelectedIndex)
            {
                //block minval, dialog to set path to def byteStream
                case (3):
                    {
                        oneFieldEditForm def = new oneFieldEditForm("path to bytestream",
                            "set path to file, that will be used as default byte stream");
                        def.ShowDialog();
                        if (def.DialogResult == DialogResult.OK)
                        {
                            string filePath = def.ReturnValueString();//.Replace("\\","\\\\");/// C:\\Users\\Андрей\\Pictures\\YfD6qf0NTSk.jpg";// !!!!!!!!!!!!!!!!!!!!!!!! !!

                            textBox4.Text = filePath;

                            textBox4.Enabled = false;
                        }
                        def.Close();
                    }
                    break;
                //block def, dialog to set listparam 
                case (4):
                    {
                        textBox4.Enabled = false;
                        ValueEditForm listTypeForm = (this.srcVM.GetType().Equals(typeof(ListValMeta)))
                            ?new ValueEditForm("ListElem Edit Form",((ListValMeta)srcVM).ElemMeta)
                            :new ValueEditForm();
                        listTypeForm.StructVal = StructVal;
                        listTypeForm.ShowDialog();
                        if (listTypeForm.DialogResult == DialogResult.OK)
                        {
                            valueListElement = listTypeForm.ReturnValueMeta();
                            textBox4.Text = valueListElement.ToString();
                        }
                        listTypeForm.Close();

                        break;
                    }
                //block min max, dialog to set path to glob
                case (5):
                    {
                        textBox2.Enabled = false; textBox3.Enabled = false;
                        oneFieldEditForm def = new oneFieldEditForm("path to glob",
                                    "set dataGlobName, that will be used as datatype", "wows");
                        
                        def.ShowDialog();
                        if (def.DialogResult == DialogResult.OK)
                        {
                            textBox4.Text = def.ReturnValueString();
                        }
                        def.Close();
                        textBox3.Text = double.MaxValue.ToString();
                    } break;
                case (6):
                    {
                        textBox2.Enabled = false; textBox3.Enabled = false;
                        textBox4.Enabled = false;
                        oneFieldEditFormComboBox def = new oneFieldEditFormComboBox("path to struct",
                                    "set StructName, that will be used as datatype", StructVal.ToArray()); //!!! Warning !! must be not null
                        def.ShowDialog();
                        if (def.DialogResult == DialogResult.OK)
                        {
                            textBox4.Text = def.ReturnValueString();
                        }
                        def.Close();
                    } break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
