using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace PROTOCOL
{
    public partial class FRProt : Form, IDisposable
    {
        public delegate void OnHideForm();
        public OnHideForm onHide = null;

        private delegate void UpdateList();
        private UpdateList update;

        private enum UpdateMethod { _timer, _event };
        UpdateMethod updateMethod = UpdateMethod._event;
        private Timer timer;

        public enum SaveMethod { _none, _tofile, _todb };
        private SaveMethod _saveMethod = SaveMethod._none;
        public SaveMethod saveMethod
        {
            get
            {
                return _saveMethod;
            }
            set
            {
                switch (value)
                {
                    case SaveMethod._none:
                        _saveMethod = SaveMethod._none;
                        //Если будем как-то отдельно открывать коннекцию к базе, то надо её закрыть
                        closeDB();
                        //Если у нас запись в файл, то надо его закрыть.
                        closeFile();
                        break;
                    case SaveMethod._todb:
                        _saveMethod = SaveMethod._todb;
                        //Если у нас запись в файл, то надо его закрыть.
                        closeFile();
                        openDB();
                        break;
                    case SaveMethod._tofile:
                        _saveMethod = SaveMethod._tofile;
                        closeDB();
                        openFile();
                        break;
                }
            }
        }
        //Последняя записанная запись из протокола
        private static int lastRecordedIndex = 0;
        //Коннекция для записи в БД
        //Пока не пользуем, возьмем готовую из RAGLIB
        //private SqlConnection sqlConnection = null;
        //Для записи в файл
        private string fileName = null;
        public void setLogName(string _logName)
        {
            fileName = _logName;
        }
        private StreamWriter streamWriter = null;
        private bool openFile(string fName = null)
        {
            //Прверяем может файл уже открыт
            if (streamWriter != null) return true;
            if(fName==null)
                fName = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\log.txt";
            try
            {
                streamWriter = new StreamWriter(fName, true);
                log.add(LogRecord.LogReason.info, "Открыли файл протокола.");
                return true;
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "FRProt: OpenFile: {0}", ex.Message);
                return false;
            }
        }
        private void closeFile()
        {
            if (streamWriter != null)
            {
                try
                {
                    log.add(LogRecord.LogReason.info, "Закрываем файл протокола.");
                    streamWriter.Flush();
                    streamWriter.Dispose();
                    streamWriter = null;
                }
                catch (Exception ex)
                {
                    log.add(LogRecord.LogReason.error, "FRProt: CloseFile: {0}", ex.Message);
                    return;
                }
            }
        }

        private void checkLogTable()
        {
            //04/05/2018
            /*
            string SQL = "select count(*) as nn from INFORMATION_SCHEMA.TABLES where table_type='BASE TABLE' and TABLE_SCHEMA='Uran' and table_name='logtable'";
            Select S = new Select(SQL);
            if (!S.Read())
                throw new ArgumentException("RAGLIB:DBSPar:CheckTable: " + SQL + " - не нашли записей");
            if (S.AsInt("nn") == 1)
                return;
            //Если таблица не существует, создаем её
            SQL = "CREATE TABLE [Uran].[logtable](" +
                "[id] [int] IDENTITY(1,1) NOT NULL," +
                "[tstamp] [varchar](20) NOT NULL," +
                "[reason] [varchar](20) NOT NULL," +
                "[text] [varchar](200) NOT NULL," +
                "CONSTRAINT [PK_logtable] PRIMARY KEY CLUSTERED " +
                "   (" +
                "       [id] ASC" +
                "   ) ON [PRIMARY]" +
                ") ON [PRIMARY]";
            ExecSQL E = new ExecSQL(SQL);
             */ 
        }
        //Если будем предпринимать какие-то действия с базой, то здесь можно надо будет всё закрыть
        private void openDB()
        {
            checkLogTable();
        }
        //Если будем предпринимать какие-то действия с базой, то здесь можно надо будет всё закрыть
        private void closeDB()
        {
        }
                
        public FRProt(Form _frm)
        {
            InitializeComponent();
            Owner = _frm;
            KeyPreview = true;
            KeyDown += new KeyEventHandler(FRProt_KeyDown);
        
            //Настраиваем колонки
            lvProt.Columns.Add("Время",200);
            lvProt.Columns.Add("",500);

            //Настраиваем таймер
            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            if (updateMethod == UpdateMethod._event)
            {
                Load += new EventHandler(FRProt_Load);
            }
        }
        ~FRProt()
        {
            timer.Stop();
            timer.Dispose();
            timer = null;
            closeFile();
            closeDB();
        }
        void FRProt_Load(object sender, EventArgs e)
        {
            if (updateMethod == UpdateMethod._event)
            {
                update += new UpdateList(logToList);
                log.onLogChanged += new log.OnLogChanged(invokeUpdateList);
                FormClosed += new FormClosedEventHandler(FRProt_FormClosed);
                logToList();
            }
        }

        void FRProt_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (updateMethod == UpdateMethod._event)
            {
                log.onLogChanged -= new log.OnLogChanged(invokeUpdateList);
            }
        }

        void invokeUpdateList()
        {
            try
            {
                BeginInvoke(update);
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "FRProt: invokeUpdateList: {0}", ex.Message);
                return;
            }
        }

        void logToList()
        {
            while (log.size() > 0)
            {
                LogRecord rec = log.get();
                string[] subitems = { rec.dt.ToString("dd/MM/yy HH:mm:ss.ff"), rec.text };
                ListViewItem item = new ListViewItem(subitems, (int)rec.reason);
                lvProt.EnsureVisible(lvProt.Items.Add(item).Index);
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            
            if (updateMethod == UpdateMethod._timer)
            {
                while (log.size() > 0)
                {
                    LogRecord rec = log.get();
                    string[] subitems = { rec.dt.ToString("dd/MM/yy HH:mm:ss.ff"), rec.text }; 
                    ListViewItem item = new ListViewItem(subitems, (int)rec.reason);
                    lvProt.EnsureVisible(lvProt.Items.Add(item).Index);
                }
            }
            if (_saveMethod == SaveMethod._tofile)
            {
                while (lastRecordedIndex < lvProt.Items.Count)
                {
                    streamWriter.WriteLine(string.Format("{0} -> {1}", lvProt.Items[lastRecordedIndex].Text, lvProt.Items[lastRecordedIndex].SubItems[1].Text));
                    lastRecordedIndex++;
                }
                streamWriter.Flush();
            }
            if (_saveMethod == SaveMethod._todb)
            {
                while (lastRecordedIndex < lvProt.Items.Count)
                {
                    //04/05/2018
                    /*
                    string SQL = "insert into [Uran].[logtable] values ('" + lvProt.Items[lastRecordedIndex].Text + "','" +
                       ((LogRecord.LogReason)lvProt.Items[lastRecordedIndex].ImageIndex).ToString() + "','" +
                       lvProt.Items[lastRecordedIndex].SubItems[1].Text + "')";
                    ExecSQL E = new ExecSQL(SQL);
                    */ 
                    lastRecordedIndex++;
                }
            }
        }
        
        void FRProt_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F11:
                case Keys.Escape:
                    Visible = false;
                    if (onHide != null) onHide();
                    break;
            }
        }
        private void FRProt_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch(e.CloseReason)
            {
                case CloseReason.UserClosing:
                    e.Cancel = true;
                    Visible = false;
                    if (onHide != null) onHide();
                    break;
            }
        }
    }
}
