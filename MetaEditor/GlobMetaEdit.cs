using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InterSystems.Globals;
using CacheEXTREME2.WMetaGlobal;
using MetaCache_v3.EditForms;
using MetaCache_v3.helpclass;
using CacheEXTREME2.WDirectGlobal;
using System.Collections;

namespace MetaCache_v3
{
    public partial class GlobMetaEdit : Form
    {
        String _globName;
        InterSystems.Globals.Connection _conn;

        GlobalMeta globalMeta;
        //MetaReaderWriter MR;
        CacheEXTREME2.WMetaGlobal.MetaReaderWriter MRW;
        List<string> keys = new List<string>();
        List<string> structs = new List<string>();
        bool _structEdit = false;
        int subIndex = 0;
        //int structIndex = 0;
        bool isStructEditor
        {
            // get { return _structEdit; }
            set
            {
                if (value)
                {
                    statusStrip1.Items[0].Text = "StructEdit Mode On";
                }
                else
                {
                    if (subIndex > 0)
                    {
                        statusStrip1.Items[0].Text = "Index [" + subIndex + "] Editor "
                            + globalMeta.GetKeysMeta(globalMeta.KeysCount)[subIndex - 1].ToString();
                    }
                }
                _structEdit = value;
            }
        }

        public GlobMetaEdit(InterSystems.Globals.Connection conn, String GlobName)
        {
            InitializeComponent();
            _conn = conn;
            _globName = GlobName;

        }

        private void GlobMetaEdit_Load(object sender, EventArgs e)
        {
            MRW = new MetaReaderWriter(_conn);
            Text = _globName;
            // проверка на существование такого глобала, загрузка его
            if (_conn.CreateNodeReference(_globName).HasData())
            {
                globalMeta = MRW.GetMeta(_globName);
            }
            foreach (ValueMeta vv in globalMeta.GetKeysMeta(globalMeta.KeysCount))
            {
                keys.Add(vv.SemanticName);
                SubListBox.Items.Add(keys.Count + "  " + vv.SemanticName);
            }

            foreach (StructDefinition structVal in globalMeta.GetLocalStructs())
                structs.Add(structVal.StructTypeName);
            StructListBox.Items.AddRange(structs.ToArray());

            ValListBox.Items.Clear();

            subUp.Visible = false;
            SubDown.Visible = false;
            ValUp.Visible = false;
            ValDown.Visible = false;

            btnEditSub.Enabled = false;
            btnDeleteSub.Enabled = false;
            btnEditVal.Enabled = false;
            btnDeleteVal.Enabled = false;

        }
        //Просто создаем новую запись о сабскрипте, активируем его, передаем управление СабЕдиту
        private void AddSub_Click(object sender, EventArgs e)
        {
            ValueMeta vm;
            SubscriptEditForm newSubscript = new SubscriptEditForm();
            newSubscript.StructVal = structs;
            newSubscript.ShowDialog();

            if (newSubscript.DialogResult == DialogResult.OK)
            {
                vm = newSubscript.ReturnValueMeta();
                keys.Add(vm.SemanticName);
                SubListBox.Items.Add(keys.Count + "  " + vm.SemanticName);
                globalMeta.AddKeyMeta(vm, newSubscript.ReturnValuesDescription());
                //globalMeta.AddSubscript(vm, newSubscript.ReturnValuesDescription());

            }
            newSubscript.Close();
            subIndex = keys.Count;
            isStructEditor = false;
        }

        private void btnDeleteSub_Click(object sender, EventArgs e)
        {
            if (subIndex >= 0 && subIndex <= globalMeta.KeysCount)
            {
                DialogResult dr = MessageBox.Show(
                    "Deleting subscript: " + globalMeta[subIndex - 1].ToString()
                        + " will delete all node meta"
                    , "Deleting index"
                    , MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    SubListBox.Items.RemoveAt(subIndex - 1);
                    globalMeta.RemoveKey(subIndex - 1);
                    ValListBox.Text = "";
                }
            }
        }
        //
        //subscripts work
        private void EditSub_Click(object sender, EventArgs e)
        {
            ValueMeta vm = globalMeta.GetKeysMeta(globalMeta.KeysCount)[SubListBox.SelectedIndex];
            /// ОСТОРОЖНО ХАК!!
            SubscriptEditForm newSubscript = new SubscriptEditForm(vm,
                // передаем параметры VM описанние из МС. Если нет такого - заполняем  пустым. а почему его нету это уже вопрос.
                (globalMeta.GetNodeMeta(subIndex - 1).Value.Count == 0) ? "" : globalMeta[subIndex - 1, 0].SemanticName);
            newSubscript.StructVal = structs;
            newSubscript.ShowDialog();

            if (newSubscript.DialogResult == DialogResult.OK)
            {
                ValueMeta nvm = newSubscript.ReturnValueMeta();
                int selecedIndex = keys.IndexOf(vm.SemanticName);
                keys[selecedIndex] = nvm.SemanticName;
                SubListBox.Items[selecedIndex] = (selecedIndex + 1 + "  " + nvm.SemanticName);

                globalMeta.ResetKeyMeta(vm, nvm, newSubscript.ReturnValuesDescription());
            }
            newSubscript.Close();
            isStructEditor = false;
        }
        private void AddVal_Click(object sender, EventArgs e)
        {
            ValueMeta vm;
            ValueEditForm newValue = new ValueEditForm();
            newValue.StructVal = structs;
            newValue.ShowDialog();
            if (newValue.DialogResult == DialogResult.OK && subIndex >= 0)
            {
                vm = newValue.ReturnValueMeta();
                globalMeta.AddValueMeta(subIndex - 1, vm);
                ValListBox.Items.Add(vm.ToString());
            }
            newValue.Close();
        }
        private void btnDeleteVal_Click(object sender, EventArgs e)
        {
            if (SubListBox.SelectedIndex >= 0 && ValListBox.SelectedIndex >= 0)
            {
                DialogResult dr = MessageBox.Show(globalMeta[subIndex - 1, ValListBox.SelectedIndex].ToString()
                    , "Deleting operation", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    globalMeta.DeleteValueMeta(subIndex - 1, ValListBox.SelectedIndex);
                }
                //globalMeta[subIndex].Value.RemoveAt(ValListBox.SelectedIndex);
            }
        }
        //
        //valueswork
        private void btnEditVal_Click(object sender, EventArgs e)
        {
            ValueMeta vm;
            /*if (globalMeta[subIndex].Value[ValListBox.SelectedIndex].ExtremeType == ExtremeTypes.EXTREME_STRUCT)
            {
                StructListBox.SelectedIndex = StructListBox.Items.IndexOf(
                    ((StructValMeta)globalMeta[subIndex].Value[ValListBox.SelectedIndex]).StructTypeName);
                isStructEditor = true;
                StructListBox_SelectedIndexChanged(sender, e);
                return;
            }*/

            ValueEditForm newValue = new ValueEditForm("Existed Value Edit Form", globalMeta[subIndex - 1, ValListBox.SelectedIndex]);
            newValue.StructVal = structs;
            newValue.ShowDialog();
            if (newValue.DialogResult == DialogResult.OK)
            {
                vm = newValue.ReturnValueMeta();
                globalMeta.EditValueMeta(subIndex - 1, ValListBox.SelectedIndex, vm);
                //MessageBox.Show("Not implementet Editing Value");
                //globalMeta[subIndex, ValListBox.SelectedIndex] = vm;
                //globalMeta[subIndex].Value[ValListBox.SelectedIndex] = (vm);
                ValListBox.Items[ValListBox.SelectedIndex] = (vm.ToString());
            }
            newValue.Close();
        }
        /// <summary>
        /// Сохранить результаты изменений 
        /// нужно применить метаврайтер !!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            MRW.SaveMeta(globalMeta);
            DialogResult = DialogResult.OK;
        }
        //
        private void SubListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SubListBox.SelectedItem != null)
            {
                subIndex = SubListBox.SelectedIndex + 1;
                ValListBox.Items.Clear();
                if (globalMeta.KeysCount >= 1)
                {
                    List<ValueMeta> vl = globalMeta.GetNodeMeta(subIndex - 1).Value;//globalMeta.GetEntityValuesMeta(SubListBox.SelectedItem.ToString()).Value;
                    //
                    if (vl != null)
                    {
                        if (vl.Count > 0)
                        {
                            ValListBox.Items.AddRange(vl.ToArray());
                            //ValListBox.Items.RemoveAt(0);
                        }
                    }
                }
                btnEditSub.Enabled = ((SubListBox.Items.Count > 0) && (SubListBox.SelectedIndex >= 0)) ? true : false;
                btnDeleteSub.Enabled = ((SubListBox.Items.Count > 0) && (SubListBox.SelectedIndex >= 0)) ? true : false;
                isStructEditor = false;
                ValListBox_SelectedIndexChanged(sender, e);
            }
        }
        private void ValListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditVal.Enabled = ((ValListBox.Items.Count > 0) && (ValListBox.SelectedIndex >= 0)) ? true : false;
            btnDeleteVal.Enabled = ((ValListBox.Items.Count > 0) && (ValListBox.SelectedIndex >= 0)) ? true : false;
        }
        //
        //
        private void subUp_Click(object sender, EventArgs e)
        {
            int sIndex = SubListBox.SelectedIndex;
            if (sIndex > 1)
            {
                //KeyValuePair<string, List<ValueMeta>> kvA = globalMeta[sIndex];
                KeyValuePair<string, List<ValueMeta>> kvA = globalMeta.GetNodeMeta(sIndex);

                //KeyValuePair<string, List<ValueMeta>> kvB = globalMeta[sIndex - 1];
                KeyValuePair<string, List<ValueMeta>> kvB = globalMeta.GetNodeMeta(sIndex - 1);
            }
        }
        //
        //
        //Struct works
        private void AddStruct_Click(object sender, EventArgs e)
        {
            String newStructName = "StructName_" + structs.Count;
            do { newStructName = AddStructDialog(newStructName); }
            while (structs.Contains(newStructName));

            StructDefinition sdf = new StructDefinition();
            sdf.StructTypeName = newStructName;
            globalMeta.AddStruct(sdf);
            structs.Add(newStructName);
            StructListBox.Items.Clear();
            StructListBox.Items.AddRange(structs.ToArray());
            StructListBox.SelectedItem = newStructName;
        }

        private string AddStructDialog(string prevName)
        {
            oneFieldEditForm newGlobNameForm = new oneFieldEditForm("New Struct Name", "Enter Struct name.", prevName);
            newGlobNameForm.ShowDialog();
            if (newGlobNameForm.DialogResult == DialogResult.OK)
                return newGlobNameForm.ReturnValueString();
            return prevName;
        }

        private void StructListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (StructListBox.SelectedItem != null)
            {
                StructDefinition svm = new StructDefinition();

                StructElemListBox1.Items.Clear();
                //   if (globalMeta.localStructs.Count > 1)
                {
                    svm = globalMeta.GetStructDefinition(StructListBox.SelectedItem.ToString());
                    StructElemListBox1.Items.AddRange(svm.elementsMeta.ToArray());
                }
                EditStruct.Enabled = ((StructListBox.Items.Count > 0) && (StructListBox.SelectedIndex >= 0)) ? true : false;
                isStructEditor = true;
            }
        }

        private void EditStruct_Click(object sender, EventArgs e)
        {
            String oldStructName = StructListBox.Items[StructListBox.SelectedIndex].ToString();
            String newStructName = AddStructDialog(oldStructName);
            if (oldStructName.Equals(newStructName)) return;


            StructListBox.Items.Clear();
            structs[StructListBox.SelectedIndex] = newStructName;
            StructListBox.Items.AddRange(structs.ToArray());
            StructListBox.SelectedItem = newStructName;
            isStructEditor = true;
        }

        private void butAddValToStruct_Click(object sender, EventArgs e) // Add Val to stuct
        {
            ValueMeta vm;
            ValueEditForm newValue = new ValueEditForm();
            newValue.StructVal = structs;
            newValue.ShowDialog();
            if (newValue.DialogResult == DialogResult.OK)
            {
                vm = newValue.ReturnValueMeta();
                StructDefinition sMeta = globalMeta.GetStructDefinition(StructListBox.SelectedItem.ToString());
                sMeta.elementsMeta.Add(vm);
                //globalMeta.localStructs[StructListBox.SelectedItem.ToString()].elementsMeta.Add(vm);
                StructElemListBox1.Items.Add(vm.ToString());
            }
            newValue.Close();
        }

        private void btnEditStructVal_Click(object sender, EventArgs e)// EditValue
        {
            ValueMeta vm;
            /*if (globalMeta.GetStruct(StructListBox.SelectedItem.ToString()).elementsMeta[StructElemListBox1.SelectedIndex].ExtremeType == ExtremeTypes.EXTREME_STRUCT)
            {
                StructListBox.SelectedIndex = StructListBox.Items.IndexOf(
                    ((StructValMeta)globalMeta.GetStruct(StructListBox.SelectedItem.ToString()).elementsMeta[StructElemListBox1.SelectedIndex]).StructTypeName);

                StructListBox_SelectedIndexChanged(sender, e);
                return;
            }*/

            ValueEditForm newValue = new ValueEditForm("Existed Value Edit Form",
                globalMeta.GetStructDefinition(StructListBox.SelectedItem.ToString()).elementsMeta[StructElemListBox1.SelectedIndex]);
            newValue.StructVal = structs;
            newValue.ShowDialog();
            if (newValue.DialogResult == DialogResult.OK)
            {
                vm = newValue.ReturnValueMeta();
                //
                StructDefinition edited = globalMeta.GetStructDefinition(StructListBox.SelectedItem.ToString());
                edited.elementsMeta[StructElemListBox1.SelectedIndex] = vm;
                globalMeta.EditStruct(edited.StructTypeName, edited);
                //vvv   old version   vvv
                //globalMeta.GetStruct(StructListBox.SelectedItem.ToString()).elementsMeta.Insert(StructElemListBox1.SelectedIndex, vm);

                StructElemListBox1.Items[StructElemListBox1.SelectedIndex] = (vm.ToString());
            }
            newValue.Close();
        }

        private void StructElemListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnGenerateProxy_Click(object sender, EventArgs e)
        {
            GenerateProxyForm genForm = new GenerateProxyForm(_conn, globalMeta);
            if (genForm.ShowDialog().Equals(DialogResult.OK))
            {
                try
                {
                    genForm.ctxGen.GenerateCSharpCode();
                    MessageBox.Show("Generation completed!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnDeleteStruct_Click(object sender, EventArgs e)
        {
            if (StructElemListBox1.SelectedIndex >= 0 && StructElemListBox1.SelectedIndex < globalMeta.StructSequence.Count)
            {
                string structName = StructListBox.Items[StructListBox.SelectedIndex].ToString();
                DialogResult dr = MessageBox.Show("Doy you realy want do delete struct: " + structName + " it also delete all its entries in nodes", "eleting struct", MessageBoxButtons.YesNoCancel);
                if (dr.Equals(DialogResult.OK))
                {
                    globalMeta.RemoveStruct(structName);
                }
            }

        }

        private void btnDeleteStructVal_Click(object sender, EventArgs e)
        {
            if (StructElemListBox1.SelectedIndex >= 0 && StructElemListBox1.SelectedIndex < globalMeta.GetStructDefinition(StructListBox.SelectedItem.ToString()).elementsMeta.Count)
            {
                StructDefinition edited = globalMeta.GetStructDefinition(StructListBox.SelectedItem.ToString());
                DialogResult dr = MessageBox.Show(edited.elementsMeta[StructElemListBox1.SelectedIndex].SemanticName
                        + " from " + StructListBox.SelectedItem.ToString()
                    , "Deleting operation", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.OK)
                {
                    edited.elementsMeta.RemoveAt(StructElemListBox1.SelectedIndex);
                    StructElemListBox1.Items.RemoveAt(StructElemListBox1.SelectedIndex);
                }

            }
        }

        private void exportMetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrueNodeReference globToExport = new TrueNodeReference(_conn, _globName);
            List<string> globalSerializedList = new List<string>();
            treeWalkForSerialize(globToExport, globalSerializedList);
        }

        private void treeWalkForSerialize(TrueNodeReference glNode, List<string> serializedList)
        {
            glNode.AppendSubscript("");

            while (glNode.NextSubscript() != "")
            {
                glNode.GoNextSubscript();
                if (glNode.HasSubnodes())
                {
                    treeWalkForSerialize(glNode, serializedList);
                    glNode.GoParentNodeSubscripts();
                }
                if (glNode.HasValues())
                {
                    string keys = SerializeKeys(glNode.GetSubscripts());

                    serializedList.Add(keys);
                }
            }
        }

        private string SerializeKeys(ArrayList Keys)
        {
            StringBuilder sb = new StringBuilder(1000);
            foreach (var item in keys)
            {
                sb.Append(item.ToString()+",");
            }
            return sb.ToString();
        }

        private string SerializeValue(object value)
        {
            StringBuilder sb = new StringBuilder(1500);
            if(value is ValueList)
            {

            }
            return sb.ToString();
        }

        private string SerializeValueList(ValueList list)
        {
            StringBuilder sb = new StringBuilder(1000);
            sb.Append("{");
            foreach (var item in list.GetAll())
            {
                if(item is ValueList)
                {
                    sb.Append("{" + SerializeValueList(item as ValueList) + "}");
                }
                else
                {
                    sb.Append(item.ToString());
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}


