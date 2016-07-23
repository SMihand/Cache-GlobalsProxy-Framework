using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MetaCache_v3.EditForms
{
    public partial class oneFieldEditFormComboBox : Form
    {
        public oneFieldEditFormComboBox(String FormCaption,String lblDescription, IList  DefValues)
        {
            InitializeComponent();
            ControlBox = false; //??
            StartPosition = FormStartPosition.CenterParent;
            btnOK.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
            
            Text = FormCaption;
            label.Text = lblDescription;
            comboBox1.Items.AddRange(new ArrayList(DefValues).ToArray());
        }



        public string ReturnValueString()
        {
            return comboBox1.Text;
        }
    }
}
