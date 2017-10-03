namespace CacheEXTREMELab
{
    partial class GlobalViewEEditForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtinfo = new System.Windows.Forms.TextBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Values = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValueTypes = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtindex = new System.Windows.Forms.TextBox();
            this.btnsave = new System.Windows.Forms.Button();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.cmbGlobals = new System.Windows.Forms.ComboBox();
            this.btnDeleteGlobal = new System.Windows.Forms.Button();
            this.btnDeleteNode = new System.Windows.Forms.Button();
            this.txtNodeData = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // txtinfo
            // 
            this.txtinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtinfo.Location = new System.Drawing.Point(12, 38);
            this.txtinfo.Name = "txtinfo";
            this.txtinfo.Size = new System.Drawing.Size(776, 20);
            this.txtinfo.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(12, 63);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(461, 271);
            this.treeView.TabIndex = 1;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Values,
            this.ValueTypes});
            this.dataGridView.Location = new System.Drawing.Point(479, 90);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(309, 215);
            this.dataGridView.TabIndex = 2;
            // 
            // Values
            // 
            this.Values.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.Values.DefaultCellStyle = dataGridViewCellStyle6;
            this.Values.HeaderText = "Values";
            this.Values.Name = "Values";
            // 
            // ValueTypes
            // 
            this.ValueTypes.HeaderText = "ValueTypes";
            this.ValueTypes.Items.AddRange(new object[] {
            "System.String",
            "System.Double",
            "System.Int32"});
            this.ValueTypes.Name = "ValueTypes";
            this.ValueTypes.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ValueTypes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // txtindex
            // 
            this.txtindex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtindex.Location = new System.Drawing.Point(479, 63);
            this.txtindex.Name = "txtindex";
            this.txtindex.Size = new System.Drawing.Size(309, 20);
            this.txtindex.TabIndex = 3;
            // 
            // btnsave
            // 
            this.btnsave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnsave.Location = new System.Drawing.Point(479, 311);
            this.btnsave.Name = "btnsave";
            this.btnsave.Size = new System.Drawing.Size(212, 23);
            this.btnsave.TabIndex = 4;
            this.btnsave.Text = "Save";
            this.btnsave.UseVisualStyleBackColor = true;
            this.btnsave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtNamespace
            // 
            this.txtNamespace.Location = new System.Drawing.Point(12, 9);
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(130, 20);
            this.txtNamespace.TabIndex = 6;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(148, 9);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(130, 20);
            this.txtUser.TabIndex = 7;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(284, 9);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(130, 20);
            this.txtPassword.TabIndex = 8;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(420, 7);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(71, 23);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(636, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(71, 23);
            this.btnBrowse.TabIndex = 10;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // cmbGlobals
            // 
            this.cmbGlobals.FormattingEnabled = true;
            this.cmbGlobals.Location = new System.Drawing.Point(513, 8);
            this.cmbGlobals.Name = "cmbGlobals";
            this.cmbGlobals.Size = new System.Drawing.Size(117, 21);
            this.cmbGlobals.TabIndex = 11;
            // 
            // btnDeleteGlobal
            // 
            this.btnDeleteGlobal.Location = new System.Drawing.Point(713, 6);
            this.btnDeleteGlobal.Name = "btnDeleteGlobal";
            this.btnDeleteGlobal.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteGlobal.TabIndex = 12;
            this.btnDeleteGlobal.Text = "Delete";
            this.btnDeleteGlobal.UseVisualStyleBackColor = true;
            this.btnDeleteGlobal.Click += new System.EventHandler(this.btnDeleteGlobal_Click);
            // 
            // btnDeleteNode
            // 
            this.btnDeleteNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteNode.Location = new System.Drawing.Point(698, 311);
            this.btnDeleteNode.Name = "btnDeleteNode";
            this.btnDeleteNode.Size = new System.Drawing.Size(90, 23);
            this.btnDeleteNode.TabIndex = 13;
            this.btnDeleteNode.Text = "Delete";
            this.btnDeleteNode.UseVisualStyleBackColor = true;
            this.btnDeleteNode.Click += new System.EventHandler(this.btnDeleteNode_Click);
            // 
            // txtNodeData
            // 
            this.txtNodeData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNodeData.Location = new System.Drawing.Point(12, 342);
            this.txtNodeData.Name = "txtNodeData";
            this.txtNodeData.Size = new System.Drawing.Size(776, 20);
            this.txtNodeData.TabIndex = 14;
            // 
            // GlobalViewEEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 374);
            this.Controls.Add(this.txtNodeData);
            this.Controls.Add(this.btnDeleteNode);
            this.Controls.Add(this.btnDeleteGlobal);
            this.Controls.Add(this.cmbGlobals);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.txtNamespace);
            this.Controls.Add(this.btnsave);
            this.Controls.Add(this.txtindex);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.txtinfo);
            this.Name = "GlobalViewEEditForm";
            this.Text = "GlobalViewEEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WOWSform_FormClosing);
            this.Load += new System.EventHandler(this.GlobalViewEEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtinfo;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox txtindex;
        private System.Windows.Forms.Button btnsave;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.DataGridViewTextBoxColumn Values;
        private System.Windows.Forms.DataGridViewComboBoxColumn ValueTypes;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ComboBox cmbGlobals;
        private System.Windows.Forms.Button btnDeleteGlobal;
        private System.Windows.Forms.Button btnDeleteNode;
        private System.Windows.Forms.TextBox txtNodeData;
    }
}

