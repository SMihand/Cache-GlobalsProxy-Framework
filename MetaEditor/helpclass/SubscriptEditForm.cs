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

namespace MetaCache_v3.EditForms
{
    public partial class SubscriptEditForm : Form
    {
        internal List<string> StructVal;
        ValueMeta subMeta;
        public SubscriptEditForm()
        {
            InitializeComponent();
        }

        public SubscriptEditForm(ValueMeta vm, string description)
        {
            InitializeComponent();
            textBox5.Text = description;
            textBox1.Text = vm.SemanticName;
            subMeta = vm;
        }

        private void ValueEditForm_Load(object sender, EventArgs e)
        {
            if (subMeta != null)
            {
                comboBox1.SelectedIndex = getMetaType(subMeta);
                switch (comboBox1.SelectedIndex)
                {
                    //String,Int,Double,struct
                    case (0):
                        textBox2.Text = ((StringValMeta)(subMeta)).MinLenght.ToString();
                        textBox3.Text = ((StringValMeta)(subMeta)).MaxLength.ToString();
                        textBox4.Text = ((StringValMeta)subMeta).GetDefaultValue().ToString();
                        break;
                    case (1):
                        textBox2.Text = ((IntValMeta)(subMeta)).Minimum.ToString();
                        textBox3.Text = ((IntValMeta)(subMeta)).Maximum.ToString();
                        textBox4.Text = ((IntValMeta)subMeta).GetDefaultValue().ToString();
                        break;
                    case (2):
                        textBox2.Text = ((DoubleValMeta)(subMeta)).Minimum.ToString();
                        textBox3.Text = ((DoubleValMeta)(subMeta)).Maximum.ToString();
                        textBox4.Text = ((DoubleValMeta)subMeta).GetDefaultValue().ToString();
                        break;
                    case (3):
                        textBox2.Enabled = false;
                        textBox3.Enabled = false;
                        textBox4.Enabled = false;
                        textBox4.Text = ((StructValMeta)subMeta).GetCSharpTypeName();
                        break;
                }
            }
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        }

        private int getMetaType(ValueMeta vm)
        {
            if (vm.ExtremeType == ExtremeTypes.EXTREME_STRUCT) { return 3; }
            if (vm.ExtremeType == ExtremeTypes.EXTREME_DOUBLE) { return 2; }
            if (vm.ExtremeType == ExtremeTypes.EXTREME_INT) { return 1; }
            if (vm.ExtremeType == ExtremeTypes.EXTREME_STRING) { return 0; }
            return 99;
        }
        public string ReturnValuesDescription()
        {
            return textBox5.Text;
        }

        public ValueMeta ReturnValueMeta()
        {
            int type = comboBox1.SelectedIndex;
            ArrayList mm = new ArrayList(new object[] { 
                textBox1.Text,
                comboBox1.SelectedItem.ToString(),
                textBox2.Text,
                textBox3.Text, 
                textBox4.Text });

            switch (comboBox1.SelectedIndex)
            {
                //String,Int,Double
                case (0): return new StringValMeta(mm);
                case (1): return new IntValMeta(mm);
                case (2): return new DoubleValMeta(mm);
                case (3): return new StructValMeta(textBox1.Text, textBox4.Text);
                    //surprise ?
                //default: return new MetaCounter(0, textBox1.Text);
            }
            //added
            return null;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            textBox2.Text = "0";
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            switch (comboBox1.SelectedIndex)
            {    
                //String,Int,Double,struct
                case (0): 
                    textBox3.Text = "255"; 
                    break;
                case (1): 
                    textBox3.Text = "65535";
                    break;
                case (2): 
                    textBox3.Text = "1";
                    break;
                case (3):
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
                    break;
                default: 
                    textBox3.Text = "255";
                    break;
            }
        }
    }
}
