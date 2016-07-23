using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InterSystems.Globals;

namespace MetaCache_v3
{
    public partial class AuthorizationForm : Form
    {
        Connection conn;
        //TrueNodeReference nodeRef;
        //private string _globalName = "ProjData";
        private string _namespace = "USER";
        private string _user = "_SYSTEM";// "mihand";
        private string _password = "cachepass";//= "19735";

        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void AuthorizationForm_Load(object sender, EventArgs e)
        {
            txtNamespace.Text = _namespace;
            txtUser.Text = _user;
            txtPassword.Text = _password;
        }
        public Connection getConn()
        {
            return conn;
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {

            try
            {
                conn = ConnectionContext.GetConnection();
                _namespace = txtNamespace.Text;
                _user = txtUser.Text;
                _password = txtPassword.Text;
                if (conn.IsConnected())
                { conn.Close(); }
                // conn.Connect();
                conn.Connect(_namespace, _user, _password);

                if (conn.IsConnected())
                {
                    MainForm mf = new MainForm(conn);

                    mf.Show();
                }

                
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }

        }
    }
}
