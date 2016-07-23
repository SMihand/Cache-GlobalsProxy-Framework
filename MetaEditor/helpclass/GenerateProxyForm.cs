using CacheEXTREME2.WProxyGlobal;
using CacheEXTREME2.WMetaGlobal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MetaCache_v3.helpclass
{
    public partial class GenerateProxyForm : Form
    {
        private InterSystems.Globals.Connection _conn;
        private GlobalMeta globalMeta;
        public ContextGenerator ctxGen;

        public GenerateProxyForm(InterSystems.Globals.Connection _conn, GlobalMeta globalMeta)
        {
            // TODO: Complete member initialization
            this._conn = _conn;
            this.globalMeta = globalMeta;
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
           /* if (saveFileDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                try
                {
                    string str = saveFileDialog1.FileName;
                    int lastScope = str.LastIndexOf("\\");
                    txtDirectoryPath.Text = str.Substring(0, lastScope + 1);
                }
                catch
                {
                    MessageBox.Show("wrong filename");
                }
            }*/
            if (this.folderBrowserDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                try
                {
                    txtDirectoryPath.Text = folderBrowserDialog1.SelectedPath;
                }
                catch
                {
                    MessageBox.Show("wrong filename");
                }
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string path = txtDirectoryPath.Text;
                string contextName = txtGlobalContextName.Text.Equals(String.Empty) 
                    ? globalMeta.GlobalName + "Context" 
                    : txtGlobalContextName.Text;
                string nspace = txtNamespaceName.Text.Equals(String.Empty)
                    ? globalMeta.GlobalName + "Namespace"
                    : txtNamespaceName.Text;
                ctxGen = new ContextGenerator(globalMeta,globalMeta.GlobalName,nspace,path+"\\"+contextName+".cs");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GenerateProxyForm_Load(object sender, EventArgs e)
        {
            this.Text = "Generator for global: " + globalMeta.GlobalName;
        }
    }
}
