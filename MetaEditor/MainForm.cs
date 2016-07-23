using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using InterSystems.Data.CacheClient;
using InterSystems.Globals;
using MetaCache_v3.EditForms;


namespace MetaCache_v3
{

    public partial class MainForm : Form
    {
        Connection conn;
        //TrueNodeReference nodeRef;
        ////private string _globalName = "ProjData";
        //private string _namespace = "USER";
        //private string _user = "_SYSTEM";// "mihand";
        //private string _password = "cachepass";//= "19735";
        ProjHelper pj;
        String prevProjName = "";


        public MainForm(Connection newConn)
        {
            InitializeComponent();
            conn = newConn;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pj = new ProjHelper(conn);
            // проверка на наличие глобала проектов
            if (!pj.IsProjGlobExist()) pj.CreateProjGlobal();


            //GlobalsDirectory dir = conn.CreateGlobalsDirectory();
            comboBox1.Items.AddRange(pj.ReadProjGlobal());
            comboBox1.SelectedIndex = 0;
            comboBox1.Items.RemoveAt(0);
        }

        /// <summary>
        /// Значение меняется если:
        /// 1) выбрано новое значение 
        /// 2) обновился список значений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                // выбрано создание нового проекта
                if (comboBox1.SelectedItem.ToString().Equals("<NewProj>"))
                {
                    oneFieldEditForm newProjNameForm =
                        new oneFieldEditForm("New Project Name", "Enter  Project name.", "new Project Name");
                    newProjNameForm.ShowDialog();

                    if ((newProjNameForm.DialogResult == DialogResult.OK) | (newProjNameForm.DialogResult == DialogResult.Cancel))
                    {
                        // ОК - проверяем результат, возвращаем, если такое имя уже есть, ничего не меняем.
                        if (newProjNameForm.DialogResult == DialogResult.OK)
                        {
                            String newProjName = newProjNameForm.ReturnValueString();
                            AddNewProjForm secondForm = new AddNewProjForm(conn, newProjName);
                            secondForm.ShowDialog();
                            if (secondForm.DialogResult == DialogResult.OK)
                            {
                                comboBox1.Items.Add(secondForm.ReturnProjName());
                            }
                            secondForm.Close();
                            return;
                        }
                        // cancel 
                        else
                        {
                            MessageBox.Show("This Project exist. Create proj with another name.");
                            comboBox1.SelectedItem = prevProjName;
                            return;
                        }
                    }
                }
                prevProjName = comboBox1.SelectedItem.ToString();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                ArrayList projglobs = new ArrayList(pj.ReadProjGlobalsValue(prevProjName));
                ArrayList metaglobs = pj.GetAllGlobWithMeta();
                foreach (string s in projglobs)
                {
                    if (metaglobs.Contains(s))
                        listBox2.Items.Add(s);
                    else
                        listBox3.Items.Add(s);
                }                
            }
        }

        private void btnAddGlob_Click(object sender, EventArgs e)
        {
            /*
             * Спрашиваем название датаглоба, добавляем его в список проектов с метаглобалами
             * если нет такого метаглоба - создаем метаглобал
             * открываем окно редактирование метаглобала
             */
            if (listBox3.SelectedIndex >= 0)
            {
                string DataToMetaGlobName = listBox3.SelectedItem.ToString();
                pj.CreateMetaGlob(DataToMetaGlobName);
                listBox3.Items.Remove(DataToMetaGlobName);
                listBox2.Items.Add(DataToMetaGlobName);
                listBox2.SelectedItem = DataToMetaGlobName;
                EditGlobMeta(DataToMetaGlobName);
            }
        }
        /// <summary>
        /// Редактор Метаглобов
        /// GlobName = listBox2.SelectedItem.ToString() + ProjHelper._MetaSufix;
        /// нужен Метаврайтер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditGlobMeta_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null) 
                EditGlobMeta(listBox2.SelectedItem.ToString());
        }

        private void EditGlobMeta( String GlobName )
        {
            GlobMetaEdit gmForm = new GlobMetaEdit(conn, GlobName + ProjHelper._MetaSufix);
            gmForm.ShowDialog();

            if (gmForm.DialogResult == DialogResult.OK)
            {
                //parse result      btnEditGlobMeta_Click
            }
            gmForm.Close();
        }

        private void btnEditProj_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0)
            {
                AddNewProjForm secondForm = new AddNewProjForm(conn, comboBox1.SelectedItem.ToString());
                secondForm.ShowDialog();

                if (secondForm.DialogResult == DialogResult.OK)
                {
                    comboBox1.SelectedItem = comboBox1.SelectedItem;
                }
                secondForm.Close();
                comboBox1_SelectedValueChanged(sender, e);
                return;
            }
            MessageBox.Show("Select the project!");
        }
    }
}
