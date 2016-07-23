namespace MetaCache_v3
{
    partial class GlobMetaEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.SubListBox = new System.Windows.Forms.ListBox();
            this.AddSub = new System.Windows.Forms.Button();
            this.btnEditSub = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ValListBox = new System.Windows.Forms.ListBox();
            this.AddVal = new System.Windows.Forms.Button();
            this.btnEditVal = new System.Windows.Forms.Button();
            this.subUp = new System.Windows.Forms.Button();
            this.SubDown = new System.Windows.Forms.Button();
            this.ValUp = new System.Windows.Forms.Button();
            this.ValDown = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.StructListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.AddStruct = new System.Windows.Forms.Button();
            this.EditStruct = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.IndexEditor = new System.Windows.Forms.TabPage();
            this.btnDeleteSub = new System.Windows.Forms.Button();
            this.btnDeleteVal = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.StructEditor = new System.Windows.Forms.TabPage();
            this.StructElemListBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEditStructVal = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnGenerateProxy = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.IndexEditor.SuspendLayout();
            this.StructEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "ListOfSubScripts";
            // 
            // SubListBox
            // 
            this.SubListBox.FormattingEnabled = true;
            this.SubListBox.Location = new System.Drawing.Point(3, 28);
            this.SubListBox.MultiColumn = true;
            this.SubListBox.Name = "SubListBox";
            this.SubListBox.Size = new System.Drawing.Size(120, 147);
            this.SubListBox.TabIndex = 12;
            this.SubListBox.SelectedIndexChanged += new System.EventHandler(this.SubListBox_SelectedIndexChanged);
            // 
            // AddSub
            // 
            this.AddSub.Location = new System.Drawing.Point(3, 180);
            this.AddSub.Name = "AddSub";
            this.AddSub.Size = new System.Drawing.Size(120, 23);
            this.AddSub.TabIndex = 22;
            this.AddSub.Text = "AddSubScript";
            this.AddSub.UseVisualStyleBackColor = true;
            this.AddSub.Click += new System.EventHandler(this.AddSub_Click);
            // 
            // btnEditSub
            // 
            this.btnEditSub.Location = new System.Drawing.Point(3, 209);
            this.btnEditSub.Name = "btnEditSub";
            this.btnEditSub.Size = new System.Drawing.Size(68, 23);
            this.btnEditSub.TabIndex = 22;
            this.btnEditSub.Text = "Edit";
            this.btnEditSub.UseVisualStyleBackColor = true;
            this.btnEditSub.Click += new System.EventHandler(this.EditSub_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(176, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "ValueGrid";
            // 
            // ValListBox
            // 
            this.ValListBox.FormattingEnabled = true;
            this.ValListBox.Items.AddRange(new object[] {
            "\"shipsCount\",\"integer\",0,0,0",
            "\"workers\",\"list\",0,100,\"string\",0,255,0)"});
            this.ValListBox.Location = new System.Drawing.Point(129, 28);
            this.ValListBox.Name = "ValListBox";
            this.ValListBox.Size = new System.Drawing.Size(350, 147);
            this.ValListBox.TabIndex = 12;
            this.ValListBox.SelectedIndexChanged += new System.EventHandler(this.ValListBox_SelectedIndexChanged);
            // 
            // AddVal
            // 
            this.AddVal.Location = new System.Drawing.Point(129, 180);
            this.AddVal.Name = "AddVal";
            this.AddVal.Size = new System.Drawing.Size(140, 23);
            this.AddVal.TabIndex = 22;
            this.AddVal.Text = "AddValue";
            this.AddVal.UseVisualStyleBackColor = true;
            this.AddVal.Click += new System.EventHandler(this.AddVal_Click);
            // 
            // btnEditVal
            // 
            this.btnEditVal.Location = new System.Drawing.Point(129, 209);
            this.btnEditVal.Name = "btnEditVal";
            this.btnEditVal.Size = new System.Drawing.Size(89, 23);
            this.btnEditVal.TabIndex = 22;
            this.btnEditVal.Text = "Edit";
            this.btnEditVal.UseVisualStyleBackColor = true;
            this.btnEditVal.Click += new System.EventHandler(this.btnEditVal_Click);
            // 
            // subUp
            // 
            this.subUp.Enabled = false;
            this.subUp.Location = new System.Drawing.Point(137, 291);
            this.subUp.Name = "subUp";
            this.subUp.Size = new System.Drawing.Size(53, 23);
            this.subUp.TabIndex = 24;
            this.subUp.Text = "up";
            this.subUp.UseVisualStyleBackColor = true;
            this.subUp.Click += new System.EventHandler(this.subUp_Click);
            // 
            // SubDown
            // 
            this.SubDown.Enabled = false;
            this.SubDown.Location = new System.Drawing.Point(137, 313);
            this.SubDown.Name = "SubDown";
            this.SubDown.Size = new System.Drawing.Size(53, 23);
            this.SubDown.TabIndex = 24;
            this.SubDown.Text = "down";
            this.SubDown.UseVisualStyleBackColor = true;
            // 
            // ValUp
            // 
            this.ValUp.Enabled = false;
            this.ValUp.Location = new System.Drawing.Point(196, 292);
            this.ValUp.Name = "ValUp";
            this.ValUp.Size = new System.Drawing.Size(53, 23);
            this.ValUp.TabIndex = 24;
            this.ValUp.Text = "up";
            this.ValUp.UseVisualStyleBackColor = true;
            // 
            // ValDown
            // 
            this.ValDown.Enabled = false;
            this.ValDown.Location = new System.Drawing.Point(196, 314);
            this.ValDown.Name = "ValDown";
            this.ValDown.Size = new System.Drawing.Size(53, 23);
            this.ValDown.TabIndex = 24;
            this.ValDown.Text = "down";
            this.ValDown.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(356, 181);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(55, 51);
            this.button5.TabIndex = 19;
            this.button5.Text = "Finish";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // StructListBox
            // 
            this.StructListBox.FormattingEnabled = true;
            this.StructListBox.Location = new System.Drawing.Point(359, 21);
            this.StructListBox.Name = "StructListBox";
            this.StructListBox.Size = new System.Drawing.Size(120, 147);
            this.StructListBox.TabIndex = 12;
            this.StructListBox.SelectedIndexChanged += new System.EventHandler(this.StructListBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(359, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "ListOfStructs";
            // 
            // AddStruct
            // 
            this.AddStruct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddStruct.Location = new System.Drawing.Point(359, 173);
            this.AddStruct.Name = "AddStruct";
            this.AddStruct.Size = new System.Drawing.Size(120, 23);
            this.AddStruct.TabIndex = 22;
            this.AddStruct.Text = "AddStruct";
            this.AddStruct.UseVisualStyleBackColor = true;
            this.AddStruct.Click += new System.EventHandler(this.AddStruct_Click);
            // 
            // EditStruct
            // 
            this.EditStruct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EditStruct.Location = new System.Drawing.Point(359, 202);
            this.EditStruct.Name = "EditStruct";
            this.EditStruct.Size = new System.Drawing.Size(120, 23);
            this.EditStruct.TabIndex = 22;
            this.EditStruct.Text = "EditStruct";
            this.EditStruct.UseVisualStyleBackColor = true;
            this.EditStruct.Click += new System.EventHandler(this.EditStruct_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 280);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(507, 22);
            this.statusStrip1.TabIndex = 25;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.IndexEditor);
            this.tabControl1.Controls.Add(this.StructEditor);
            this.tabControl1.Location = new System.Drawing.Point(12, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(493, 269);
            this.tabControl1.TabIndex = 26;
            // 
            // IndexEditor
            // 
            this.IndexEditor.Controls.Add(this.btnGenerateProxy);
            this.IndexEditor.Controls.Add(this.btnDeleteSub);
            this.IndexEditor.Controls.Add(this.btnDeleteVal);
            this.IndexEditor.Controls.Add(this.button3);
            this.IndexEditor.Controls.Add(this.ValListBox);
            this.IndexEditor.Controls.Add(this.SubListBox);
            this.IndexEditor.Controls.Add(this.label2);
            this.IndexEditor.Controls.Add(this.label3);
            this.IndexEditor.Controls.Add(this.AddSub);
            this.IndexEditor.Controls.Add(this.btnEditSub);
            this.IndexEditor.Controls.Add(this.button5);
            this.IndexEditor.Controls.Add(this.btnEditVal);
            this.IndexEditor.Controls.Add(this.AddVal);
            this.IndexEditor.Location = new System.Drawing.Point(4, 22);
            this.IndexEditor.Name = "IndexEditor";
            this.IndexEditor.Padding = new System.Windows.Forms.Padding(3);
            this.IndexEditor.Size = new System.Drawing.Size(485, 243);
            this.IndexEditor.TabIndex = 0;
            this.IndexEditor.Text = "IndexEditor";
            this.IndexEditor.UseVisualStyleBackColor = true;
            // 
            // btnDeleteSub
            // 
            this.btnDeleteSub.Location = new System.Drawing.Point(77, 209);
            this.btnDeleteSub.Name = "btnDeleteSub";
            this.btnDeleteSub.Size = new System.Drawing.Size(46, 23);
            this.btnDeleteSub.TabIndex = 25;
            this.btnDeleteSub.Text = "Delete";
            this.btnDeleteSub.UseVisualStyleBackColor = true;
            this.btnDeleteSub.Click += new System.EventHandler(this.btnDeleteSub_Click);
            // 
            // btnDeleteVal
            // 
            this.btnDeleteVal.Location = new System.Drawing.Point(223, 209);
            this.btnDeleteVal.Name = "btnDeleteVal";
            this.btnDeleteVal.Size = new System.Drawing.Size(46, 23);
            this.btnDeleteVal.TabIndex = 24;
            this.btnDeleteVal.Text = "Delete";
            this.btnDeleteVal.UseVisualStyleBackColor = true;
            this.btnDeleteVal.Click += new System.EventHandler(this.btnDeleteVal_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(275, 180);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 50);
            this.button3.TabIndex = 23;
            this.button3.Text = "MetaGlob Summary";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // StructEditor
            // 
            this.StructEditor.Controls.Add(this.StructElemListBox1);
            this.StructEditor.Controls.Add(this.label1);
            this.StructEditor.Controls.Add(this.btnEditStructVal);
            this.StructEditor.Controls.Add(this.button2);
            this.StructEditor.Controls.Add(this.StructListBox);
            this.StructEditor.Controls.Add(this.label4);
            this.StructEditor.Controls.Add(this.EditStruct);
            this.StructEditor.Controls.Add(this.AddStruct);
            this.StructEditor.Location = new System.Drawing.Point(4, 22);
            this.StructEditor.Name = "StructEditor";
            this.StructEditor.Padding = new System.Windows.Forms.Padding(3);
            this.StructEditor.Size = new System.Drawing.Size(485, 243);
            this.StructEditor.TabIndex = 1;
            this.StructEditor.Text = "StructEditor";
            this.StructEditor.UseVisualStyleBackColor = true;
            // 
            // StructElemListBox1
            // 
            this.StructElemListBox1.FormattingEnabled = true;
            this.StructElemListBox1.Location = new System.Drawing.Point(6, 21);
            this.StructElemListBox1.Name = "StructElemListBox1";
            this.StructElemListBox1.Size = new System.Drawing.Size(347, 147);
            this.StructElemListBox1.TabIndex = 23;
            this.StructElemListBox1.SelectedIndexChanged += new System.EventHandler(this.StructElemListBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Elements of Struct";
            // 
            // btnEditStructVal
            // 
            this.btnEditStructVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditStructVal.Location = new System.Drawing.Point(213, 202);
            this.btnEditStructVal.Name = "btnEditStructVal";
            this.btnEditStructVal.Size = new System.Drawing.Size(140, 23);
            this.btnEditStructVal.TabIndex = 25;
            this.btnEditStructVal.Text = "EditValue";
            this.btnEditStructVal.UseVisualStyleBackColor = true;
            this.btnEditStructVal.Click += new System.EventHandler(this.btnEditStructVal_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(213, 173);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 23);
            this.button2.TabIndex = 26;
            this.button2.Text = "AddValue";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnGenerateProxy
            // 
            this.btnGenerateProxy.Location = new System.Drawing.Point(417, 181);
            this.btnGenerateProxy.Name = "btnGenerateProxy";
            this.btnGenerateProxy.Size = new System.Drawing.Size(62, 51);
            this.btnGenerateProxy.TabIndex = 26;
            this.btnGenerateProxy.Text = "Generate Proxies";
            this.btnGenerateProxy.UseVisualStyleBackColor = true;
            this.btnGenerateProxy.Click += new System.EventHandler(this.btnGenerateProxy_Click);
            // 
            // GlobMetaEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 302);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ValDown);
            this.Controls.Add(this.SubDown);
            this.Controls.Add(this.ValUp);
            this.Controls.Add(this.subUp);
            this.Name = "GlobMetaEdit";
            this.Text = "<GlobName>";
            this.Load += new System.EventHandler(this.GlobMetaEdit_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.IndexEditor.ResumeLayout(false);
            this.IndexEditor.PerformLayout();
            this.StructEditor.ResumeLayout(false);
            this.StructEditor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox SubListBox;
        private System.Windows.Forms.Button AddSub;
        private System.Windows.Forms.Button btnEditSub;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox ValListBox;
        private System.Windows.Forms.Button AddVal;
        private System.Windows.Forms.Button btnEditVal;
        private System.Windows.Forms.Button subUp;
        private System.Windows.Forms.Button SubDown;
        private System.Windows.Forms.Button ValUp;
        private System.Windows.Forms.Button ValDown;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ListBox StructListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AddStruct;
        private System.Windows.Forms.Button EditStruct;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage IndexEditor;
        private System.Windows.Forms.TabPage StructEditor;
        private System.Windows.Forms.ListBox StructElemListBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEditStructVal;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnDeleteVal;
        private System.Windows.Forms.Button btnDeleteSub;
        private System.Windows.Forms.Button btnGenerateProxy;
    }
}