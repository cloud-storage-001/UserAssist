namespace UserAssist
{
    partial class frmUserAssist
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUserAssist));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.explainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.commandsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromRegistryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromREGFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromDATFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highlightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAtStartupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.AccessibleDescription = "?";
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.explainToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.clearToolStripMenuItem.Text = "清除";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // explainToolStripMenuItem
            // 
            this.explainToolStripMenuItem.Name = "explainToolStripMenuItem";
            this.explainToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.explainToolStripMenuItem.Text = "说明";
            this.explainToolStripMenuItem.Click += new System.EventHandler(this.explainToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 25);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(692, 314);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "键";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "索引";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "名字";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "计数";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "最后访问时间";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commandsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(692, 25);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "菜单";
            // 
            // commandsToolStripMenuItem
            // 
            this.commandsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromRegistryToolStripMenuItem,
            this.loadFromREGFileToolStripMenuItem,
            this.loadFromDATFileToolStripMenuItem,
            this.highlightToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.clearAllToolStripMenuItem,
            this.loggingToolStripMenuItem,
            this.loadAtStartupToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.commandsToolStripMenuItem.Name = "commandsToolStripMenuItem";
            this.commandsToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.commandsToolStripMenuItem.Text = "命令";
            // 
            // loadFromRegistryToolStripMenuItem
            // 
            this.loadFromRegistryToolStripMenuItem.Name = "loadFromRegistryToolStripMenuItem";
            this.loadFromRegistryToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.loadFromRegistryToolStripMenuItem.Text = "从本地注册表加载";
            this.loadFromRegistryToolStripMenuItem.Click += new System.EventHandler(this.loadFromRegistryToolStripMenuItem_Click);
            // 
            // loadFromREGFileToolStripMenuItem
            // 
            this.loadFromREGFileToolStripMenuItem.Name = "loadFromREGFileToolStripMenuItem";
            this.loadFromREGFileToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.loadFromREGFileToolStripMenuItem.Text = "从REG文件加载";
            this.loadFromREGFileToolStripMenuItem.Click += new System.EventHandler(this.loadFromREGFileToolStripMenuItem_Click);
            // 
            // loadFromDATFileToolStripMenuItem
            // 
            this.loadFromDATFileToolStripMenuItem.Name = "loadFromDATFileToolStripMenuItem";
            this.loadFromDATFileToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.loadFromDATFileToolStripMenuItem.Text = "从DAT文件加载（实验性）";
            this.loadFromDATFileToolStripMenuItem.Click += new System.EventHandler(this.loadFromDATFileToolStripMenuItem_Click);
            // 
            // highlightToolStripMenuItem
            // 
            this.highlightToolStripMenuItem.Name = "highlightToolStripMenuItem";
            this.highlightToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.highlightToolStripMenuItem.Text = "高亮";
            this.highlightToolStripMenuItem.Click += new System.EventHandler(this.highlightToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.saveToolStripMenuItem.Text = "保存";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.clearAllToolStripMenuItem.Text = "全部清除";
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllToolStripMenuItem_Click);
            // 
            // loggingToolStripMenuItem
            // 
            this.loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            this.loggingToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.loggingToolStripMenuItem.Text = "日志已禁用";
            this.loggingToolStripMenuItem.Click += new System.EventHandler(this.loggingToolStripMenuItem_Click);
            // 
            // loadAtStartupToolStripMenuItem
            // 
            this.loadAtStartupToolStripMenuItem.Name = "loadAtStartupToolStripMenuItem";
            this.loadAtStartupToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.loadAtStartupToolStripMenuItem.Text = "启动时加载";
            this.loadAtStartupToolStripMenuItem.Click += new System.EventHandler(this.loadAtStartupToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exitToolStripMenuItem.Text = "退出";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abotToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.helpToolStripMenuItem.Text = "帮助";
            // 
            // abotToolStripMenuItem
            // 
            this.abotToolStripMenuItem.Name = "abotToolStripMenuItem";
            this.abotToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.abotToolStripMenuItem.Text = "关于";
            this.abotToolStripMenuItem.Click += new System.EventHandler(this.abotToolStripMenuItem_Click);
            // 
            // frmUserAssist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 338);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.listView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmUserAssist";
            this.Text = "UserAssist";
            this.Load += new System.EventHandler(this.frmUserAssist_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem commandsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromRegistryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromREGFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highlightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromDATFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem explainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAtStartupToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
    }
}

