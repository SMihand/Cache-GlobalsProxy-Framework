using InterSystems.Globals;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

using CacheEXTREME2.WDirectGlobal;
using CacheEXTREME2.WMetaGlobal;

using CacheEXTREME2;

namespace CacheEXTREMELab
{
    public partial class GlobalViewEEditForm : Form
    {
        TrueNodeReference nodeRef;
        private string _globalName = "wows";
        private string _namespace = "USER";
        private string _user = "mihand";
        private string _password = "123456";
        private Connection conn;

        public GlobalViewEEditForm()
        {
            InitializeComponent();
        }

        private void GlobalViewEEditForm_Load(object sender, EventArgs e)
        {
            cmbGlobals.Enabled = false;
            cmbGlobals.BackColor = Color.LightGray;
            cmbGlobals.Text = _globalName;
            txtNamespace.Text = _namespace;
            txtUser.Text = _user;
            txtPassword.Text = _password;
            cmbGlobalsReload();
        }

        private void cmbGlobalsReload()
        {
            try
            {
                if (conn == null)
                {
                    conn = ConnectionContext.GetConnection();
                    conn.Connect(_namespace, _user, _password);
                }
                if (conn.IsConnected())
                {
                    //tests();
                    txtinfo.Text = "З’єднання з БД виконано " + _namespace;
                    GlobalsDirectory dir = conn.CreateGlobalsDirectory();
                    string global = _globalName; cmbGlobals.Items.Clear();
                    do
                    {
                        global = dir.NextGlobalName();
                        cmbGlobals.Items.Add(global);
                    } while (!global.Equals(string.Empty));
                    cmbGlobals.Enabled = true;
                    cmbGlobals.BackColor = Color.White;
                    return;
                }
                throw new Exception("Something wrong while connecting.");
            }
            catch (Exception ex)
            {
                txtinfo.Text = ex.Message;
                conn.Close();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _globalName = cmbGlobals.Text;
                _namespace = txtNamespace.Text;
                _user = txtUser.Text;
                _password = txtPassword.Text;
                conn.Close();
                cmbGlobals.Enabled = false;
                cmbGlobals.BackColor = Color.LightGray;
                this.GlobalViewEEditForm_Load(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            _globalName = cmbGlobals.Text;
            try
            {
                nodeRef = new TrueNodeReference(conn, _globalName);
                CacheTreeNode globalTree = CacheHelper.CacheGlobalToCacheTree(nodeRef);
                string globalKey = _namespace + '.' + _globalName;
                globalTree.Name = globalKey;
                object[] ooo = treeView.Nodes.Find(globalTree.Name, false);
                if (treeView.Nodes.ContainsKey(globalKey))
                {
                    treeView.Nodes.RemoveByKey(globalKey);
                }
                treeView.Nodes.Add(globalTree);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot load global: " + _globalName + " cause: " + ex.Message);
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                dataGridView.Rows.Clear();
                CacheTreeNode node = e.Node as CacheTreeNode;
                //
                nodeRef = new TrueNodeReference(conn, node.globalName, node.SubscriptsTyped);
                //
                if (node.Subscripts.Length > 0)
                {
                    txtindex.Text = node.Subscripts.Last();
                }
                txtinfo.Clear();
                txtNodeData.Text = "(";
                ArrayList subscripts = nodeRef.GetSubscripts();
                for (int i = 0; i < subscripts.Count; i++)
                {
                    txtinfo.Text += subscripts[i].ToString() + "(" + subscripts[i].GetType().ToString() + ")" + " : ";
                    txtNodeData.Text += (i < subscripts.Count - 1) ? subscripts[i].ToString() + ", " : subscripts[i];
                }
                try
                {
                    ArrayList values = nodeRef.TryGetValues();
                    txtNodeData.Text += ")=(";
                    for (int i = 0; i < values.Count; i++)
                    {
                        dataGridView.Rows.Add();
                        if (values[i] is IList)
                        {
                            txtNodeData.Text += complexListToString(values[i] as IList);
                            dataGridView["Values", i].Value = "Array value see under";
                        }
                        else
                        {
                            dataGridView["Values", i].Value = values[i];
                            txtNodeData.Text += values[i];
                            //((DataGridViewComboBoxCell)dataGridView["ValueTypes", i]).Value = t.Equals(typeof(Decimal)) ? typeof(Double).ToString() : t.ToString();
                        }
                        txtNodeData.Text += (i < values.Count - 1) ? ", " : "";
                    }
                    txtNodeData.Text += ")";
                }
                catch (Exception ex)
                {
                    txtinfo.Text = ex.Message;
                }
            }
            catch (Exception ex) { txtinfo.Text = ex.Message; }
        }
        private string complexListToString(IList values)
        {
            string toReturn = "(";
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] is IList)
                {
                    toReturn += complexListToString(values[i] as IList);
                }
                else
                {
                    toReturn += values[i].ToString();
                }
                toReturn += (i < values.Count - 1) ? ", " : "";
            }
            toReturn += ")";
            return toReturn;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ArrayList subs = nodeRef.GetSubscripts();
            ArrayList values = new ArrayList();
            try
            {
                for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                {
                    object value = dataGridView["Values", i].Value;
                    object type = dataGridView["ValueTypes", i].Value;
                    if (value != null)
                    {
                        if (type.Equals(typeof(double).ToString()))
                        {
                            values.Add(double.Parse(value.ToString()));
                        }
                        if (type.Equals(typeof(int).ToString()))
                        {
                            values.Add(int.Parse(value.ToString()));
                        }
                        if (type.Equals(typeof(string).ToString()))
                        {
                            values.Add(value.ToString());
                        }
                    }
                }
                nodeRef.SetValues(values);
                txtinfo.Text = "values saved";
            }
            catch (Exception ex)
            {
                txtinfo.Text = ex.Message;
            }
        }

        private void WOWSform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        ///
        ///TESTS
        ///
        private void tests(bool invoke = false) 
        {if(invoke){
            TrueNodeReference WTr = new TrueNodeReference(conn, "wowsTest");
            TrueNodeReference Wr = new TrueNodeReference(conn, "wows");
            NodeReference wtr = conn.CreateNodeReference("wowsTest");
            NodeReference wr = conn.CreateNodeReference("wows");
            GlobalMeta wowsMeta = null;
            GlobalMeta wowsTestMeta = null;
            try
            {
                MetaReaderWriter metaReader = new MetaReaderWriter(conn);
                wowsTestMeta = metaReader.GetMeta("wowsTestMeta");
                wowsMeta = metaReader.GetMeta("wowsMeta");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try { 
                byte[] bytes = File.ReadAllBytes("C:\\Users\\Андрей\\Pictures\\YfD6qf0NTSk.jpg");
                Image img = Image.FromFile("C:\\Users\\Андрей\\Pictures\\YfD6qf0NTSk.jpg");
                byte[] bytesImage = imageToByteArray(img);
                //
                //
                ArrayList keyes = new ArrayList() { "carry", 4 };
                ArrayList values = new ArrayList() { 
                    new ArrayList(){"l1_1","l2_2","L1_33"},
                    new ArrayList(){"l2_1","l2_2","L2_33"}, 
                    1100,
                    bytesImage};
                WTr.SetSubscripts(keyes);
                WTr.Kill();
                WTr.SetValues(keyes, values);
                Image i = byteArrayToImage((byte[])WTr.GetValues(wowsTestMeta.GetNodeMeta(2).Value)[3]);
            }catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }}

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void btnDeleteGlobal_Click(object sender, EventArgs e)
        {
            try
            {
                string globalToDeleteName = this.cmbGlobals.Text;
                NodeReference nr = this.conn.CreateNodeReference(globalToDeleteName);
                if (DialogResult.OK == MessageBox.Show("Do you realy want to kill global: " + nodeRef.GlobalName, "Warning!", MessageBoxButtons.YesNoCancel))
                {
                    nr.Kill();
                }
                cmbGlobalsReload();
                cmbGlobals.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in delete method: " + ex.Message);
            }
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.OK == MessageBox.Show("Do you realy want to kill: " + nodeRef.ToString(), "Warning!", MessageBoxButtons.YesNoCancel))
                {
                    nodeRef.Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}