namespace Komin
{
    partial class KominServerForm
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.localstart = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SummaryPage = new System.Windows.Forms.TabPage();
            this.grouplist = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.userlist = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connections_count = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LogPage = new System.Windows.Forms.TabPage();
            this.savelog = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.logbox = new System.Windows.Forms.TextBox();
            this.SQLExecPage = new System.Windows.Forms.TabPage();
            this.sqlReaderOutput = new System.Windows.Forms.ListView();
            this.readerexec = new System.Windows.Forms.CheckBox();
            this.sqlExec = new System.Windows.Forms.Button();
            this.sqlCommandText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.SummaryPage.SuspendLayout();
            this.LogPage.SuspendLayout();
            this.SQLExecPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(61, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "host IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "port:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(222, 12);
            this.textBox1.MaxLength = 5;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(328, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.onStartStopListening);
            // 
            // localstart
            // 
            this.localstart.Location = new System.Drawing.Point(409, 10);
            this.localstart.Name = "localstart";
            this.localstart.Size = new System.Drawing.Size(107, 23);
            this.localstart.TabIndex = 5;
            this.localstart.Text = "start at localhost";
            this.localstart.UseVisualStyleBackColor = true;
            this.localstart.Click += new System.EventHandler(this.onLocalhostStart);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SummaryPage);
            this.tabControl1.Controls.Add(this.LogPage);
            this.tabControl1.Controls.Add(this.SQLExecPage);
            this.tabControl1.Location = new System.Drawing.Point(12, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1122, 506);
            this.tabControl1.TabIndex = 6;
            // 
            // SummaryPage
            // 
            this.SummaryPage.Controls.Add(this.button3);
            this.SummaryPage.Controls.Add(this.grouplist);
            this.SummaryPage.Controls.Add(this.userlist);
            this.SummaryPage.Controls.Add(this.connections_count);
            this.SummaryPage.Controls.Add(this.label3);
            this.SummaryPage.Location = new System.Drawing.Point(4, 22);
            this.SummaryPage.Name = "SummaryPage";
            this.SummaryPage.Padding = new System.Windows.Forms.Padding(3);
            this.SummaryPage.Size = new System.Drawing.Size(1114, 480);
            this.SummaryPage.TabIndex = 0;
            this.SummaryPage.Text = "Summary";
            this.SummaryPage.UseVisualStyleBackColor = true;
            // 
            // grouplist
            // 
            this.grouplist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader5,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader6});
            this.grouplist.FullRowSelect = true;
            this.grouplist.GridLines = true;
            this.grouplist.Location = new System.Drawing.Point(506, 23);
            this.grouplist.Name = "grouplist";
            this.grouplist.Size = new System.Drawing.Size(512, 451);
            this.grouplist.TabIndex = 3;
            this.grouplist.UseCompatibleStateImageBehavior = false;
            this.grouplist.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "ID";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Group";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Group Holder";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Group Caps.";
            this.columnHeader10.Width = 71;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Memberslist";
            this.columnHeader6.Width = 229;
            // 
            // userlist
            // 
            this.userlist.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.userlist.FullRowSelect = true;
            this.userlist.GridLines = true;
            this.userlist.Location = new System.Drawing.Point(6, 23);
            this.userlist.Name = "userlist";
            this.userlist.Size = new System.Drawing.Size(494, 451);
            this.userlist.TabIndex = 2;
            this.userlist.UseCompatibleStateImageBehavior = false;
            this.userlist.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ID";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Login";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "State";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Friendlist";
            this.columnHeader3.Width = 143;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Groups";
            this.columnHeader4.Width = 211;
            // 
            // connections_count
            // 
            this.connections_count.AutoSize = true;
            this.connections_count.Location = new System.Drawing.Point(81, 7);
            this.connections_count.Name = "connections_count";
            this.connections_count.Size = new System.Drawing.Size(0, 13);
            this.connections_count.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "connections:";
            // 
            // LogPage
            // 
            this.LogPage.Controls.Add(this.savelog);
            this.LogPage.Controls.Add(this.button2);
            this.LogPage.Controls.Add(this.logbox);
            this.LogPage.Location = new System.Drawing.Point(4, 22);
            this.LogPage.Name = "LogPage";
            this.LogPage.Padding = new System.Windows.Forms.Padding(3);
            this.LogPage.Size = new System.Drawing.Size(1114, 480);
            this.LogPage.TabIndex = 1;
            this.LogPage.Text = "Log";
            this.LogPage.UseVisualStyleBackColor = true;
            // 
            // savelog
            // 
            this.savelog.Enabled = false;
            this.savelog.Location = new System.Drawing.Point(952, 451);
            this.savelog.Name = "savelog";
            this.savelog.Size = new System.Drawing.Size(75, 23);
            this.savelog.TabIndex = 2;
            this.savelog.Text = "save to file";
            this.savelog.UseVisualStyleBackColor = true;
            this.savelog.Click += new System.EventHandler(this.savelog_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(1033, 451);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "clear log";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.onClearLog);
            // 
            // logbox
            // 
            this.logbox.HideSelection = false;
            this.logbox.Location = new System.Drawing.Point(6, 6);
            this.logbox.Multiline = true;
            this.logbox.Name = "logbox";
            this.logbox.ReadOnly = true;
            this.logbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logbox.Size = new System.Drawing.Size(1102, 439);
            this.logbox.TabIndex = 0;
            this.logbox.TextChanged += new System.EventHandler(this.logbox_TextChanged);
            // 
            // SQLExecPage
            // 
            this.SQLExecPage.Controls.Add(this.sqlReaderOutput);
            this.SQLExecPage.Controls.Add(this.readerexec);
            this.SQLExecPage.Controls.Add(this.sqlExec);
            this.SQLExecPage.Controls.Add(this.sqlCommandText);
            this.SQLExecPage.Controls.Add(this.label4);
            this.SQLExecPage.Location = new System.Drawing.Point(4, 22);
            this.SQLExecPage.Name = "SQLExecPage";
            this.SQLExecPage.Padding = new System.Windows.Forms.Padding(3);
            this.SQLExecPage.Size = new System.Drawing.Size(1114, 480);
            this.SQLExecPage.TabIndex = 2;
            this.SQLExecPage.Text = "Execute SQL";
            this.SQLExecPage.UseVisualStyleBackColor = true;
            // 
            // sqlReaderOutput
            // 
            this.sqlReaderOutput.Enabled = false;
            this.sqlReaderOutput.FullRowSelect = true;
            this.sqlReaderOutput.GridLines = true;
            this.sqlReaderOutput.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.sqlReaderOutput.Location = new System.Drawing.Point(12, 34);
            this.sqlReaderOutput.Name = "sqlReaderOutput";
            this.sqlReaderOutput.Size = new System.Drawing.Size(1096, 440);
            this.sqlReaderOutput.TabIndex = 4;
            this.sqlReaderOutput.UseCompatibleStateImageBehavior = false;
            this.sqlReaderOutput.View = System.Windows.Forms.View.Details;
            // 
            // readerexec
            // 
            this.readerexec.AutoSize = true;
            this.readerexec.Location = new System.Drawing.Point(957, 10);
            this.readerexec.Name = "readerexec";
            this.readerexec.Size = new System.Drawing.Size(70, 17);
            this.readerexec.TabIndex = 2;
            this.readerexec.Text = "as reader";
            this.readerexec.UseVisualStyleBackColor = true;
            this.readerexec.CheckedChanged += new System.EventHandler(this.readerexec_CheckedChanged);
            // 
            // sqlExec
            // 
            this.sqlExec.Location = new System.Drawing.Point(1033, 6);
            this.sqlExec.Name = "sqlExec";
            this.sqlExec.Size = new System.Drawing.Size(75, 23);
            this.sqlExec.TabIndex = 3;
            this.sqlExec.Text = "Execute";
            this.sqlExec.UseVisualStyleBackColor = true;
            this.sqlExec.Click += new System.EventHandler(this.sqlExec_Click);
            // 
            // sqlCommandText
            // 
            this.sqlCommandText.Location = new System.Drawing.Point(72, 8);
            this.sqlCommandText.Name = "sqlCommandText";
            this.sqlCommandText.Size = new System.Drawing.Size(879, 20);
            this.sqlCommandText.TabIndex = 1;
            this.sqlCommandText.TextChanged += new System.EventHandler(this.sqlCommandText_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Command:";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.onLogTimerUpdate);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1033, 451);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "refresh";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.timer1_Tick);
            // 
            // KominServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1146, 557);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.localstart);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "KominServerForm";
            this.Text = "Komin server";
            this.Load += new System.EventHandler(this.onFormLoad);
            this.tabControl1.ResumeLayout(false);
            this.SummaryPage.ResumeLayout(false);
            this.SummaryPage.PerformLayout();
            this.LogPage.ResumeLayout(false);
            this.LogPage.PerformLayout();
            this.SQLExecPage.ResumeLayout(false);
            this.SQLExecPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button localstart;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage SummaryPage;
        private System.Windows.Forms.Label connections_count;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage LogPage;
        private System.Windows.Forms.ListView grouplist;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ListView userlist;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox logbox;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.TabPage SQLExecPage;
        private System.Windows.Forms.ListView sqlReaderOutput;
        private System.Windows.Forms.CheckBox readerexec;
        private System.Windows.Forms.Button sqlExec;
        private System.Windows.Forms.TextBox sqlCommandText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button savelog;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.Button button3;

    }
}

