using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using PROTOCOL;
using System.Diagnostics;

namespace USPC
{
    #region Data structures
    // Ascan structure is designed to convert an A-scan binary buffer to a data structure.
    [Serializable()]
    [StructLayout(LayoutKind.Explicit)]
    public struct Ascan
    {
        public const Byte ENABLE_MASK = 0x20;
        public const UInt32 TOF_MASK = 0xFFFFFF;
        public const Byte G1_FILTER_LEVEL_MASK = 0x0F;
        public const Byte G2_FILTER_LEVEL_MASK = 0xF0;

        [FlagsAttribute]
        public enum GateFlags : byte
        {
            CouplingAlarm = 0x1,
            DET = 0x2,
            ThicknessAlarmMin = 0x4,
            ThicknessAlarmMax = 0x8,
            Format16bits = 0x10
        };
        [FlagsAttribute]
        public enum GateInAscan : byte
        {
            GateStartInAscan = 0x2,
            GateEndInAscan = 0x10,
            Input1 = 0x20
        };

        [FlagsAttribute]
        public enum GateIFFlags : byte
        {
            AmpAlarm = 0x2,
            Format16bits = 0x10,
            ThicknessAlarmMin = 0x20,
            ThicknessAlarmMax = 0x40
        };
        [FlagsAttribute]
        public enum GateIFInAscan : byte
        {
            GateStartInAscan = 0x40,
            GateEndInAscan = 0x80
        };
        [FlagsAttribute]
        public enum AlarmFlags : int
        {
            CycleAlarm = 0x100000,
            PRFAlarm = 0x200000,
            PowerAlarm = 0x20000000
        };

        [FieldOffset(0)]
        public UInt32 G1Begin;          // Begin of gate 1 [ns]    
        [FieldOffset(4)]
        public UInt32 G1End;            // End of gate 1 [ns]
        [FieldOffset(8)]
        public UInt32 G2Begin;          // Begin of gate 2 [ns]
        [FieldOffset(12)]
        public UInt32 G2End;            // End of gate 2 [ns]
        [FieldOffset(16)]
        public UInt32 GIFBegin;         // Begin of IF gate [ns]
        [FieldOffset(20)]
        public UInt32 GIFEnd;           // End of IF gate [ns]
        [FieldOffset(24)]
        public Byte G1Level;            // Gate 1 level [%]
        [FieldOffset(25)]
        public Byte G2Level;            // Gate 2 level [%]
        [FieldOffset(26)]
        public Byte GIFLevel;           // IF gate level [%]
        [FieldOffset(27)]
        public Byte G1AlarmFilterLevel; // Gate 1 alarm filter level dB.
        // Warning: This byte is shared with G2AlarmFilterLevel
        // G1FilterLevel = mAscan.G1AlarmFilterLevel & Ascan.G1_FILTER_LEVEL_MASK;
        [FieldOffset(27)]
        public Byte G2AlarmFilterLevel; // Gate 2 alarm filter level dB.
        // Warning: This byte is shared with G1AlarmFilterLevel
        // G2FilterLevel = (mAscan.G2AlarmFilterLevel & Ascan.G2_FILTER_LEVEL_MASK)>>4;
        [FieldOffset(28)]
        public UInt32 AscanBegin;       // Begin of the A-scan in [ns].      

        [FieldOffset(56)]
        public Byte G1Amp;	            // Gate 1 Amplitude [%]
        [FieldOffset(57)]
        public Byte G1QofC;             // Quality of coupling [%]
        [FieldOffset(57)]
        public Byte G1WTmsb;             // WallThickness(msb) .
        [FieldOffset(58)]
        public GateFlags G1Flags;	    // Alarms amplitude and distance
        [FieldOffset(59)]
        public GateInAscan G1InAscan;	// Gate into Ascan status
        [FieldOffset(59)]
        public Byte Enable;	            // Warning: This Byte is shared with G1InAscan
        // Enable = mAscan.Enable & Ascan.ENABLE_MASK;
        [FieldOffset(60)]
        public UInt32 G1TofWt;			// In steps of 5 [ns].(DAC) (24 bits)
        // Warning: This UInt32 is shared with G1Amp.
        // TofWt = mAscan.G1TofWt & Ascan.TOF_MASK;
        [FieldOffset(63)]
        public Byte G1WT;			    // Output DAC [%].
        [FieldOffset(63)]
        public Byte G1WTlsb;			// WallThickness(lsb)

        [FieldOffset(64)]
        public Byte G2Amp;	            // Gate 2 Amplitude
        [FieldOffset(65)]
        public Byte G2QofC;             // Quality of coupling [%]
        [FieldOffset(65)]
        public UInt16 G2WTmsb; 			// WallThickness(nsb) 
        [FieldOffset(66)]
        public GateFlags G2Flags;       // Alarms amplitude and distance 
        [FieldOffset(67)]
        public GateInAscan G2InAscan;	// Gate into Ascan status
        [FieldOffset(68)]
        public UInt32 G2TofWt;		    // In steps of 5 [ns].
        // Warning: This UInt32 is shared with G1Amp.
        // TofWt = mAscan.G1TofWt & Ascan.TOF_MASK;
        [FieldOffset(71)]
        public Byte G2WTlsb;	        // WallThickness(lsb)
        [FieldOffset(71)]
        public Byte G2WT;			    // Output DAC

        [FieldOffset(72)]
        public Byte GIFWTmsb;			// WallThickness(msb) 
        [FieldOffset(73)]
        public Byte ChannelNumber;  	// Channel index
        [FieldOffset(74)]
        public GateIFFlags GIFFlags;    // Alarms amplitude and distance
        [FieldOffset(75)]
        public GateIFInAscan GIFInAscan;// Gate into Ascan status
        [FieldOffset(76)]
        public UInt32 GIFTof;		    // In steps of 5 [ns].
        // Warning: This UInt32 is shared with Channel index.
        // TofWt = mAscan.G1TofWt & Ascan.TOF_MASK;
        [FieldOffset(79)]
        public Byte GateIFWT;			// Output DAC
        [FieldOffset(79)]
        public Byte GateIWTlsb;			// WallThickness(lsb) 
        [FieldOffset(80)]
        public UInt16 PRFAlarm;		    // PRF Alarm bit 0x2
        [FieldOffset(82)]
        public AlarmFlags Alarms;       // Alarms flags: Cycle, PRF and power
        [FieldOffset(92)]
        public UInt16 DataSize;			// Amount of points in the a-scan.
        [FieldOffset(96)]
        public UInt32 TimeEqu;			// Time equivalent in [ns].
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        [FieldOffset(100)]
        public Byte[] Points;			// Ascan points.
    }

    // AcqCscan structure is designed to convert an Cscan binary buffer coming from acquisition to a data structure.
    [Serializable()]
    [StructLayout(LayoutKind.Explicit)]
    public struct AcqAscan
    {
        public const Byte ENABLE_MASK = 0x20;
        public const UInt32 TOF_MASK = 0xFFFFFF;
        public const Byte G1_FILTER_LEVEL_MASK = 0x0F;
        public const Byte G2_FILTER_LEVEL_MASK = 0xF0;

        [FlagsAttribute]
        public enum GateFlags : byte
        {
            CouplingAlarm = 0x1,
            DET = 0x2,
            ThicknessMin = 0x4,
            ThicknessMax = 0x8,
            Format16bits = 0x10
        };
        [FlagsAttribute]
        public enum GateInAscan : byte
        {
            GateStartInAscan = 0x2,
            GateEndInAscan = 0x10
        };
        [FlagsAttribute]
        public enum GateIFFlags : byte
        {
            CouplingAlarm = 0x1,
            DET = 0x2,
            EnablePin6 = 0x4,
            Format16bits = 0x10,
        };

        [FieldOffset(0)]
        public UInt32 ScanCounter;      // Number of acquisitions
        [FieldOffset(4)]
        public UInt32 PulserCounter;    // Number of shots from start of acquisition

        [FieldOffset(8)]
        public Byte G1Amp;	          // Gate 1 amplitude [%] 
        [FieldOffset(9)]
        public Byte G1WTmsb;		  // WallThickness (msb) [time] 
        [FieldOffset(9)]
        public Byte G1QofC;		      // Quality of Coupling [%]
        [FieldOffset(10)]
        public GateFlags G1Flags;	    // Alarms amplitude and distance
        [FieldOffset(12)]
        public UInt32 G1Tof;			// In steps of 5 [ns].
        // Warning: This UInt32 is shared with G1Amp.
        // MyTofWt = Ascan.G1TofWt & AcqCscan.TOF_MASK;
        [FieldOffset(15)]
        public Byte G1WTDac;			// Output DAC
        [FieldOffset(15)]
        public Byte G1WTlsb;			// WallThickness (lsb) [time] 

        [FieldOffset(16)]
        public Byte G2Amp;	            // Gate 2 amplitude [%]
        [FieldOffset(17)]
        public Byte G2WTmsb; 			// WallThickness (msb) [time] 
        [FieldOffset(17)]
        public Byte G2QofC; 			// Quality of Coupling [%]
        [FieldOffset(18)]
        public GateFlags G2Flags;       // Alarms amplitude and distance 
        [FieldOffset(19)]
        public GateInAscan G2InAscan;	// Gate into Ascan status
        [FieldOffset(20)]
        public UInt32 G2Tof;		    // In steps of 5 [ns].
        // Warning: This UInt32 is shared with G2Amp.
        // MyTofWt = Ascan.G2TofWt & AcqCscan.TOF_MASK;
        [FieldOffset(23)]
        public Byte G2WTDac;			// Output DAC
        [FieldOffset(23)]
        public Byte G2WTlsb;			//WallThickness (lsb) [time]  

        [FieldOffset(24)]
        public Byte GIFCouplingAlarm;  	// Gate IF Coupling alarm
        [FieldOffset(24)]
        public UInt16 GIFWTmsb;			// WallThickness (msb) [time]
        [FieldOffset(25)]
        public Byte Channel;			// Channel index   
        [FieldOffset(26)]
        public GateIFFlags GIFFlags;    // Alarms amplitude and distance
        [FieldOffset(28)]
        public UInt32 GIFTof;		    // In steps of 5 [ns].
        // Warning: This UInt32 is shared with Channel index.
        // MyTofWt = Ascan.GIFTofWt & AcqCscan.TOF_MASK;
        [FieldOffset(31)]
        public Byte GIFWTDac;		// Output DAC
        [FieldOffset(31)]
        public Byte GIFWTlsb;		// WallThickness (msb) [time]
    }

    // AscanInfo structre must hold parameters requested to display Ascan and gates
    [Serializable()]
    public struct AscanInfo
    {
        public enum VideoMode
        {
            RF,
            PositiveWave,
            NegativeWave,
            FullWave
        };

        public enum PhaseType
        {
            PositiveWave,
            NegativeWave,
            FullWave,
            RF
        };

        public VideoMode Video;
        public double ZeroVideo;
        public PhaseType GIFPhase;
        public PhaseType G1Phase;
        public PhaseType G2Phase;

        public double gate1_trigger;
        public double gate1_position;
        public double gate1_width;
        public double gate1_level;
        public double gate2_trigger;
        public double gate2_position;
        public double gate2_width;
        public double gate2_level;
        public double gateIF_trigger;
        public double gateIF_position;
        public double gateIF_width;
        public double gateIF_level;
        public double scope_trigger;
        public double scope_offset;
        public double scope_range;
        public double gate1_level_alarm;
        public double gate2_level_alarm;
    }

    // Информация о текущем статусе захвата
    [Serializable()]
    public struct AcqSatus
    {
        public Int32 status;
        public Int32 NumberOfScansAcquired;
        public Int32 NumberOfScansRead;
        public Int32 bufferSize;
        public Int32 scanSize;
    }
    
    #endregion Data structures

    #region constants
    [FlagsAttribute]
    public enum ErrorCode : uint
    {
        // Generic errors
        PCXUS_NO_ERROR = 0,
        PCXUS_ERROR = 1,

        // Base error codes
        PCXUS_OPEN_ERROR = 0x10000000,
        PCXUS_CLOSE_ERROR = 0x20000000,
        PCXUS_READ_ERROR = 0x30000000,
        PCXUS_WRITE_ERROR = 0x40000000,
        PCXUS_LOAD_ERROR = 0x50000000,
        PCXUS_SAVE_ERROR = 0x60000000,
        PCXUS_ASCAN_ERROR = 0x70000000,
        PCXUS_ACQ_CONFIG_ERROR = 0x80000000,
        PCXUS_ACQ_START_ERROR = 0x81000000,
        PCXUS_ACQ_READ_ERROR = 0x82000000,
        PCXUS_ACQ_STOP_ERROR = 0x83000000,
        PCXUS_ACQ_CLEAR_ERROR = 0x84000000,
        PCXUS_ACQ_STATUS_ERROR = 0x85000000,
        PCXUS_ACQ_START_PA_ERROR = 0x86000000,
        PCXUS_ACQ_STOP_PA_ERROR = 0x87000000,
        PCXUS_DUPLIC_ERROR = 0x90000000,

        // Additional codes for checking input parameters
        PCXUS_INVALID_BOARD = 0x100,
        PCXUS_INVALID_CHANNEL = 0x200,
        PCXUS_INVALID_DEVICE = 0x300,
        PCXUS_INVALID_PROBE = 0x400,
        PCXUS_INVALID_SETTING = 0x500,
        PCXUS_INVALID_PARAMETER = 0x600,
        PCXUS_INVALID_BOOT = 0x700,

        // Additional codes for system errors
        PCXUS_INVALID_HANDLE_ERROR = 0x1000,
        PCXUS_EXCEPTION = 0x2000,
        PCXUS_INVALID_POINTER_ERROR = 0x3000,
        PCXUS_ALLOC_ERROR = 0x4000,

        // Additional codes for acquisition
        PCXUS_ACQ_CONFIG_MODE_ERROR = 2,
        PCXUS_ACQ_CONFIG_START_ERROR = 3,
        PCXUS_ACQ_CONFIG_ALARM_ERROR = 4,
        PCXUS_ACQ_CONFIG_PREPOST_TRIGGER_ERROR = 5,
        PCXUS_ACQ_CONFIG_DISCRIMINATOR_ERROR = 6,
        PCXUS_ACQ_CONFIG_BUFFER_SIZE_ERROR = 7,
        PCXUS_ACQ_CONFIGURED_ERROR = 8,
        PCXUS_ACQ_NO_CONFIG_ERROR = 0xB,
        PCXUS_ACQ_RUNNING_ERROR = 0xC,
        PCXUS_ACQ_RUNNING_HR_ERROR = 0xD,
        PCXUS_ACQ_INCOMPLET_ERROR = 0xE,
        PCXUS_ACQ_TIMEOUT_ERROR = 0xF,
        PCXUS_ACQ_CONFIG_HR_DIGIT_ERROR = 0x15,
        PCXUS_ACQ_CONFIG_HR_GATE_ERROR = 0x17,
        PCXUS_ACQ_CONFIG_PRF_ERROR = 0x18,
        PCXUS_ACQ_CONFIG_ECHO_MAX_ERROR = 0x20,
        PCXUS_ACQ_CONFIG_FILTER_ERROR = 0x21,
        PCXUS_ACQ_FIFO_OVERFLOW_ERROR = 0x22,

        // Additional miscellaneous codes
        PCXUS_INVALID_SERIAL_NUMBER = 0x5000,
        PCXUS_EXTENSION_BOARD_NOT_FOUND = 0x5001,
        PCXUS_CONNECT_ERROR = 0x5002,
        PCXUS_INVALID_NUMBER_OF_BOARDS = 0x5003,
        PCXUS_INVALID_PACKAGE = 0x5004,
        PCXUS_FILE_PROBE_ERROR = 0x5005,
        PCXUS_FILE_LAW_ERROR = 0x5006,
        PCXUS_INVALID_INDEX = 0x5007,
        PCXUS_FILE_EXTERNAL_ERROR = 0x5008,
        PCXUS_NOT_OPEN = 0x5009,
        PCXUS_SET_PARAM_ERROR = 0x500A,
        PCXUS_GET_PARAM_ERROR = 0x500B,
        PCXUS_ALREADY_OPEN = 0x500C,
        PCXUS_ERR_CONVERT = 0x500D,
        PCXUS_BOARD_NOT_FOUND = 0x500E,
        PCXUS_RS_ERROR = 0x500F,
        PCXUS_TOO_MANY_API = 0x5010,
        PCXUS_CHECK_ERROR = 0x5011,
        PCXUS_CLR_SENSIBILITY_ERROR = 0x5012,

        // Additional codes for setup
        PCXUS_INVALID_EXTENSION_BOARD = 0x6010,
        PCXUS_MUX_NOT_FOUND = 0x6011,
        PCXUS_MUX_HIGH_NOT_FOUND = 0x6012,
        PCXUS_RESET_ERROR = 0x6013,
        PCXUS_LOAD_FIRMWARE1_ERROR = 0x6014,
        PCXUS_LOAD_FIRMWARE2_ERROR = 0x6015,
        PCXUS_LOAD_FIRMWARE3_ERROR = 0x6016,
        PCXUS_SINGLE_CHANNEL_BOARD_REQUESTED = 0x6017,
        PCXUS_MULTI_CHANNELS_BOARD_REQUESTED = 0x6018,
        PCXUS_HIGH_VERSION_BOARD_REQUESTED = 0x6019,
        PCXUS_DAC_SUPPORT_ERROR = 0x601A,

        PCXUS_UNKNOWN_ERROR = 0xFFFFFFFF
    }

    public enum UnitCode : int
    {
        // Basic units (compatible with USPC3100)
        UNIT_NONE = -1,
        UNIT_us = 0,
        UNIT_mm = 1,
        UNIT_in = 2,
    }

    public enum AcqMode : int
    {
        CSCAN_MODE = 0x800, //Gates (Amplitudes, TOF/WT, Alarms) ,(not supported by version I and MB)
        ASCAN_MODE = 0x1000,//A-Scan and Gates; (A-Scan resolution depends of digitising mode and filter selected),(only for hardware version LA, MBA and MHA(*))         
        HR_MODE = 0x2000,   //A-Scan HR;  A-Scan resolution is fixed by FrequencyDivider parameter (gates information excluded) ),(only for hardware version LA).
    }

    public enum ACQ_STATUS : int
    {
        ACQ_NO_CONFIGURED = 0,
        ACQ_RUNNING = 1,
        ACQ_FINISHED_WITH_SCANSBACKLOG = 2,
        ACQ_FINISHED_WITHOUT_SCANSBACKLOG = 3,
        ACQ_WAITING_START = 4,
        ACQ_FIFO_ERROR = 5,
        ACQ_BUFFER_FULL = 6,
        ACQ_BLOC_SIZE_ERROR = 7
    }
    #endregion constants

    public class PCXUS
    {
      
        #region DLL_IMPORTS
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_Open(out Int32 hPCXUS, Int32 boot);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_Close(Int32 hPCXUS);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_Load(Int32 Board, Int32 Test, String file);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_Save(Int32 Board, Int32 Test, String file);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_WRITE(Int32 hPCXUS, Int32 Board, Int32 Test, Int32 Unit, String strParam, ref Double dblValue, [In,Out] Double[] dblArrayValue1, [In,Out] Double[] dblArrayValue2, StringBuilder strValue, ref Int32 Clipped);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_READ(Int32 hPCXUS, Int32 Board, Int32 Test, Int32 Unit, String strParam, ref Double dblValue, [In,Out] Double[] dblArrayValue1, [In,Out] Double[] dblArrayValue2, StringBuilder strValue);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_ASCAN(Int32 Board, Int32 Test, [In,Out] ref Ascan ascan, Int32 Timeout);
        [DllImport("DLL_PCXUS.dll")]
        private static extern void PCXUS_Get_Alarms(ref Int32[] PowerAlarm, ref Int32[] PRFAlamm, ref Int32[] CycleAram);
        [DllImport("DLL_PCXUS.dll")]
        private static extern void PCXUS_Clear_All_Alarms();
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_CONFIG(Int32 hPCXUS, Int32 Board, Int32 AcqMode, Int32 StartMode, Int32[] AcqCondition, Int32 PrePostScans, Int32 FrequencyDivider, Int32 BufferSize, ref Int32 InterruptFluidity, ref Int32 Param);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_GET_STATUS(Int32 Board, ref Int32 Status, ref Int32 NumberOfScansAcquired, ref Int32 NumberOfScansRead, ref Int32 BufferSize, ref Int32 ScanSize);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_START(Int32 hPCXUS, Int32 Board, Int32 NumberOfScansToAcquire);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_READ(Int32 hPCXUS, Int32 Board, Int32 NumberOfScansToRead, Int32 TimeOut, ref Int32 NumberRead, ref Int32 ScansBacklog, [In, Out] byte[] pData);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_READ(Int32 hPCXUS, Int32 Board, Int32 NumberOfScansToRead, Int32 TimeOut, ref Int32 NumberRead, ref Int32 ScansBacklog, [In, Out] AcqAscan[] pData);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_STOP(Int32 hPCXUS, Int32 Board);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_ACQ_CLEAR(Int32 hPCXUS, Int32 Board);
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_Get_number_of_boards();
        [DllImport("DLL_PCXUS.dll")]
        private static extern Int32 PCXUS_Get_Serial_Number(Int32 Board, out Int32 SerialNumber);
        [DllImport("DLL_PCXUS.dll")]
        private static extern uint PCXUS_Get_MUX_RCPP_Serial_Number(Int32 Board, out Int32 SerialNumber);
        #endregion DLL_IMPORTS
        public const int MAX_ROW = 100;
        public const int MAX_STRING = 256;

        private Int32 hPCXUS = 0;
        private int error = 0;
        //работает после открытия платы
        public int numBoards
        {
            get
            {
                return (int)PCXUS_Get_number_of_boards();
            }
        }

        public int Err
        {
            get
            {
                return error;
            }
        }
        
        //работает после открытия платы
        public int serialNumber(int _board = 0)
        {
            int serialNumber;
            error = PCXUS_Get_Serial_Number(_board, out serialNumber);
            return serialNumber;
        }

        public PCXUS()
        {
        }

        ~PCXUS()
        {
            close();
        }

        private bool checkHandle()
        {
            if (hPCXUS == 0)
            {
                log.add(LogRecord.LogReason.error, "board not open: hPCXUS = {0}", hPCXUS);
                error = (int)ErrorCode.PCXUS_INVALID_HANDLE_ERROR;
                return false;
            }
            else
                return true;
        }

        public double setParamValueDouble(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            double dblValue = 0;
            double[] dblArrayValue1 = new double[PCXUS.MAX_ROW];
            double[] dblArrayValue2 = new double[PCXUS.MAX_ROW];
            StringBuilder strValue = new StringBuilder(PCXUS.MAX_STRING);
            int Clipped = 0;
            error = PCXUS.PCXUS_WRITE(hPCXUS, _board, _test, (int)_unit, _paramName, ref dblValue, dblArrayValue1, dblArrayValue2, strValue, ref Clipped);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: PCXUS_READ error : 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return dblValue;
            }
            else
            {
                if (Clipped != 0)
                {
                    log.add(LogRecord.LogReason.info, "{0}:{1}: {2} = {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Clipped", Clipped);
                    error = (int)ErrorCode.PCXUS_INVALID_SETTING;
                }
                log.add(LogRecord.LogReason.info, "{0}:{1}: {2} = {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _paramName, dblValue);
                return dblValue;
            }
        }



        public double getParamValueDouble(string _paramName,int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us )
        {
            double dblValue = 0;
            double[] dblArrayValue1 = new double[PCXUS.MAX_ROW];
            double[] dblArrayValue2 = new double[PCXUS.MAX_ROW];
            StringBuilder strValue = new StringBuilder(PCXUS.MAX_STRING);
            error = PCXUS.PCXUS_READ(hPCXUS, _board, _test, (int)_unit, _paramName, ref dblValue, dblArrayValue1, dblArrayValue2, strValue);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: PCXUS_READ error : 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return dblValue;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}:{1}: {2} = {3}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _paramName, dblValue);
                return dblValue;
            }
        }

        public string getParamValueString(string _paramName, int _board = 0, int _test = 0, UnitCode _unit = UnitCode.UNIT_us)
        {
            double dblValue = 0;
            double[] dblArrayValue1 = new double[PCXUS.MAX_ROW];
            double[] dblArrayValue2 = new double[PCXUS.MAX_ROW];
            StringBuilder strValue = new StringBuilder(PCXUS.MAX_STRING);
            error = PCXUS.PCXUS_READ(hPCXUS, _board, _test, (int)_unit, _paramName, ref dblValue, dblArrayValue1, dblArrayValue2, strValue);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: PCXUS_READ error : 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return null;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}:{1}: {2} = \"{3}\"", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _paramName, strValue.ToString());
                return strValue.ToString();
            }
        }



        public bool open(Int32 _mode)
        {
            if (hPCXUS != 0)
            {
                error = (int)ErrorCode.PCXUS_ALREADY_OPEN;
                log.add(LogRecord.LogReason.warning, "{0}: {1}: Предупреждение: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Плата уже открыта");
                return false;
            }
            try
            {
                error = PCXUS_Open(out hPCXUS, _mode);
                if (error != 0)
                {
                    log.add(LogRecord.LogReason.error, "{0}: {1}: Error: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                    return false;
                }
                else
                {
                    log.add(LogRecord.LogReason.info, "{0}: {1}: PCXUS opened: hPCXUS = {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, hPCXUS);
                    return true;

                }
            }
            catch (Exception ex)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: Error: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                error = 0x00000001;
                return false;
            }
        }
        
        
        public bool close()
        {
            if (!checkHandle()) return false;
            error = PCXUS_Close(hPCXUS);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                hPCXUS = 0;
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "PCXUS closed");
                return true;
            }
        }

        public bool load(string _fName, int _board = -1, int _test = -1)
        {
            if (!checkHandle()) return false;
            error = PCXUS_Load(_board, _test, _fName);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info,"{0}: {1}: Configuration file \"{2}\" loaded", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _fName);
                return true;
            }
        }

        public bool save(string _fName, int _board = -1, int _test = -1)
        {
            if (!checkHandle()) return false;
            error = PCXUS_Save(_board, _test, _fName);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: Configuration file \"{2}\" saved", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, _fName);
                return true;
            }
        }

        public bool config(Int32 _board, Int32 _bufferSize)
        {
            if(!checkHandle())return false;
            Int32 AcqMode = 0x800; 
            Int32 StartMode = 1;
            Int32[] Conditions = new Int32[8];
            Int32 PrePostScans = 0;
            Int32 FrequencyDivider = 0;
            Int32 InterruptFluidity = 464;
            //Int32 InterruptFluidity = 64;
            Int32 Param = 0;
            // Setup acquisition
            error = PCXUS.PCXUS_ACQ_CONFIG(
                hPCXUS,
                _board,
                AcqMode,
                StartMode,
                Conditions,
                PrePostScans,
                FrequencyDivider,
                _bufferSize,
                ref InterruptFluidity,
                ref Param);
            // Display Fluidity come back
            log.add(LogRecord.LogReason.info,"PCXUS.PCXUS_ACQ_CONFIG: InterruptFluidity = {0}", InterruptFluidity);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "success");                
                return true;
            }
        }

        public bool status(Int32 _board, ref Int32 _status, ref Int32 _NumberOfScansAcquired, ref Int32 _NumberOfScansRead, ref Int32 _BufferSize, ref Int32 _scanSize)
        {
            if (!checkHandle()) return false;
            error = PCXUS_ACQ_GET_STATUS(_board, ref _status, ref _NumberOfScansAcquired, ref _NumberOfScansRead, ref _BufferSize, ref _scanSize);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "success");
                return true;
            }
        }

        public bool start(Int32 _board)
        {
            if (!checkHandle()) return false;
            Int32 error = PCXUS_ACQ_START(hPCXUS, _board, -1);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "success");
                return true;
            }
            
        }

        public bool stop(Int32 _board)
        {
            if (!checkHandle()) return false;
            error = PCXUS_ACQ_STOP(hPCXUS, _board);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "success");
                return true;
            }
        }
        public bool clear(Int32 _board)
        {
            if (!checkHandle()) return false;
            error = PCXUS_ACQ_CLEAR(hPCXUS, _board);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "success");
                return true;
            }
        }
        public Int32 read(Int32 _board, byte[] _data, int _timeout = 200)
        {
            if (!checkHandle()) return 0;
            Int32 NumberOfRead = 0;
            Int32 ScansBacklog = 0;
            error = PCXUS_ACQ_READ(hPCXUS, _board, 0, _timeout, ref NumberOfRead, ref ScansBacklog, _data);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return 0;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2} = {3}, {4} = {5}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "NumberOfRead", NumberOfRead, "ScansBacklog", ScansBacklog);
                return NumberOfRead;
            }
        }
        public Int32 read(Int32 _board, AcqAscan[] _data, int _timeout = 200)
        {
            if (!checkHandle()) return 0;
            Int32 NumberOfRead = 0;
            Int32 ScansBacklog = 0;
            error = PCXUS_ACQ_READ(hPCXUS, _board, 0, _timeout, ref NumberOfRead, ref ScansBacklog, _data);
            if (error != 0)
            {
                log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return 0;
            }
            else
            {
                log.add(LogRecord.LogReason.info, "{0}: {1}: {2} = {3}, {4} = {5}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "NumberOfRead", NumberOfRead, "ScansBacklog", ScansBacklog);
                return NumberOfRead;
            }
        }

        public bool readAscan(ref Ascan ascan,int _timeout = 100, int _board = 0, int _test = 0)
        {

            error = PCXUS_ACQ_ASCAN(_board, _test, ref ascan, _timeout);
            if (error != 0)
            {
                //log.add(LogRecord.LogReason.error, "{0}: {1}: 0x{2:X8}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, error);
                return false;
            }
            else
            {
                //log.add(LogRecord.LogReason.info, "{0}: {1}: {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, "Success");
                return true;
            }
        }

        public AscanInfo GetAscanInfo(int _board, int _channel)
        {
            AscanInfo info = new AscanInfo();

            // Part 1.  This part gets parameters to display Ascan according to: 
            //          - the scope video mode
            //          - the scope zero calibration
            //          To display gates according to:
            //          - the wave alternance selection (phase)   
            
            info.Video = (AscanInfo.VideoMode)getParamValueDouble("scope_video",_board,_channel);
            info.ZeroVideo = getParamValueDouble("scope_zero", _board, _channel);
            info.GIFPhase = (AscanInfo.PhaseType)getParamValueDouble("gateIF_phase",_board,_channel);
            info.G1Phase = (AscanInfo.PhaseType)getParamValueDouble("gate1_phase", _board, _channel);
            info.G2Phase = (AscanInfo.PhaseType)getParamValueDouble("gate2_phase", _board, _channel);

            // Part 2.  This part gets parameters to convert Ascan data coming from acquisition to Ascan structure ready to display 

            info.gate1_trigger = getParamValueDouble("gate1_trigger", _board, _channel);
            info.gate1_position = getParamValueDouble("gate1_position", _board, _channel);
            info.gate1_width = getParamValueDouble("gate1_width", _board, _channel);
            info.gate1_level = getParamValueDouble("gate1_level", _board, _channel);
            info.gate1_level_alarm = getParamValueDouble("gate1_nb_alarm_level", _board, _channel);

            info.gate2_trigger = getParamValueDouble("gate2_trigger", _board, _channel);
            info.gate2_position = getParamValueDouble("gate2_position", _board, _channel);
            info.gate2_width = getParamValueDouble("gate2_width", _board, _channel);
            info.gate2_level = getParamValueDouble("gate2_level", _board, _channel);
            info.gate2_level_alarm = getParamValueDouble("gate2_nb_alarm_level", _board, _channel);

            info.gateIF_trigger = getParamValueDouble("gateIF_trigger", _board, _channel);
            info.gateIF_position = getParamValueDouble("gateIF_position", _board, _channel);
            info.gateIF_width = getParamValueDouble("gateIF_width", _board, _channel);
            info.gateIF_level = getParamValueDouble("gateIF_level", _board, _channel);

            info.scope_trigger = getParamValueDouble("scope_trigger", _board, _channel);
            info.scope_offset = getParamValueDouble("scope_offset", _board, _channel);
            info.scope_range = getParamValueDouble("scope_range", _board, _channel);

            return info;
        }
    }
}
