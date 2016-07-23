using System;
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
    public partial class oneFieldEditForm : Form
    {
        public oneFieldEditForm(String FormCaption,String lblDescription, String DefValue="")
        {
            InitializeComponent();
            ControlBox = false; //??
            StartPosition = FormStartPosition.CenterParent;
            btnOK.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
            
            Text = FormCaption;
            label.Text = lblDescription;
            textBox.Text = DefValue;
        }



        public string ReturnValueString()
        {
            return textBox.Text;
        }
    }
}
