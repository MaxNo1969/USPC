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
            this.miStart = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miView = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStreepMenuItemClick = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miTest = new System.Windows.Forms.ToolStripMenuItem();
            this.miBoardTest = new System.Windows.Forms.ToolStripMenuItem();
            this.miTestUSPCAscan = new System.Windows.Forms.ToolStripMenuItem();
            this.tCPServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miTestAscanFromNet = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsProt = new System.Windows.Forms.ToolStripMenuItem();
            this.miWindowsSignals = new System.Windows.Forms.ToolStripMenuItem();
            this.miUSPC = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpenUSPC = new System.Windows.Forms.ToolStripMenuItem();
            this.miLoadUSPC = new System.Windows.Forms.ToolStripMenuItem();
            this.miCloseUSPC = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.miBoardInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.miEmul = new System.Windows.Forms.ToolStripMenuItem();
            this.sb = new System.Windows.Forms.StatusStrip();
            this.info = new System.Windows.Forms.ToolStripStatusLabel();
            this.pb = new System.Windows.Forms.ToolStripProgressBar();
            this.dataSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tubePos = new System.Windows.Forms.ToolStripStatusLabel();
            this.speed = new System.Windows.Forms.ToolStripStatusLabel();
            this.duration = new System.Windows.Forms.ToolStripStatusLabel();
            this.heap = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerUpdUI = new System.Windows.Forms.Timer(this.components);
            this.tb = new System.Windows.Forms.ToolStrip();
            this.lblTubesCount = new System.Windows.Forms.ToolStripLabel();
            this.cbTypeSize = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.menu.SuspendLayout();
            this.sb.SuspendLayout();
            this.tb.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMainWork,
            this.настройкиToolStripMenuItem,
            this.miView,
            this.miTest,
            this.miWindows,
            this.miUSPC,
            this.miEmul});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1020, 24);
            this.menu.TabIndex = 1;
            this.menu.Text = "menuStrip1";
            // 
            // miMainWork
            // 
            this.miMainWork.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miStart,
            this.toolStripMenuItem1,
            this.miExit});
            this.miMainWork.Name = "miMainWork";
            this.miMainWork.Size = new System.Drawing.Size(57, 20);
            this.miMainWork.Text = "Работа";
            // 
            // miStart
            // 
            this.miStart.Name = "miStart";
            this.miStart.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miStart.Size = new System.Drawing.Size(146, 22);
            this.miStart.Text = "Старт";
            this.miStart.Click += new System.EventHandler(this.miStart_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.miExit.Size = new System.Drawing.Size(146, 22);
            this.miExit.Text = "Выход";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSettings});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // miSettings
            // 
            this.miSettings.Name = "miSettings";
            this.miSettings.Size = new System.Drawing.Size(134, 22);
            this.miSettings.Text = "Настройки";
            this.miSettings.Click += new System.EventHandler(this.miSettings_Click);
            // 
            // miView
            // 
            this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStreepMenuItemClick,
            this.saveToolStripMenuItem,
            this.testChartToolStripMenuItem,
            this.genToolStripMenuItem});
            this.miView.Name = "miView";
            this.miView.Size = new System.Drawing.Size(76, 20);
            this.miView.Text = "Просмотр";
            // 
            // openToolStreepMenuItemClick
            // 
            this.openToolStreepMenuItemClick.Name = "openToolStreepMenuItemClick";
            this.openToolStreepMenuItemClick.Size = new System.Drawing.Size(163, 22);
            this.openToolStreepMenuItemClick.Text = "Загрузить файл";
            this.openToolStreepMenuItemClick.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.saveToolStripMenuItem.Text = "Выгрузить файл";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // testChartToolStripMenuItem
            // 
            this.testChartToolStripMenuItem.Name = "testChartToolStripMenuItem";
            this.testChartToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.testChartToolStripMenuItem.Text = "TestChart";
            this.testChartToolStripMenuItem.Click += new System.EventHandler(this.testChartToolStripMenuItem_Click);
            // 
            // genToolStripMenuItem
            // 
            this.genToolStripMenuItem.Name = "genToolStripMenuItem";
            this.genToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.genToolStripMenuItem.Text = "Генерировать";
            this.genToolStripMenuItem.Click += new System.EventHandler(this.genToolStripMenuItem_Click);
            // 
            // miTest
            // 
            this.miTest.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBoardTest,
            this.miTestUSPCAscan,
            this.tCPServerToolStripMenuItem,
            this.miTestAscanFromNet});
            this.miTest.Name = "miTest";
            this.miTest.Size = new System.Drawing.Size(95, 20);
            this.miTest.Text = "Тестирование";
            // 
            // miBoardTest
            // 
            this.miBoardTest.Name = "miBoardTest";
            this.miBoardTest.Size = new System.Drawing.Size(173, 22);
            this.miBoardTest.Text = "Плата";
            // 
            // miTestUSPCAscan
            // 
            this.miTestUSPCAscan.Name = "miTestUSPCAscan";
            this.miTestUSPCAscan.Size = new System.Drawing.Size(173, 22);
            this.miTestUSPCAscan.Text = "TestAcqFromNet";
            this.miTestUSPCAscan.Click += new System.EventHandler(this.miTestUSPCAscan_Click);
            // 
            // tCPServerToolStripMenuItem
            // 
            this.tCPServerToolStripMenuItem.Name = "tCPServerToolStripMenuItem";
            this.tCPServerToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.tCPServerToolStripMenuItem.Text = "TCPServer";
            this.tCPServerToolStripMenuItem.Click += new System.EventHandler(this.tCPServerToolStripMenuItem_Click);
            // 
            // miTestAscanFromNet
            // 
            this.miTestAscanFromNet.Name = "miTestAscanFromNet";
            this.miTestAscanFromNet.Size = new System.Drawing.Size(173, 22);
            this.miTestAscanFromNet.Text = "TestAscanFromNet";
            this.miTestAscanFromNet.Click += new System.EventHandler(this.miTestAscanFromNet_Click);
            // 
            // miWindows
            // 
            this.miWindows.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miWindowsProt,
            this.miWindowsSignals});
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
            // miWindowsSignals
            // 
            this.miWindowsSignals.Name = "miWindowsSignals";
            this.miWindowsSignals.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.miWindowsSignals.Size = new System.Drawing.Size(154, 22);
            this.miWindowsSignals.Text = "Сигналы";
            this.miWindowsSignals.Click += new System.EventHandler(this.miWindowsSignals_Click);
            // 
            // miUSPC
            // 
            this.miUSPC.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOpenUSPC,
            this.miLoadUSPC,
            this.miCloseUSPC,
            this.toolStripMenuItem2,
            this.miBoardInfo});
            this.miUSPC.Name = "miUSPC";
            this.miUSPC.Size = new System.Drawing.Size(48, 20);
            this.miUSPC.Text = "USPC";
            // 
            // miOpenUSPC
            // 
            this.miOpenUSPC.Name = "miOpenUSPC";
            this.miOpenUSPC.Size = new System.Drawing.Size(215, 22);
            this.miOpenUSPC.Text = "Открыть плату";
            this.miOpenUSPC.Click += new System.EventHandler(this.miOpenUSPC_Click);
            // 
            // miLoadUSPC
            // 
            this.miLoadUSPC.Name = "miLoadUSPC";
            this.miLoadUSPC.Size = new System.Drawing.Size(215, 22);
            this.miLoadUSPC.Text = "Загрузить конфигурацию";
            this.miLoadUSPC.Click += new System.EventHandler(this.miLoadUSPC_Click);
            // 
            // miCloseUSPC
            // 
            this.miCloseUSPC.Name = "miCloseUSPC";
            this.miCloseUSPC.Size = new System.Drawing.Size(215, 22);
            this.miCloseUSPC.Text = "Закрыть плату";
            this.miCloseUSPC.Click += new System.EventHandler(this.miCloseUSPC_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(212, 6);
            // 
            // miBoardInfo
            // 
            this.miBoardInfo.Name = "miBoardInfo";
            this.miBoardInfo.Size = new System.Drawing.Size(215, 22);
            this.miBoardInfo.Text = "Информация";
            this.miBoardInfo.Click += new System.EventHandler(this.miBoardInfo_Click);
            // 
            // miEmul
            // 
            this.miEmul.Name = "miEmul";
            this.miEmul.Size = new System.Drawing.Size(74, 20);
            this.miEmul.Text = "Эмуляция";
            this.miEmul.Click += new System.EventHandler(this.эмуляцияToolStripMenuItem_Click);
            // 
            // sb
            // 
            this.sb.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.info,
            this.pb,
            this.dataSize,
            this.tubePos,
            this.speed,
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
            this.info.Size = new System.Drawing.Size(495, 17);
            this.info.Spring = true;
            this.info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pb
            // 
            this.pb.AutoSize = false;
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(200, 16);
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
            // speed
            // 
            this.speed.AutoSize = false;
            this.speed.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.speed.Name = "speed";
            this.speed.Size = new System.Drawing.Size(50, 17);
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
            this.toolStripLabel1,
            this.lblTubesCount,
            this.cbTypeSize});
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
            // cbTypeSize
            // 
            this.cbTypeSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeSize.Name = "cbTypeSize";
            this.cbTypeSize.Size = new System.Drawing.Size(121, 33);
            this.cbTypeSize.SelectedIndexChanged += new System.EventHandler(this.cbTypeSize_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(74, 30);
            this.toolStripLabel1.Text = "Типоразмер";
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
        private System.Windows.Forms.ToolStripMenuItem miStart;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miTest;
        private System.Windows.Forms.ToolStripMenuItem miBoardTest;
        private System.Windows.Forms.ToolStripStatusLabel info;
        private System.Windows.Forms.ToolStripMenuItem miWindows;
        private System.Windows.Forms.ToolStripMenuItem miWindowsProt;
        private System.Windows.Forms.ToolStripMenuItem openToolStreepMenuItemClick;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miUSPC;
        private System.Windows.Forms.ToolStripMenuItem miOpenUSPC;
        private System.Windows.Forms.ToolStripMenuItem miLoadUSPC;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem miBoardInfo;
        private System.Windows.Forms.ToolStripStatusLabel dataSize;
        private System.Windows.Forms.ToolStripStatusLabel tubePos;
        private System.Windows.Forms.ToolStripStatusLabel duration;
        private System.Windows.Forms.ToolStripStatusLabel heap;
        private System.Windows.Forms.Timer timerUpdUI;
        private System.Windows.Forms.ToolStripMenuItem miTestUSPCAscan;
        private System.Windows.Forms.ToolStripMenuItem tCPServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miCloseUSPC;
        private System.Windows.Forms.ToolStripMenuItem miTestAscanFromNet;
        private System.Windows.Forms.ToolStripMenuItem miWindowsSignals;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miSettings;
        private System.Windows.Forms.ToolStrip tb;
        private System.Windows.Forms.ToolStripLabel lblTubesCount;
        private System.Windows.Forms.ToolStripMenuItem miEmul;
        private System.Windows.Forms.ToolStripStatusLabel speed;
        private System.Windows.Forms.ToolStripMenuItem testChartToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar pb;
        private System.Windows.Forms.ToolStripMenuItem genToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cbTypeSize;

    }
}

