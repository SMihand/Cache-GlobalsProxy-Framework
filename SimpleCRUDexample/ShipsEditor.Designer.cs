namespace SimpleCRUDexample
{
    partial class ShipsEditor
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
            this.grdEntities = new System.Windows.Forms.DataGridView();
            this.colNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCountry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCaptain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCapacity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEff = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.txtCountry = new System.Windows.Forms.TextBox();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.numRank = new System.Windows.Forms.NumericUpDown();
            this.txtShipName = new System.Windows.Forms.TextBox();
            this.numStuffCount = new System.Windows.Forms.NumericUpDown();
            this.numEff = new System.Windows.Forms.NumericUpDown();
            this.btnKill = new System.Windows.Forms.Button();
            this.btnClearKey = new System.Windows.Forms.Button();
            this.txtCaptainName = new System.Windows.Forms.TextBox();
            this.btnGetAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdEntities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStuffCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEff)).BeginInit();
            this.SuspendLayout();
            // 
            // grdEntities
            // 
            this.grdEntities.AllowUserToAddRows = false;
            this.grdEntities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdEntities.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNum,
            this.colCountry,
            this.colClass,
            this.colRank,
            this.colName,
            this.colCaptain,
            this.colCapacity,
            this.colEff});
            this.grdEntities.Location = new System.Drawing.Point(12, 72);
            this.grdEntities.MultiSelect = false;
            this.grdEntities.Name = "grdEntities";
            this.grdEntities.ReadOnly = true;
            this.grdEntities.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdEntities.Size = new System.Drawing.Size(674, 222);
            this.grdEntities.TabIndex = 0;
            this.grdEntities.SelectionChanged += new System.EventHandler(this.grdEntities_SelectionChanged);
            // 
            // colNum
            // 
            this.colNum.HeaderText = "N";
            this.colNum.Name = "colNum";
            this.colNum.ReadOnly = true;
            this.colNum.Width = 90;
            // 
            // colCountry
            // 
            this.colCountry.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCountry.HeaderText = "Country";
            this.colCountry.Name = "colCountry";
            this.colCountry.ReadOnly = true;
            // 
            // colClass
            // 
            this.colClass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colClass.HeaderText = "Class";
            this.colClass.Name = "colClass";
            this.colClass.ReadOnly = true;
            // 
            // colRank
            // 
            this.colRank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRank.HeaderText = "Rank";
            this.colRank.Name = "colRank";
            this.colRank.ReadOnly = true;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colCaptain
            // 
            this.colCaptain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCaptain.HeaderText = "Captain";
            this.colCaptain.Name = "colCaptain";
            this.colCaptain.ReadOnly = true;
            // 
            // colCapacity
            // 
            this.colCapacity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCapacity.HeaderText = "Capacity";
            this.colCapacity.Name = "colCapacity";
            this.colCapacity.ReadOnly = true;
            // 
            // colEff
            // 
            this.colEff.HeaderText = "Efficienty";
            this.colEff.Name = "colEff";
            this.colEff.ReadOnly = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(452, 326);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(169, 31);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(535, 25);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(54, 29);
            this.btnGet.TabIndex = 4;
            this.btnGet.Text = "Get";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // txtCountry
            // 
            this.txtCountry.Location = new System.Drawing.Point(12, 29);
            this.txtCountry.Name = "txtCountry";
            this.txtCountry.Size = new System.Drawing.Size(172, 20);
            this.txtCountry.TabIndex = 5;
            // 
            // txtClass
            // 
            this.txtClass.Location = new System.Drawing.Point(190, 30);
            this.txtClass.Name = "txtClass";
            this.txtClass.Size = new System.Drawing.Size(172, 20);
            this.txtClass.TabIndex = 6;
            // 
            // numRank
            // 
            this.numRank.Location = new System.Drawing.Point(368, 29);
            this.numRank.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numRank.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numRank.Name = "numRank";
            this.numRank.Size = new System.Drawing.Size(56, 20);
            this.numRank.TabIndex = 9;
            // 
            // txtShipName
            // 
            this.txtShipName.Location = new System.Drawing.Point(430, 29);
            this.txtShipName.Name = "txtShipName";
            this.txtShipName.Size = new System.Drawing.Size(99, 20);
            this.txtShipName.TabIndex = 11;
            // 
            // numStuffCount
            // 
            this.numStuffCount.Location = new System.Drawing.Point(238, 332);
            this.numStuffCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numStuffCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numStuffCount.Name = "numStuffCount";
            this.numStuffCount.Size = new System.Drawing.Size(120, 20);
            this.numStuffCount.TabIndex = 12;
            this.numStuffCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numEff
            // 
            this.numEff.DecimalPlaces = 2;
            this.numEff.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numEff.Location = new System.Drawing.Point(378, 332);
            this.numEff.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numEff.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.numEff.Name = "numEff";
            this.numEff.Size = new System.Drawing.Size(68, 20);
            this.numEff.TabIndex = 13;
            // 
            // btnKill
            // 
            this.btnKill.Location = new System.Drawing.Point(627, 326);
            this.btnKill.Name = "btnKill";
            this.btnKill.Size = new System.Drawing.Size(59, 31);
            this.btnKill.TabIndex = 14;
            this.btnKill.Text = "Kill";
            this.btnKill.UseVisualStyleBackColor = true;
            this.btnKill.Click += new System.EventHandler(this.btnKill_Click);
            // 
            // btnClearKey
            // 
            this.btnClearKey.Location = new System.Drawing.Point(628, 23);
            this.btnClearKey.Name = "btnClearKey";
            this.btnClearKey.Size = new System.Drawing.Size(58, 31);
            this.btnClearKey.TabIndex = 15;
            this.btnClearKey.Text = "ClearKey";
            this.btnClearKey.UseVisualStyleBackColor = true;
            this.btnClearKey.Click += new System.EventHandler(this.btnClearKey_Click);
            // 
            // txtCaptainName
            // 
            this.txtCaptainName.Location = new System.Drawing.Point(62, 332);
            this.txtCaptainName.Name = "txtCaptainName";
            this.txtCaptainName.Size = new System.Drawing.Size(170, 20);
            this.txtCaptainName.TabIndex = 16;
            // 
            // btnGetAll
            // 
            this.btnGetAll.Location = new System.Drawing.Point(595, 25);
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.Size = new System.Drawing.Size(27, 29);
            this.btnGetAll.TabIndex = 17;
            this.btnGetAll.Text = "All";
            this.btnGetAll.UseVisualStyleBackColor = true;
            this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
            // 
            // ShipsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 360);
            this.Controls.Add(this.btnGetAll);
            this.Controls.Add(this.txtCaptainName);
            this.Controls.Add(this.btnClearKey);
            this.Controls.Add(this.btnKill);
            this.Controls.Add(this.numEff);
            this.Controls.Add(this.numStuffCount);
            this.Controls.Add(this.txtShipName);
            this.Controls.Add(this.numRank);
            this.Controls.Add(this.txtClass);
            this.Controls.Add(this.txtCountry);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grdEntities);
            this.Name = "ShipsEditor";
            this.Text = "ShipsEditor";
            this.Load += new System.EventHandler(this.CRUDExample_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdEntities)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStuffCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEff)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.TextBox txtCountry;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.NumericUpDown numRank;
        private System.Windows.Forms.TextBox txtShipName;
        private System.Windows.Forms.NumericUpDown numStuffCount;
        private System.Windows.Forms.NumericUpDown numEff;
        private System.Windows.Forms.Button btnKill;
        private System.Windows.Forms.Button btnClearKey;
        private System.Windows.Forms.DataGridView grdEntities;
        private System.Windows.Forms.TextBox txtCaptainName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCountry;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCaptain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCapacity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEff;
        private System.Windows.Forms.Button btnGetAll;
    }
}