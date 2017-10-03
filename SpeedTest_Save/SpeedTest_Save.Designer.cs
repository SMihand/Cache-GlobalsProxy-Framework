namespace EXTREMEAccessTESTS
{
    partial class EXTREMEcomparator
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.probeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.objectCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nativeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.trueCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressObject = new System.Windows.Forms.ToolStripProgressBar();
            this.progressNative = new System.Windows.Forms.ToolStripProgressBar();
            this.progressTrue = new System.Windows.Forms.ToolStripProgressBar();
            this.progressClear = new System.Windows.Forms.ToolStripProgressBar();
            this.btnValidation = new System.Windows.Forms.ToolStripStatusLabel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.probeCol,
            this.objectCol,
            this.nativeCol,
            this.trueCol});
            this.dataGridView.Location = new System.Drawing.Point(12, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(1103, 242);
            this.dataGridView.TabIndex = 0;
            // 
            // probeCol
            // 
            this.probeCol.HeaderText = "probeNum";
            this.probeCol.Name = "probeCol";
            // 
            // objectCol
            // 
            this.objectCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.objectCol.HeaderText = "object(milisec)";
            this.objectCol.Name = "objectCol";
            // 
            // nativeCol
            // 
            this.nativeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nativeCol.HeaderText = "native(milisec)";
            this.nativeCol.Name = "nativeCol";
            // 
            // trueCol
            // 
            this.trueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.trueCol.HeaderText = "true(milisec)";
            this.trueCol.Name = "trueCol";
            // 
            // txtInfo
            // 
            this.txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInfo.Location = new System.Drawing.Point(12, 262);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(937, 20);
            this.txtInfo.TabIndex = 2;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(1032, 260);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(83, 23);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "TestAgain";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressObject,
            this.progressNative,
            this.progressTrue,
            this.progressClear,
            this.btnValidation});
            this.statusStrip.Location = new System.Drawing.Point(0, 291);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1127, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // progressObject
            // 
            this.progressObject.Name = "progressObject";
            this.progressObject.Size = new System.Drawing.Size(400, 16);
            // 
            // progressNative
            // 
            this.progressNative.Name = "progressNative";
            this.progressNative.Size = new System.Drawing.Size(400, 16);
            this.progressNative.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // progressTrue
            // 
            this.progressTrue.Name = "progressTrue";
            this.progressTrue.Size = new System.Drawing.Size(100, 16);
            // 
            // progressClear
            // 
            this.progressClear.Name = "progressClear";
            this.progressClear.Size = new System.Drawing.Size(120, 16);
            // 
            // btnValidation
            // 
            this.btnValidation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnValidation.Name = "btnValidation";
            this.btnValidation.Size = new System.Drawing.Size(76, 17);
            this.btnValidation.Text = "OFF validation";
            this.btnValidation.Click += new System.EventHandler(this.btnValidation_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDown1.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(955, 262);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            2000000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(71, 20);
            this.numericUpDown1.TabIndex = 5;
            this.numericUpDown1.Value = new decimal(new int[] {
            70000,
            0,
            0,
            0});
            // 
            // EXTREMEcomparator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 313);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.dataGridView);
            this.Name = "EXTREMEcomparator";
            this.Text = "Compararor extreme";
            this.Load += new System.EventHandler(this.EXTREMEcomparator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar progressNative;
        private System.Windows.Forms.ToolStripProgressBar progressTrue;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ToolStripProgressBar progressClear;
        private System.Windows.Forms.ToolStripProgressBar progressObject;
        private System.Windows.Forms.DataGridViewTextBoxColumn probeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn objectCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn nativeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn trueCol;
        private System.Windows.Forms.ToolStripStatusLabel btnValidation;
    }
}

