namespace USPC
{
    partial class FRMain
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
            this.menu = new System.Windows.Forms.MenuStrip();
            this.miMainWork = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miView = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miTest = new System.Windows.Forms.ToolStripMenuItem();
            this.miTestTestAcq = new System.Windows.Forms.ToolStripMenuItem();
            this.miTestTestAscan = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsProt = new System.Windows.Forms.ToolStripMenuItem();
            this.miUSPC = new System.Windows.Forms.ToolStripMenuItem();
            this.miUSPCOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miUSPCLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.miUSPCClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.miUSPCGetInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.sb = new System.Windows.Forms.StatusStrip();
            this.info = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tubePos = new System.Windows.Forms.ToolStripStatusLabel();
            this.duration = new System.Windows.Forms.ToolStripStatusLabel();
            this.heap = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerUpdUI = new System.Windows.Forms.Timer(this.components);
            this.tb = new System.Windows.Forms.ToolStrip();
            this.lblTubesCount = new System.Windows.Forms.ToolStripLabel();
            this.menu.SuspendLayout();
            this.sb.SuspendLayout();
            this.tb.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUSPC,
            this.miMainWork,
            this.miView,
            this.miTest,
            this.miWindows});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1020, 24);
            this.menu.TabIndex = 1;
            this.menu.Text = "menuStrip1";
            // 
            // miMainWork
            // 
            this.miMainWork.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miExit});
            this.miMainWork.Name = "miMainWork";
            this.miMainWork.Size = new System.Drawing.Size(57, 20);
            this.miMainWork.Text = "Работа";
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.miExit.Size = new System.Drawing.Size(152, 22);
            this.miExit.Text = "Выход";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // miView
            // 
            this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miLoadFile,
            this.miSaveFile});
            this.miView.Name = "miView";
            this.miView.Size = new System.Drawing.Size(76, 20);
            this.miView.Text = "Просмотр";
            // 
            // miLoadFile
            // 
            this.miLoadFile.Name = "miLoadFile";
            this.miLoadFile.Size = new System.Drawing.Size(163, 22);
            this.miLoadFile.Text = "Загрузить файл";
            this.miLoadFile.Click += new System.EventHandler(this.miLoadFile_Click);
            // 
            // miSaveFile
            // 
            this.miSaveFile.Name = "miSaveFile";
            this.miSaveFile.Size = new System.Drawing.Size(163, 22);
            this.miSaveFile.Text = "Выгрузить файл";
            this.miSaveFile.Click += new System.EventHandler(this.miSaveFile_Click);
            // 
            // miTest
            // 
            this.miTest.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miTestTestAcq,
            this.miTestTestAscan});
            this.miTest.Name = "miTest";
            this.miTest.Size = new System.Drawing.Size(95, 20);
            this.miTest.Text = "Тестирование";
            // 
            // miTestTestAcq
            // 
            this.miTestTestAcq.Name = "miTestTestAcq";
            this.miTestTestAcq.Size = new System.Drawing.Size(155, 22);
            this.miTestTestAcq.Text = "TestAcq";
            this.miTestTestAcq.Click += new System.EventHandler(this.miBoardTest_Click);
            // 
            // miTestTestAscan
            // 
            this.miTestTestAscan.Name = "miTestTestAscan";
            this.miTestTestAscan.Size = new System.Drawing.Size(155, 22);
            this.miTestTestAscan.Text = "TestUSPCAscan";
            this.miTestTestAscan.Click += new System.EventHandler(this.miTestUSPCAscan_Click);
            // 
            // miWindows
            // 
            this.miWindows.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miWindowsProt});
            this.miWindows.Name = "miWindows";
            this.miWindows.Size = new System.Drawing.Size(47, 20);
            this.miWindows.Text = "Окна";
            // 
            // miWindowsProt
            // 
            this.miWindowsProt.Name = "miWindowsProt";
            this.miWindowsProt.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.miWindowsProt.Size = new System.Drawing.Size(154, 22);
            this.miWindowsProt.Text = "Протокол";
            this.miWindowsProt.Click += new System.EventHandler(this.miShowProt_Click);
            // 
            // miUSPC
            // 
            this.miUSPC.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miUSPCOpen,
            this.miUSPCLoad,
            this.miUSPCClose,
            this.toolStripMenuItem2,
            this.miUSPCGetInfo});
            this.miUSPC.Name = "miUSPC";
            this.miUSPC.Size = new System.Drawing.Size(48, 20);
            this.miUSPC.Text = "USPC";
            // 
            // miUSPCOpen
            // 
            this.miUSPCOpen.Name = "miUSPCOpen";
            this.miUSPCOpen.Size = new System.Drawing.Size(215, 22);
            this.miUSPCOpen.Text = "Открыть плату";
            this.miUSPCOpen.Click += new System.EventHandler(this.miOpenUSPC_Click);
            // 
            // miUSPCLoad
            // 
            this.miUSPCLoad.Name = "miUSPCLoad";
            this.miUSPCLoad.Size = new System.Drawing.Size(215, 22);
            this.miUSPCLoad.Text = "Загрузить конфигурацию";
            this.miUSPCLoad.Click += new System.EventHandler(this.miLoadUSPC_Click);
            // 
            // miUSPCClose
            // 
            this.miUSPCClose.Name = "miUSPCClose";
            this.miUSPCClose.Size = new System.Drawing.Size(215, 22);
            this.miUSPCClose.Text = "Закрыть плату";
            this.miUSPCClose.Click += new System.EventHandler(this.miCloseUSPC_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(212, 6);
            // 
            // miUSPCGetInfo
            // 
            this.miUSPCGetInfo.Name = "miUSPCGetInfo";
            this.miUSPCGetInfo.Size = new System.Drawing.Size(215, 22);
            this.miUSPCGetInfo.Text = "Информация";
            this.miUSPCGetInfo.Click += new System.EventHandler(this.miBoardInfo_Click);
            // 
            // sb
            // 
            this.sb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.info,
            this.dataSize,
            this.tubePos,
            this.duration,
            this.heap});
            this.sb.Location = new System.Drawing.Point(0, 517);
            this.sb.Name = "sb";
            this.sb.Size = new System.Drawing.Size(1020, 22);
            this.sb.TabIndex = 2;
            this.sb.Text = "sb";
            // 
            // info
            // 
            this.info.AutoSize = false;
            this.info.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.info.Name = "info";
            this.info.Size = new System.Drawing.Size(747, 17);
            this.info.Spring = true;
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataSize
            // 
            this.dataSize.AutoSize = false;
            this.dataSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.dataSize.Name = "dataSize";
            this.dataSize.Size = new System.Drawing.Size(80, 17);
            // 
            // tubePos
            // 
            this.tubePos.AutoSize = false;
            this.tubePos.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tubePos.Name = "tubePos";
            this.tubePos.Size = new System.Drawing.Size(48, 17);
            // 
            // duration
            // 
            this.duration.AutoSize = false;
            this.duration.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.duration.Name = "duration";
            this.duration.Size = new System.Drawing.Size(50, 17);
            // 
            // heap
            // 
            this.heap.AutoSize = false;
            this.heap.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.heap.Name = "heap";
            this.heap.Size = new System.Drawing.Size(80, 17);
            this.heap.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timerUpdUI
            // 
            this.timerUpdUI.Tick += new System.EventHandler(this.timerUpdUI_Tick);
            // 
            // tb
            // 
            this.tb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTubesCount});
            this.tb.Location = new System.Drawing.Point(0, 24);
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(1020, 33);
            this.tb.TabIndex = 4;
            // 
            // lblTubesCount
            // 
            this.lblTubesCount.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblTubesCount.AutoSize = false;
            this.lblTubesCount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblTubesCount.Name = "lblTubesCount";
            this.lblTubesCount.Size = new System.Drawing.Size(50, 30);
            this.lblTubesCount.Text = "0";
            this.lblTubesCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FRMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 539);
            this.Controls.Add(this.tb);
            this.Controls.Add(this.sb);
            this.Controls.Add(this.menu);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menu;
            this.Name = "FRMain";
            this.Text = "USPC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FRMain_FormClosing);
            this.Load += new System.EventHandler(this.FRMain_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.sb.ResumeLayout(false);
            this.sb.PerformLayout();
            this.tb.ResumeLayout(false);
            this.tb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem miMainWork;
        private System.Windows.Forms.ToolStripMenuItem miView;
        private System.Windows.Forms.StatusStrip sb;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miTest;
        private System.Windows.Forms.ToolStripMenuItem miTestTestAcq;
        private System.Windows.Forms.ToolStripStatusLabel info;
        private System.Windows.Forms.ToolStripMenuItem miWindows;
        private System.Windows.Forms.ToolStripMenuItem miWindowsProt;
        private System.Windows.Forms.ToolStripMenuItem miLoadFile;
        private System.Windows.Forms.ToolStripMenuItem miSaveFile;
        private System.Windows.Forms.ToolStripMenuItem miUSPC;
        private System.Windows.Forms.ToolStripMenuItem miUSPCOpen;
        private System.Windows.Forms.ToolStripMenuItem miUSPCLoad;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem miUSPCGetInfo;
        private System.Windows.Forms.ToolStripStatusLabel dataSize;
        private System.Windows.Forms.ToolStripStatusLabel tubePos;
        private System.Windows.Forms.ToolStripStatusLabel duration;
        private System.Windows.Forms.ToolStripStatusLabel heap;
        private System.Windows.Forms.Timer timerUpdUI;
        private System.Windows.Forms.ToolStripMenuItem miTestTestAscan;
        private System.Windows.Forms.ToolStripMenuItem miUSPCClose;
        private System.Windows.Forms.ToolStrip tb;
        private System.Windows.Forms.ToolStripLabel lblTubesCount;

    }
}

