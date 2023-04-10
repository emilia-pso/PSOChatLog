
namespace PSOChatLog
{
    partial class frmChatDetail
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
            this.cmdALL = new System.Windows.Forms.Button();
            this.txtRow = new System.Windows.Forms.TextBox();
            this.txtALL = new System.Windows.Forms.TextBox();
            this.cmdID = new System.Windows.Forms.Button();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cmdName = new System.Windows.Forms.Button();
            this.btnUpMax = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnDownMax = new System.Windows.Forms.Button();
            this.cmdText = new System.Windows.Forms.Button();
            this.txtText = new System.Windows.Forms.TextBox();
            this.lblRow = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.lblSharp = new System.Windows.Forms.Label();
            this.txtNickName = new System.Windows.Forms.TextBox();
            this.lblNickName = new System.Windows.Forms.Label();
            this.txtInputID = new System.Windows.Forms.TextBox();
            this.btnIDSerch = new System.Windows.Forms.Button();
            this.lstCharacterList = new System.Windows.Forms.ListBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.btnLogAnalysis = new System.Windows.Forms.Button();
            this.lblCharacterList = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnSTOP = new System.Windows.Forms.Button();
            this.cboSelectAnalysis = new System.Windows.Forms.ComboBox();
            this.NICT = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // cmdALL
            // 
            this.cmdALL.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmdALL.Location = new System.Drawing.Point(12, 47);
            this.cmdALL.Name = "cmdALL";
            this.cmdALL.Size = new System.Drawing.Size(75, 34);
            this.cmdALL.TabIndex = 10;
            this.cmdALL.Text = "全文";
            this.cmdALL.UseVisualStyleBackColor = true;
            this.cmdALL.Click += new System.EventHandler(this.cmdALL_Click);
            // 
            // txtRow
            // 
            this.txtRow.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtRow.Location = new System.Drawing.Point(93, 12);
            this.txtRow.Name = "txtRow";
            this.txtRow.Size = new System.Drawing.Size(120, 31);
            this.txtRow.TabIndex = 9;
            // 
            // txtALL
            // 
            this.txtALL.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtALL.Location = new System.Drawing.Point(93, 49);
            this.txtALL.Name = "txtALL";
            this.txtALL.Size = new System.Drawing.Size(609, 31);
            this.txtALL.TabIndex = 11;
            // 
            // cmdID
            // 
            this.cmdID.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmdID.Location = new System.Drawing.Point(12, 84);
            this.cmdID.Name = "cmdID";
            this.cmdID.Size = new System.Drawing.Size(75, 34);
            this.cmdID.TabIndex = 12;
            this.cmdID.Text = "ID";
            this.cmdID.UseVisualStyleBackColor = true;
            this.cmdID.Click += new System.EventHandler(this.cmdID_Click);
            // 
            // txtID
            // 
            this.txtID.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtID.Location = new System.Drawing.Point(93, 86);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(120, 31);
            this.txtID.TabIndex = 13;
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtName.Location = new System.Drawing.Point(93, 123);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(609, 31);
            this.txtName.TabIndex = 15;
            // 
            // cmdName
            // 
            this.cmdName.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmdName.Location = new System.Drawing.Point(12, 121);
            this.cmdName.Name = "cmdName";
            this.cmdName.Size = new System.Drawing.Size(75, 34);
            this.cmdName.TabIndex = 14;
            this.cmdName.Text = "名前";
            this.cmdName.UseVisualStyleBackColor = true;
            this.cmdName.Click += new System.EventHandler(this.cmdName_Click);
            // 
            // btnUpMax
            // 
            this.btnUpMax.Font = new System.Drawing.Font("MS UI Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUpMax.Location = new System.Drawing.Point(708, 1);
            this.btnUpMax.Name = "btnUpMax";
            this.btnUpMax.Size = new System.Drawing.Size(75, 75);
            this.btnUpMax.TabIndex = 21;
            this.btnUpMax.Text = "▲";
            this.btnUpMax.UseVisualStyleBackColor = true;
            this.btnUpMax.Click += new System.EventHandler(this.btnUpMax_Click);
            // 
            // btnUp
            // 
            this.btnUp.Font = new System.Drawing.Font("MS UI Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUp.Location = new System.Drawing.Point(708, 76);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 205);
            this.btnUp.TabIndex = 22;
            this.btnUp.Text = "△";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Font = new System.Drawing.Font("MS UI Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDown.Location = new System.Drawing.Point(708, 281);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 205);
            this.btnDown.TabIndex = 23;
            this.btnDown.Text = "▽";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnDownMax
            // 
            this.btnDownMax.Font = new System.Drawing.Font("MS UI Gothic", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnDownMax.Location = new System.Drawing.Point(708, 486);
            this.btnDownMax.Name = "btnDownMax";
            this.btnDownMax.Size = new System.Drawing.Size(75, 75);
            this.btnDownMax.TabIndex = 24;
            this.btnDownMax.Text = "▼";
            this.btnDownMax.UseVisualStyleBackColor = true;
            this.btnDownMax.Click += new System.EventHandler(this.btnDownMax_Click);
            // 
            // cmdText
            // 
            this.cmdText.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmdText.Location = new System.Drawing.Point(12, 158);
            this.cmdText.Name = "cmdText";
            this.cmdText.Size = new System.Drawing.Size(75, 34);
            this.cmdText.TabIndex = 16;
            this.cmdText.Text = "会話";
            this.cmdText.UseVisualStyleBackColor = true;
            this.cmdText.Click += new System.EventHandler(this.cmdText_Click);
            // 
            // txtText
            // 
            this.txtText.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtText.Location = new System.Drawing.Point(93, 160);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(609, 31);
            this.txtText.TabIndex = 17;
            // 
            // lblRow
            // 
            this.lblRow.AutoSize = true;
            this.lblRow.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRow.Location = new System.Drawing.Point(12, 15);
            this.lblRow.Name = "lblRow";
            this.lblRow.Size = new System.Drawing.Size(34, 24);
            this.lblRow.TabIndex = 8;
            this.lblRow.Text = "行";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(12, 199);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(160, 16);
            this.radioButton1.TabIndex = 19;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "選択された行のIDを表示する";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(12, 221);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(152, 16);
            this.radioButton2.TabIndex = 20;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "入力したIDを検索表示する";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // lblSharp
            // 
            this.lblSharp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSharp.Location = new System.Drawing.Point(10, 195);
            this.lblSharp.Name = "lblSharp";
            this.lblSharp.Size = new System.Drawing.Size(692, 1);
            this.lblSharp.TabIndex = 18;
            this.lblSharp.Text = "label1";
            // 
            // txtNickName
            // 
            this.txtNickName.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtNickName.Location = new System.Drawing.Point(144, 278);
            this.txtNickName.Name = "txtNickName";
            this.txtNickName.Size = new System.Drawing.Size(558, 31);
            this.txtNickName.TabIndex = 3;
            this.txtNickName.Leave += new System.EventHandler(this.txtNickName_Leave);
            // 
            // lblNickName
            // 
            this.lblNickName.AutoSize = true;
            this.lblNickName.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblNickName.Location = new System.Drawing.Point(12, 281);
            this.lblNickName.Name = "lblNickName";
            this.lblNickName.Size = new System.Drawing.Size(119, 24);
            this.lblNickName.TabIndex = 2;
            this.lblNickName.Text = "ニックネーム";
            // 
            // txtInputID
            // 
            this.txtInputID.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtInputID.Location = new System.Drawing.Point(10, 242);
            this.txtInputID.Name = "txtInputID";
            this.txtInputID.Size = new System.Drawing.Size(120, 31);
            this.txtInputID.TabIndex = 0;
            // 
            // btnIDSerch
            // 
            this.btnIDSerch.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnIDSerch.Location = new System.Drawing.Point(144, 240);
            this.btnIDSerch.Name = "btnIDSerch";
            this.btnIDSerch.Size = new System.Drawing.Size(150, 34);
            this.btnIDSerch.TabIndex = 1;
            this.btnIDSerch.Text = "ID検索";
            this.btnIDSerch.UseVisualStyleBackColor = true;
            this.btnIDSerch.Click += new System.EventHandler(this.btnIDSerch_Click);
            // 
            // lstCharacterList
            // 
            this.lstCharacterList.FormattingEnabled = true;
            this.lstCharacterList.ItemHeight = 12;
            this.lstCharacterList.Location = new System.Drawing.Point(12, 339);
            this.lstCharacterList.Name = "lstCharacterList";
            this.lstCharacterList.Size = new System.Drawing.Size(370, 208);
            this.lstCharacterList.TabIndex = 7;
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblRemarks.Location = new System.Drawing.Point(384, 312);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(58, 24);
            this.lblRemarks.TabIndex = 4;
            this.lblRemarks.Text = "備考";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtRemarks.Location = new System.Drawing.Point(388, 339);
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(314, 208);
            this.txtRemarks.TabIndex = 5;
            this.txtRemarks.Leave += new System.EventHandler(this.txtRemarks_Leave);
            // 
            // btnLogAnalysis
            // 
            this.btnLogAnalysis.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnLogAnalysis.Location = new System.Drawing.Point(300, 240);
            this.btnLogAnalysis.Name = "btnLogAnalysis";
            this.btnLogAnalysis.Size = new System.Drawing.Size(200, 34);
            this.btnLogAnalysis.TabIndex = 25;
            this.btnLogAnalysis.Text = "チャットログを分析";
            this.btnLogAnalysis.UseVisualStyleBackColor = true;
            this.btnLogAnalysis.Click += new System.EventHandler(this.btnLogAnalysis_Click);
            // 
            // lblCharacterList
            // 
            this.lblCharacterList.AutoSize = true;
            this.lblCharacterList.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCharacterList.Location = new System.Drawing.Point(12, 312);
            this.lblCharacterList.Name = "lblCharacterList";
            this.lblCharacterList.Size = new System.Drawing.Size(166, 24);
            this.lblCharacterList.TabIndex = 6;
            this.lblCharacterList.Text = "キャラクターリスト";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(506, 240);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(196, 33);
            this.progressBar1.TabIndex = 26;
            this.progressBar1.Visible = false;
            // 
            // btnSTOP
            // 
            this.btnSTOP.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnSTOP.Location = new System.Drawing.Point(300, 240);
            this.btnSTOP.Name = "btnSTOP";
            this.btnSTOP.Size = new System.Drawing.Size(200, 34);
            this.btnSTOP.TabIndex = 27;
            this.btnSTOP.Text = "分析を停止";
            this.btnSTOP.UseVisualStyleBackColor = true;
            this.btnSTOP.Visible = false;
            this.btnSTOP.Click += new System.EventHandler(this.btnSTOP_Click);
            // 
            // cboSelectAnalysis
            // 
            this.cboSelectAnalysis.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cboSelectAnalysis.FormattingEnabled = true;
            this.cboSelectAnalysis.Location = new System.Drawing.Point(506, 240);
            this.cboSelectAnalysis.Name = "cboSelectAnalysis";
            this.cboSelectAnalysis.Size = new System.Drawing.Size(196, 32);
            this.cboSelectAnalysis.TabIndex = 28;
            // 
            // NICT
            // 
            this.NICT.AutoSize = true;
            this.NICT.Location = new System.Drawing.Point(608, 12);
            this.NICT.Name = "NICT";
            this.NICT.Size = new System.Drawing.Size(94, 12);
            this.NICT.TabIndex = 29;
            this.NICT.Text = "Powered by NICT";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ChatDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.NICT);
            this.Controls.Add(this.lblCharacterList);
            this.Controls.Add(this.txtRemarks);
            this.Controls.Add(this.lblRemarks);
            this.Controls.Add(this.lstCharacterList);
            this.Controls.Add(this.txtInputID);
            this.Controls.Add(this.btnIDSerch);
            this.Controls.Add(this.lblNickName);
            this.Controls.Add(this.txtNickName);
            this.Controls.Add(this.lblSharp);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.lblRow);
            this.Controls.Add(this.cmdText);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.btnDownMax);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnUpMax);
            this.Controls.Add(this.cmdName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.cmdID);
            this.Controls.Add(this.txtALL);
            this.Controls.Add(this.txtRow);
            this.Controls.Add(this.cmdALL);
            this.Controls.Add(this.btnLogAnalysis);
            this.Controls.Add(this.btnSTOP);
            this.Controls.Add(this.cboSelectAnalysis);
            this.Controls.Add(this.progressBar1);
            this.MaximumSize = new System.Drawing.Size(800, 2400);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "ChatDetail";
            this.Text = "チャット詳細";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChatDetail_FormClosing);
            this.Load += new System.EventHandler(this.ChatDetail_Load);
            this.ResizeEnd += new System.EventHandler(this.ChatDetail_ResizeEnd);
            this.Resize += new System.EventHandler(this.ChatDetail_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button cmdALL;
        private System.Windows.Forms.TextBox txtRow;
        private System.Windows.Forms.TextBox txtALL;
        private System.Windows.Forms.Button cmdID;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button cmdName;
        private System.Windows.Forms.Button btnUpMax;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnDownMax;
        private System.Windows.Forms.Button cmdText;
        private System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.Label lblRow;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Label lblSharp;
        private System.Windows.Forms.TextBox txtNickName;
        private System.Windows.Forms.Label lblNickName;
        private System.Windows.Forms.TextBox txtInputID;
        private System.Windows.Forms.Button btnIDSerch;
        private System.Windows.Forms.ListBox lstCharacterList;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Button btnLogAnalysis;
        private System.Windows.Forms.Label lblCharacterList;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnSTOP;
        private System.Windows.Forms.ComboBox cboSelectAnalysis;
        private System.Windows.Forms.Label NICT;
        private System.Windows.Forms.Timer timer1;
    }
}