using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwinCAT.Ads;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace ADS_Sample
{
    #region Function results
    enum tcFunctionResult
    {
        TC_SUCCESS = 0,
        TC_PARTIAL_FAILURE,
        TC_NOT_CONNECTED,
        TC_GENERAL_FAILURE_1,
        TC_FAIL_TO_LOAD_PLC_CONFIG,
        TC_FAIL_TO_CREATE_HANDLE,
        TC_FAIL_TO_READ_DATA,
        TC_FAIL_TO_WRITE_DATA,
        TC_FAIL_TO_CONNECT_DEVICE,
        TC_VARLIST_OUTOFBOUND,
        TC_AXIS_OUTOFBOUND,
        TC_FAIL_TO_READ_AXIS_FEEDBACK,
        TC_NO_AXIS
    }
    #endregion

    #region Data Structure
    struct tcPlcVar
    {
        public string VariableName;
        public string VariableType;
        public int DataSize;
        public int Count;
        public object Data;
        public int Handle;
        public string Tag;
        public int Index;
    }
    public class Axis_PlcToHmi
    {
        public bool isDisabled = false;
        public bool isFwDisabled = false;
        public bool isBwDisabled = false;
        public bool isCalibrated = false;
        public bool hasJob = false;
        public bool isNotMoving = false;
        public bool isPositiveDirection = false;
        public bool isNegativeDirection = false;
        public bool isInTarget = false;
        public bool isInRange = false;
        public bool hasError = false;
        public uint ErrorID = 0;
        public double controlleroverride = 0;
        public double actualPosition = 0, actualVelocity = 0;
        public double setPosition = 0, setVelocity = 0;
    }
    public class Axis_HmiToPlc
    {	
        public double TARGET_POSITION { get; set; }
	    public double TARGET_VELOCITY { get; set; }
	    public double TARGET_ACCELERATION { get; set; }
	    public double TARGET_DECELERATION { get; set; }
	    public double TARGET_JERK { get; set; }
	    public double CONTROLLER_OVERRIDE { get; set; }
        public bool SERVO_HALT { get; set; }
	    public bool SERVO_ON { get; set; }
	    public bool SERVO_ON_FW { get; set; }
	    public bool SERVO_ON_BW { get; set; }
	    public bool SERVO_OFF { get; set; }
	    public bool SERVO_MOVE_ABS { get; set; }
	    public bool SERVO_MOVE_REL { get; set; }
	    public bool SERVO_HOME { get; set; }
	    public bool SERVO_RESET	{ get; set; }
	    public bool SERVO_JOG_MODE { get; set; }
	    public bool SERVO_JOG_FW_FAST { get; set; }
	    public bool SERVO_JOG_BW_FAST { get; set; }
	    public bool SERVO_JOG_FW_SLOW { get; set; }
	    public bool SERVO_JOG_BW_SLOW { get; set; }
    }
    #endregion

    #region Log Entry Class
    class tcLogEntry
    {
        public string Time { get; set; }
        public string Message { get; set; }
    }
    #endregion

    class TwincatConnector
    {
        #region Special Definitions
        public class _Axis_PlcToHmi
        {
            public int handle = 0;
            public short size = 0;
            public bool isDisabled = false;
            public bool isFwDisabled = false;
            public bool isBwDisabled = false;
            public bool isCalibrated = false;
            public bool hasJob = false;
            public bool isNotMoving = false;
            public bool isPositiveDirection = false;
            public bool isNegativeDirection = false;
            public bool isInTarget = false;
            public bool isInRange = false;
            public bool hasError = false;
            public uint ErrorId = 0;
            public double controlleroverride = 0;
            public double actualPosition = 0, actualVelocity = 0;
            public double setPosition = 0, setVelocity = 0;
        }
        public class _Axis_HmiToPlc
        {
            public int handle = 0;
            public short size = 0;
            public double TARGET_POSITION { get; set; }
            public double TARGET_VELOCITY { get; set; }
            public double TARGET_ACCELERATION { get; set; }
            public double TARGET_DECELERATION { get; set; }
            public double TARGET_JERK { get; set; }
            public double CONTROLLER_OVERRIDE { get; set; }
            public bool SERVO_HALT { get; set; }
            public bool SERVO_ON { get; set; }
            public bool SERVO_ON_FW { get; set; }
            public bool SERVO_ON_BW { get; set; }
            public bool SERVO_OFF { get; set; }
            public bool SERVO_MOVE_ABS { get; set; }
            public bool SERVO_MOVE_REL { get; set; }
            public bool SERVO_HOME { get; set; }
            public bool SERVO_RESET { get; set; }
            public bool SERVO_JOG_MODE { get; set; }
            public bool SERVO_JOG_FW_FAST { get; set; }
            public bool SERVO_JOG_BW_FAST { get; set; }
            public bool SERVO_JOG_FW_SLOW { get; set; }
            public bool SERVO_JOG_BW_SLOW { get; set; }
        }
        #endregion

        #region internal use
        private static XDocument tcConnectorConfig;
        private static TwinCAT.Ads.TcAdsClient tcClient = new TcAdsClient();
        private static ObservableCollection<tcPlcVar> tcVarList = new ObservableCollection<tcPlcVar>();
        private static tcFunctionResult tcError;
        public static ObservableCollection<tcLogEntry> tcLogList = new ObservableCollection<tcLogEntry>();

        private static short Axis_MaxAxes = 0;

        private static _Axis_PlcToHmi[] tcAxFeedback;
        private static _Axis_HmiToPlc[] tcAxCommand;

        private static string tcConfigPath = "UI Config/PlcConfig.xml";
        #endregion

        #region Connector Properties
        public string ConfigFilePath { get { return tcConfigPath; } set { tcConfigPath = value; } }
        public static bool IsConnected
        {
            get { return tcClient.IsConnected; }
        }
        public static int Count
        {
            get { return tcVarList.Count; }
        }
        public static int Axis_Count
        {
            get { return (int)Axis_MaxAxes; }
        }
        public string GetError
        {
            get
            {
                switch (tcError)
                {
                    case tcFunctionResult.TC_FAIL_TO_CONNECT_DEVICE:
                        return "Error - Failed to connect to remote device. Check that AMS Net ID and Port is correct. Ensure that the route is also correctly added.";
                    case tcFunctionResult.TC_FAIL_TO_LOAD_PLC_CONFIG:
                        return "Error - Failed find connector configuration file.";
                    case tcFunctionResult.TC_NOT_CONNECTED:
                        return "Error - Device not connected yet.";
                    default:
                        return "No error";
                }
            }
        }
        #endregion

        #region Connects to target device, defaults to local
        public static tcFunctionResult tcConnect()
        {
            try
            {
                tcConnectorConfig = XDocument.Load(tcConfigPath);
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("{0}\t: {1}", "Unable to load PlcConfig.xml", ex.Message));
                return tcFunctionResult.TC_FAIL_TO_LOAD_PLC_CONFIG;
            }
            XElement tcPLC = tcConnectorConfig.Root;//("Plc");
            tcClient = new TcAdsClient();
            try
            {
                tcClient.Connect(tcPLC.Attribute("AmsNetId").Value, int.Parse(tcPLC.Attribute("AmsPort").Value));
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("{0}\t: {1}", "Unable to connect to device", ex.Message));
                return tcFunctionResult.TC_FAIL_TO_CONNECT_DEVICE;
            }
            LogMessage(string.Format("{0}\t: {1}", "Report", "Device connected."));
            return tcFunctionResult.TC_SUCCESS;
        }
        #endregion

        #region Adds a TwinCAT varible handle to the cache
        public static tcFunctionResult tcCreateHandle()
        {
            if (!tcClient.IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;

            bool _io_ok = false, _axis_ok = false;

            #region Creating PLC IO handles
            IEnumerable<XElement> PlcVars = tcConnectorConfig.Root.Elements("PlcVar");
            foreach (XElement PlcVar in PlcVars)
            {
                tcPlcVar PlcVarBuffer = new tcPlcVar();
                try
                {
                    PlcVarBuffer.Handle = tcClient.CreateVariableHandle(PlcVar.Attribute("Name").Value);
                    if (PlcVarBuffer.Handle == 0) return tcFunctionResult.TC_FAIL_TO_CREATE_HANDLE;
                    PlcVarBuffer.VariableName = PlcVar.Attribute("Name").Value;
                    PlcVarBuffer.VariableType = PlcVar.Attribute("Type").Value;
                    PlcVarBuffer.DataSize = int.Parse(PlcVar.Attribute("Size").Value);
                    PlcVarBuffer.Count = int.Parse(PlcVar.Attribute("Count").Value);
                    PlcVarBuffer.Data = new object();
                    PlcVarBuffer.Tag = PlcVar.Attribute("Tag").Value;
                    PlcVarBuffer.Index = int.Parse(PlcVar.Attribute("Index").Value);
                    tcVarList.Add(PlcVarBuffer);
                    _io_ok = true;
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("{0}\t: {1}", "Error handling PlcConfig.xml for IO " + PlcVar.Attribute("Name").Value, ex.Message));
                }
            }
            #endregion

            #region Creating PLC Axis handles
            XElement AxsVar = tcConnectorConfig.Root.Element("AxsVar");
            if (AxsVar == null)
            {
                Axis_MaxAxes = 0;
                LogMessage(string.Format("{0}\t: {1}", "Report", "No axis specified in config file"));
            }
            else
            {
                try
                {
                    int AxCount = tcClient.CreateVariableHandle(AxsVar.Attribute("Count").Value);
                    Axis_MaxAxes = (short)tcClient.ReadAny(AxCount, typeof(short));
                    tcClient.DeleteVariableHandle(AxCount);
                    tcAxFeedback = new _Axis_PlcToHmi[Axis_MaxAxes];
                    tcAxCommand = new _Axis_HmiToPlc[Axis_MaxAxes];
                    for (int k = 0; k < Axis_MaxAxes; k++)
                    {
                        tcAxFeedback[k] = new _Axis_PlcToHmi();
                        tcAxFeedback[k].handle = tcClient.CreateVariableHandle(AxsVar.Attribute("PlcToHmi").Value + "[" + (k+1).ToString() + "]");
                        tcAxFeedback[k].size = short.Parse(AxsVar.Attribute("P2HSize").Value);
                        tcAxCommand[k] = new _Axis_HmiToPlc();
                        tcAxCommand[k].handle = tcClient.CreateVariableHandle(AxsVar.Attribute("HmiToPlc").Value + "[" + (k + 1).ToString() + "]");
                        tcAxCommand[k].size = short.Parse(AxsVar.Attribute("H2PSize").Value);
                    }
                    _axis_ok = true;
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("{0}\t: {1}", "Error", "Cannot create handle for axis " + ex.Message ));
                }
            }
            #endregion

            if (_io_ok && _axis_ok)
            {
                LogMessage(string.Format("{0}\t: {1}", "Report", "Handles created."));
                return tcFunctionResult.TC_SUCCESS;
            }
            else
            {
                LogMessage(string.Format("{0}\t: {1}", "Report", "Not all handles created."));
                return tcFunctionResult.TC_SUCCESS;
            }
        }
        #endregion

        #region Read all data in cache
        public static tcFunctionResult tcReadAll()
        {
            if (!tcClient.IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            for (int i = 0; i < tcVarList.Count; i++)
            {
                AdsStream tcDataStream = new AdsStream(tcVarList[i].DataSize);
                AdsBinaryReader tcStreamReader = new AdsBinaryReader(tcDataStream);
                try
                {
                    tcClient.Read(tcVarList[i].Handle, tcDataStream);
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("{0}\t: {1}", "Failed to read from device", ex.Message));
                    return tcFunctionResult.TC_FAIL_TO_READ_DATA;
                }
                tcPlcVar buffer = tcVarList[i];
                switch (tcVarList[i].VariableType.ToLower())
                {
                    case "bool":
                        buffer.Data = tcStreamReader.ReadBoolean();
                        break;
                    case "arbool":
                        bool[] boolBuffer = new bool[buffer.Count];
                        for (int j = 0; j < boolBuffer.Length; j++)
                        {
                            boolBuffer[j] = tcStreamReader.ReadBoolean();
                        }
                        buffer.Data = (object)boolBuffer;
                        break;
                    case "int":
                        buffer.Data = tcStreamReader.ReadInt16();
                        break;
                    case "arint":
                        short[] shortBuffer = new short[buffer.Count];
                        for (int j = 0; j < shortBuffer.Length; j++)
                        {
                            shortBuffer[j] = tcStreamReader.ReadInt16();
                        }
                        buffer.Data = (object)shortBuffer;
                        break;
                    case "string":
                        buffer.Data = tcStreamReader.ReadPlcString(255);
                        break;
                    case "real":
                        buffer.Data = tcStreamReader.ReadSingle();
                        break;
                    case "lreal":
                        buffer.Data = tcStreamReader.ReadDouble();
                        break;
                }
                tcVarList[i] = buffer;
                tcStreamReader.Close();
            }
            return tcFunctionResult.TC_SUCCESS;
        }
        #endregion

        #region Write data to PLC
        public static tcFunctionResult tcWriteData(int index, object data)
        {
            if (!tcClient.IsConnected)
            {
                LogMessage(string.Format("Error\t: {0}", "Attempted to write to PLC when not connected"));
                return tcFunctionResult.TC_NOT_CONNECTED;
            }
            if (index > tcVarList.Count || index < 0)
            {
                LogMessage(string.Format("Error\t: {0}", "Attempted to access a value outside the list"));
                return tcFunctionResult.TC_VARLIST_OUTOFBOUND;
            }
            AdsStream _DataStrem = new AdsStream(tcVarList[index].DataSize);
            AdsBinaryWriter tcStreamWriter = new AdsBinaryWriter(_DataStrem);
            try
            {
                switch (tcVarList[index].VariableType.ToLower())
                {
                    case "bool":
                        tcStreamWriter.Write((bool)data);
                        tcClient.Write(tcVarList[index].Handle, _DataStrem);
                        break;
                    case "arbool":
                        break;
                    case "int":
                        break;
                    case "arint":
                        break;
                    case "string":
                        break;
                    case "real":
                        break;
                    case "lreal":
                        break;
                }
                tcStreamWriter.Close();
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("Error at writing data {0}\t: {1}", index.ToString(), ex.Message));
                return tcFunctionResult.TC_FAIL_TO_WRITE_DATA;
            }
            finally
            {
                tcStreamWriter.Close();
            }
            return tcFunctionResult.TC_SUCCESS;
        }
        #endregion

        #region Retrieve data from list
        public static object tcGetData(int index)
        {
            if (index >= tcVarList.Count || index < 0) return null;
            return tcVarList[index].Data;
        }
        #endregion

        #region Ends the connection
        public static void tcDispose()
        {
            int totalCount = tcVarList.Count;
            for (int i = 0; i < totalCount; i++)
            {
                int j = totalCount - (i + 1);
                if (tcVarList[j].Handle != 0) tcClient.DeleteVariableHandle(tcVarList[j].Handle);
                tcVarList.RemoveAt(j);
            }

            for (int i = 0; i < Axis_MaxAxes; i++)
            {
                if (tcAxFeedback[i].handle != 0) tcClient.DeleteVariableHandle(tcAxFeedback[i].handle);
                tcAxFeedback[i].handle = 0;
                if (tcAxCommand[i].handle != 0) tcClient.DeleteVariableHandle(tcAxCommand[i].handle);
                tcAxCommand[i].handle = 0;
            }

            tcClient.Dispose();
            LogMessage(string.Format("{0}\t: {1}", "Report", "Device connection terminated."));
        }
        #endregion

        #region Write to log
        public static void LogMessage(string _Message)
        {
            if (tcLogList.Count == 50) tcLogList.RemoveAt(49);
            tcLogList.Insert(0, new tcLogEntry() { Time = DateTime.Now.ToString(), Message = _Message });
            StreamWriter tcLogger = File.AppendText("log.txt");
            tcLogger.WriteLine(string.Format("{0}\t:\t {1}",DateTime.Now.ToString(), _Message));
            tcLogger.Close();
        }
        #endregion

        #region TwinCAT Axis & HMI
        static public tcFunctionResult tcUpdateAxStatus()
        {
            if (!tcClient.IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            if (Axis_MaxAxes <= 0) return tcFunctionResult.TC_NO_AXIS;
            for (int k = 0; k < Axis_MaxAxes; k++)
            {
                AdsStream _DataStream = new AdsStream(tcAxFeedback[k].size);
                AdsBinaryReader _DataReader = new AdsBinaryReader(_DataStream);
                try
                {
                    tcClient.Read(tcAxFeedback[k].handle, _DataStream);
                    tcAxFeedback[k].actualPosition = _DataReader.ReadDouble();
                    tcAxFeedback[k].actualVelocity = _DataReader.ReadDouble();
                    tcAxFeedback[k].setPosition = _DataReader.ReadDouble();
                    tcAxFeedback[k].setVelocity = _DataReader.ReadDouble();
                    tcAxFeedback[k].controlleroverride = _DataReader.ReadDouble();
                    tcAxFeedback[k].isDisabled = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isFwDisabled = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isBwDisabled = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isCalibrated = _DataReader.ReadBoolean();
                    tcAxFeedback[k].hasJob = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isNotMoving = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isPositiveDirection = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isNegativeDirection = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isInTarget = _DataReader.ReadBoolean();
                    tcAxFeedback[k].isInRange = _DataReader.ReadBoolean();
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("{0}\t: {1}", "Error", "Failed to read PlcToHmi" + ex.Message));
                    _DataReader.Close();
                    return tcFunctionResult.TC_FAIL_TO_READ_AXIS_FEEDBACK;
                }
                _DataReader.Close();
            }
            return tcFunctionResult.TC_SUCCESS;
        }
        static public Axis_PlcToHmi tcGetAxsPlcToHmi(int index)
        {
            if (!tcClient.IsConnected) return null;
            if (Axis_MaxAxes <= 0) return null;
            if (index < 0 || index > Axis_MaxAxes - 1) return null;

            AdsStream _DataStream = new AdsStream(tcAxFeedback[index].size);
            AdsBinaryReader _DataReader = new AdsBinaryReader(_DataStream);
            try
            {
                Axis_PlcToHmi _buffer = new Axis_PlcToHmi();
                tcClient.Read(tcAxFeedback[index].handle, _DataStream);
                _buffer.actualPosition = _DataReader.ReadDouble();
                _buffer.actualVelocity = _DataReader.ReadDouble();
                _buffer.setPosition = _DataReader.ReadDouble();
                _buffer.setVelocity = _DataReader.ReadDouble();
                _buffer.controlleroverride = _DataReader.ReadDouble();
                _buffer.ErrorID = _DataReader.ReadUInt32();
                _buffer.hasError = _DataReader.ReadBoolean();
                _buffer.isDisabled = _DataReader.ReadBoolean();
                _buffer.isFwDisabled = _DataReader.ReadBoolean();
                _buffer.isBwDisabled = _DataReader.ReadBoolean();
                _buffer.isCalibrated = _DataReader.ReadBoolean();
                _buffer.hasJob = _DataReader.ReadBoolean();
                _buffer.isNotMoving = _DataReader.ReadBoolean();
                _buffer.isPositiveDirection = _DataReader.ReadBoolean();
                _buffer.isNegativeDirection = _DataReader.ReadBoolean();
                _buffer.isInTarget = _DataReader.ReadBoolean();
                _buffer.isInRange = _DataReader.ReadBoolean();
                return _buffer;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("{0}\t: {1}", "Error", "Failed to read PlcToHmi" + ex.Message));
                _DataReader.Close();
                return null;
            }
        }
        static public tcFunctionResult tcGetAxisFeedback(int index, ref Axis_PlcToHmi RESULT)
        {
            if (!tcClient.IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            if (Axis_MaxAxes <= 0) return tcFunctionResult.TC_NO_AXIS;
            if (index < 0 || index > Axis_MaxAxes - 1) return tcFunctionResult.TC_AXIS_OUTOFBOUND;

            AdsStream _DataStream = new AdsStream(tcAxFeedback[index].size);
            AdsBinaryReader _DataReader = new AdsBinaryReader(_DataStream);
            try
            {
                tcClient.Read(tcAxFeedback[index].handle, _DataStream);

                RESULT.actualPosition = _DataReader.ReadDouble();
                RESULT.actualVelocity = _DataReader.ReadDouble();
                RESULT.setPosition = _DataReader.ReadDouble();
                RESULT.setVelocity = _DataReader.ReadDouble();
                RESULT.controlleroverride = _DataReader.ReadDouble();
                RESULT.ErrorID = _DataReader.ReadUInt32();
                RESULT.hasError = _DataReader.ReadBoolean();
                RESULT.isDisabled = _DataReader.ReadBoolean();
                RESULT.isFwDisabled = _DataReader.ReadBoolean();
                RESULT.isBwDisabled = _DataReader.ReadBoolean();
                RESULT.isCalibrated = _DataReader.ReadBoolean();
                RESULT.hasJob = _DataReader.ReadBoolean();
                RESULT.isNotMoving = _DataReader.ReadBoolean();
                RESULT.isPositiveDirection = _DataReader.ReadBoolean();
                RESULT.isNegativeDirection = _DataReader.ReadBoolean();
                RESULT.isInTarget = _DataReader.ReadBoolean();
                RESULT.isInRange = _DataReader.ReadBoolean();

            }
            catch (Exception ex)
            {
                LogMessage(string.Format("{0}\t: {1}", "Error", "Failed to read PlcToHmi" + ex.Message));
                _DataReader.Close();
                return tcFunctionResult.TC_FAIL_TO_READ_AXIS_FEEDBACK;
            }

            _DataReader.Close();
            return tcFunctionResult.TC_SUCCESS;
        }
        static public tcFunctionResult tcSetAxisCommand(int index, Axis_HmiToPlc COMMAND)
        {
            if (!tcClient.IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            if (Axis_MaxAxes <= 0) return tcFunctionResult.TC_NO_AXIS;
            if (index < 0 || index >= Axis_MaxAxes) return tcFunctionResult.TC_AXIS_OUTOFBOUND;

            tcAxCommand[index].CONTROLLER_OVERRIDE = COMMAND.CONTROLLER_OVERRIDE;
            tcAxCommand[index].SERVO_HOME = COMMAND.SERVO_HOME;
            tcAxCommand[index].SERVO_JOG_BW_FAST = COMMAND.SERVO_JOG_BW_FAST;
            tcAxCommand[index].SERVO_JOG_BW_SLOW = COMMAND.SERVO_JOG_BW_SLOW;
            tcAxCommand[index].SERVO_JOG_FW_FAST = COMMAND.SERVO_JOG_FW_FAST;
            tcAxCommand[index].SERVO_JOG_FW_SLOW = COMMAND.SERVO_JOG_FW_SLOW;
            tcAxCommand[index].SERVO_JOG_MODE = COMMAND.SERVO_JOG_MODE;
            tcAxCommand[index].SERVO_MOVE_ABS = COMMAND.SERVO_MOVE_ABS;
            tcAxCommand[index].SERVO_MOVE_REL = COMMAND.SERVO_MOVE_REL;
            tcAxCommand[index].SERVO_HALT = COMMAND.SERVO_HALT;
            tcAxCommand[index].SERVO_OFF = COMMAND.SERVO_OFF;
            tcAxCommand[index].SERVO_ON = COMMAND.SERVO_ON;
            tcAxCommand[index].SERVO_ON_BW = COMMAND.SERVO_ON_BW;
            tcAxCommand[index].SERVO_ON_FW = COMMAND.SERVO_ON_FW;
            tcAxCommand[index].SERVO_RESET = COMMAND.SERVO_RESET;
            tcAxCommand[index].TARGET_ACCELERATION = COMMAND.TARGET_ACCELERATION;
            tcAxCommand[index].TARGET_DECELERATION = COMMAND.TARGET_DECELERATION;
            tcAxCommand[index].TARGET_JERK = COMMAND.TARGET_JERK;
            tcAxCommand[index].TARGET_POSITION = COMMAND.TARGET_POSITION;
            tcAxCommand[index].TARGET_VELOCITY = COMMAND.TARGET_VELOCITY;

            AdsStream _dataStream = new AdsStream(tcAxCommand[index].size);
            AdsBinaryWriter _dataWriter = new AdsBinaryWriter(_dataStream);

            _dataWriter.Write(tcAxCommand[index].TARGET_POSITION);
            _dataWriter.Write(tcAxCommand[index].TARGET_VELOCITY);
            _dataWriter.Write(tcAxCommand[index].TARGET_ACCELERATION);
            _dataWriter.Write(tcAxCommand[index].TARGET_DECELERATION);
            _dataWriter.Write(tcAxCommand[index].TARGET_JERK);
            _dataWriter.Write(tcAxCommand[index].CONTROLLER_OVERRIDE);
            _dataWriter.Write(tcAxCommand[index].SERVO_ON);
            _dataWriter.Write(tcAxCommand[index].SERVO_ON_BW);
            _dataWriter.Write(tcAxCommand[index].SERVO_ON_FW);
            _dataWriter.Write(tcAxCommand[index].SERVO_OFF);
            _dataWriter.Write(tcAxCommand[index].SERVO_MOVE_ABS);
            _dataWriter.Write(tcAxCommand[index].SERVO_MOVE_REL);
            _dataWriter.Write(tcAxCommand[index].SERVO_HALT);
            _dataWriter.Write(tcAxCommand[index].SERVO_HOME);
            _dataWriter.Write(tcAxCommand[index].SERVO_RESET);
            _dataWriter.Write(tcAxCommand[index].SERVO_JOG_MODE);
            _dataWriter.Write(tcAxCommand[index].SERVO_JOG_FW_FAST);
            _dataWriter.Write(tcAxCommand[index].SERVO_JOG_BW_FAST);
            _dataWriter.Write(tcAxCommand[index].SERVO_JOG_FW_SLOW);
            _dataWriter.Write(tcAxCommand[index].SERVO_JOG_BW_SLOW);
            try
            {
                tcClient.Write(tcAxCommand[index].handle, _dataStream);
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("{0}\t: {1}", "Error", "Failed to send Axis Command " + ex.Message));
                _dataWriter.Close();
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }
            _dataWriter.Close();
            return tcFunctionResult.TC_SUCCESS;
        }
        #endregion

    }
}
