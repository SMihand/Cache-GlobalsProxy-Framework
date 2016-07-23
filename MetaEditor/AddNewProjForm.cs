using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InterSystems.Globals;
using MetaCache_v3.EditForms;

namespace MetaCache_v3
{
    public partial class AddNewProjForm : Form
    {

        Connection myConn;
        string projName = "";
        ProjHelper pj;

        //public AddNewProjForm(Connection conn)
        //{
        //    InitializeComponent();
        //    myConn = conn;
        //    pj = new ProjHelper(conn);
            
        //    listBox1.Items.AddRange((pj.GetAllFreeGlobals()).ToArray());
        //    listBox1.Items.AddRange((pj.GetAllGlobWithMeta()).ToArray());

        //}

        public AddNewProjForm(Connection conn, String ProjName)
        {
            InitializeComponent();
            myConn = conn;
            this.projName = ProjName;
            textBox1.Text = projName;
            pj = new ProjHelper(conn);

            listBox1.Items.AddRange((pj.GetAllFreeGlobals()).ToArray());
            listBox1.Items.AddRange((pj.GetAllGlobWithMeta()).ToArray());

            listBox2.Items.AddRange(pj.ReadProjGlobalsValue(projName));
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Trim().Equals(""))// && textBox1.Text not in exist-proj-list
            {
                this.DialogResult = DialogResult.OK;

                pj.SetProjData(textBox1.Text, new ArrayList(listBox2.Items).ToArray());
            }
            else MessageBox.Show("ProjTitle must be not void");
        }

        public string ReturnProjName()
        {
            return this.textBox1.Text.Trim();
        }



        public List<String> ReturnProjContent()
        {
            List<String> myList = new List<String>();
            foreach (object item in this.listBox2.Items){
                myList.Add(item.ToString());
            }
            return myList;
        }

        private void btnAddSelected_Click(object sender, EventArgs e)
        {
            if (!listBox2.Items.Contains(listBox1.SelectedItem))
                listBox2.Items.Add(listBox1.SelectedItem);
        }
        private void btnAddAll_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox2.Items.AddRange( listBox1.Items);
        }
        private void btnDelSelected_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.SelectedItem);
        }
        private void btnDelAll_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void AddNewGlob_Click(object sender, EventArgs e)
        {
            //создаем окно выбора имени
            oneFieldEditForm newGlobNameForm = new oneFieldEditForm("New Glob Name", "Enter  DataGlob name.", "newDataGlobName");
            newGlobNameForm.ShowDialog();
            if (newGlobNameForm.DialogResult == DialogResult.OK)
            {
                String DataGlobName = newGlobNameForm.ReturnValueString();
                //globExist Validation
                if (!myConn.CreateNodeReference(DataGlobName).HasData())
                {
                    // создаем глоб в базе
                    NodeReference node = myConn.CreateNodeReference(DataGlobName);
                    node.Set(DataGlobName);
                    node.Close();
                    // обновляем значение листбоксов 
                    listBox1.Items.Add(DataGlobName);
                    listBox2.Items.Add(DataGlobName);
                }
                else
                {
                    MessageBox.Show("Glob Exist");
                    return;
                }

            }
        }

    }
}
