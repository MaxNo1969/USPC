using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PROTOCOL;
using System.Diagnostics;
using System.IO;

namespace USPC
{
    public partial class FRUSPCBoard : Form
    {

        double[] x;
        double[] y1;
        double[] y2;
       
        int size = 1000;
        
        PCXUS pcxus;
        
        public FRUSPCBoard(Form _frm)
        {
            InitializeComponent();
            Owner = _frm;

            x = new double[size];
            y1 = new double[size];
            y2 = new double[size];
            for (int i = 0; i < size; i++)
            {
                x[i] = (double)i;
                y1[i] = 0;
                y1[2] = 0;
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            pcxus = new PCXUS();
            if (!pcxus.open(2))
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Не удалось открыть плату USPC");
                return;
            }
            log.add(LogRecord.LogReason.info, "Установлено {0} плат USPC", pcxus.numBoards);
            log.add(LogRecord.LogReason.info, "Board S/N (function) = {0}", pcxus.serialNumber());
            log.add(LogRecord.LogReason.info, "Board S/N (param) = {0}", (int)pcxus.getParamValueDouble("board_serial"));
            log.add(LogRecord.LogReason.info, "MUX S/N = {0}", pcxus.getParamValueDouble("mux_rcpp_serial_number"));
            log.add(LogRecord.LogReason.info, "HW Version = {0}", pcxus.getParamValueString("hardware_version"));
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            pcxus.close();
            return;
        }
        
        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Файлы конфигураций (*.us)|*.us|All files (*.*)|*.*";
                ofd.FilterIndex = 0;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string fileName = ofd.FileName;
                    if(!pcxus.load(fileName))return;
                }
            }
        }
        
        int Board = 0;
        
        private void btnAcquire_Click(object sender, EventArgs e)
        {
            if (pcxus == null) return;

            Int32 status = 0;
            Int32 NumberOfScansAcquired = 0;
            Int32 NumberOfScansRead = 0;
            Int32 BufferSize = 0;
            Int32 ScanSize = 0;
            //Подготавливаем к захвату
            if (!pcxus.config(Board, 1024*100))
            {
                return;
            }
            if (!pcxus.status(Board, ref status, ref NumberOfScansAcquired, ref NumberOfScansRead, ref BufferSize, ref ScanSize))
            {
                return;
            }
            ACQ_STATUS acqStatus = (ACQ_STATUS)status;
            //log.add(string.Format("ACQ_STATUS: {0}", acqStatus.ToString()), LogRecord.LogReason.info);
            Debug.WriteLine(string.Format("ACQ_STATUS: {0}", acqStatus.ToString()));
            //log.add(string.Format("BufferSize(in numbers od scans): {0}, ScanSize(in number of DWORD): {1}", BufferSize, ScanSize), LogRecord.LogReason.info);
            Debug.WriteLine(string.Format("BufferSize(in numbers od scans): {0}, ScanSize(in number of DWORD): {1}", BufferSize, ScanSize));
            //byte[] buffer = new byte[BufferSize*ScanSize*sizeof(uint)];
            if (!pcxus.start(Board))
            {
                return;
            }
            
            if (!pcxus.status(Board, ref status, ref NumberOfScansAcquired, ref NumberOfScansRead, ref BufferSize, ref ScanSize))
            {
                return;
            }
            acqStatus = (ACQ_STATUS)status;
            //log.add(string.Format("ACQ_STATUS: {0}", acqStatus.ToString()), LogRecord.LogReason.info);
            Debug.WriteLine(string.Format("ACQ_STATUS: {0}", acqStatus.ToString()));
            //log.add(string.Format("BufferSize(in numbers od scans): {0}, ScanSize(in number of DWORD): {1}", BufferSize, ScanSize), LogRecord.LogReason.info);
            Debug.WriteLine(string.Format("BufferSize(in numbers od scans): {0}, ScanSize(in number of DWORD): {1}", BufferSize, ScanSize));

            byte[] scans = new byte[BufferSize * ScanSize * sizeof(Int32)];
            Int32 scansReaded = pcxus.read(Board, scans);
            if (scansReaded > 0)
            {
                log.add(LogRecord.LogReason.info, "ACQ_READ: {0} scans readed", scansReaded);
            }
            pcxus.stop(Board);
            pcxus.clear(Board);
            string _fName = "result.csv";
            using (StreamWriter writer = new StreamWriter(_fName, false))
            {
                //Пишем заголовок
                string s = "ScanCounter;PulseCounter;"; 
                s += "Gate1Amp;Gate1WTmsb;Gate1Flags;Gate1Alarm;Gate1DET;Gate1ThicknessMin;";
                s += "Gate1ThicknessMax;Gate1WT16bits;Gate1TOF;Gate1WTlsb;";
                s += "Gate2Amp;Gate2WTmsb;Gate2Flags;Gate2Alarm;Gate2DET;Gate2ThicknessMin;";
                s += "Gate2ThicknessMax;Gate2WT16bits;Gate2TOF;Gate2WTlsb;";
                s += "GateIFWTmsb;Channel;GateIFFlafs;GateIFAlarm;GateIFDET;GateIFENABLE;GateIFWT16bits;GateIFTOF;GateIFWTlsb";
                writer.WriteLine(s);
                for (int i = 0; i < scansReaded; i++)
                {
                    s = "";
                    uint[] scanWords = new uint[ScanSize];
                    for(int j = 0; j< ScanSize;j++ )
                        scanWords[j] = BitConverter.ToUInt32(scans,(int)(i*ScanSize*sizeof(Int32)+j*sizeof(Int32)));
                    uint ScanCounter = (uint)scanWords[0];
                    s += string.Format("{0};",ScanCounter); 
                    uint PulseCounter = (uint)scanWords[1];
                    s += string.Format("{0};",PulseCounter); 
                
                    byte Gate1Amp = (byte)(scanWords[2] & 0x000000ff);
                    s += string.Format("{0};",Gate1Amp); 
                    byte Gate1WTmsb = (byte)((scanWords[2] & 0x0000ff00) >> 8);
                    s += string.Format("{0};",Gate1WTmsb); 
                    byte Gate1Flags = (byte)((scanWords[2] & 0x00ff0000) >> 16);
                    s += string.Format("{0};",Gate1Flags); 

                    byte Gate1Alarm = (byte)((Gate1Flags & (1 << 0))>>0);
                    s += string.Format("{0};",Gate1Alarm); 
                    byte Gate1DET = (byte)((Gate1Flags & (1 << 1))>>1);
                    s += string.Format("{0};",Gate1DET); 
                    byte Gate1ThicknessMin = (byte)((Gate1Flags & (1 << 2))>>2);
                    s += string.Format("{0};",Gate1ThicknessMin); 
                    byte Gate1ThicknessMax = (byte)((Gate1Flags & (1 << 3))>>3);
                    s += string.Format("{0};",Gate1ThicknessMax); 
                    byte Gate1WT16bits = (byte)((Gate1Flags & (1 << 4))>>4);
                    s += string.Format("{0};",Gate1WT16bits); 


                    uint Gate1TOF = (uint)(scanWords[3] & 0x00ffffff);
                    s += string.Format("{0};",Gate1TOF); 
                    uint Gate1WTlsb = (uint)((scanWords[3] & 0xff000000)>>24);
                    s += string.Format("{0};",Gate1WTlsb); 

                    uint Gate2Amp = (uint)(scanWords[4] & 0x000000ff);
                    s += string.Format("{0};",Gate2Amp); 
                    uint Gate2WTmsb = (uint)((scanWords[4] & 0x0000ff00) >> 8);
                    s += string.Format("{0};",Gate2WTmsb); 
                    uint Gate2Flags = (uint)((scanWords[4] & 0x00ff0000) >> 16);
                    s += string.Format("{0};",Gate2Flags); 

                    byte Gate2Alarm = (byte)((Gate2Flags & (1 << 0))>>0);
                    s += string.Format("{0};",Gate2Alarm); 
                    byte Gate2DET = (byte)((Gate2Flags & (1 << 1))>>1);
                    s += string.Format("{0};",Gate2DET); 
                    byte Gate2ThicknessMin = (byte)((Gate2Flags & (1 << 2))>>2);
                    s += string.Format("{0};",Gate2ThicknessMin); 
                    byte Gate2ThicknessMax = (byte)((Gate2Flags & (1 << 3))>>3);
                    s += string.Format("{0};",Gate2ThicknessMax); 
                    byte Gate2WT16bits = (byte)((Gate2Flags & (1 << 4))>>4);
                    s += string.Format("{0};",Gate2WT16bits); 


                    uint Gate2TOF = (uint)(scanWords[5] & 0x00ffffff);
                    s += string.Format("{0};",Gate2TOF); 
                    uint Gate2WTlsb = (uint)((scanWords[5] & 0xff000000) >> 24);
                    s += string.Format("{0};",Gate2WTlsb); 

                    uint GateIFWTmsb = (uint)(scanWords[6] & 0x000000ff);
                    s += string.Format("{0};",GateIFWTmsb); 
                    uint Channel = (uint)((scanWords[6] & 0x0000ff00) >> 8);
                    s += string.Format("{0};",Channel); 
                    uint GateIFFlags = (uint)((scanWords[6] & 0x00ff0000) >> 16);
                    s += string.Format("{0};",GateIFFlags); 

                    byte GateIFAlarm = (byte)((Gate1Flags & (1 << 0))>>0);
                    s += string.Format("{0};",GateIFAlarm); 
                    byte GateIFDET = (byte)((Gate1Flags & (1 << 1))>>1);
                    s += string.Format("{0};",GateIFDET); 
                    byte GateIFENABLE = (byte)((Gate1Flags & (1 << 2))>>2);
                    s += string.Format("{0};",GateIFENABLE); 
                    byte GateIFWT16bits = (byte)((Gate1Flags & (1 << 4))>>4);
                    s += string.Format("{0};",GateIFWT16bits); 

                    uint GateIFTOF = (uint)(scanWords[7] & 0x00ffffff);
                    s += string.Format("{0};",GateIFTOF); 
                    uint GateIFWTlsb = (uint)((scanWords[7] & 0xff000000) >> 24);
                    s += string.Format("{0};",GateIFWTlsb);

                    if (i < size)
                    {
                        y1[i] = (double)Gate1WTlsb;
                    }

                    writer.WriteLine(s);
                }
                writer.Close();
            }
        }
    }
}
