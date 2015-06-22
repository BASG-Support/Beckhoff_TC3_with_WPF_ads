using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwinCAT.Ads;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;
 
namespace BASG.TwinCATConnector
{
    struct AxisHandles
    {
        public int handleIn;
        public int handleOut;
    }
    class Axis
    {
        public List<AxisHandles> handles = new List<AxisHandles>();
        //public int handleIn { get; set; }
        //public int handleOut { get; set; }
        public int count
        {
            get { return handles.Count; }
            set
            {
                handles.Clear();
                if (value > 0)
                {
                    for (int i = 0; i < value; i++)
                    {
                        handles.Add(new AxisHandles());
                    }
                }
            }
        }

        public int sizeIn { get; set; }
        public int sizeOut { get; set; }
    }

    class AX_Connector
    {
        private List<Axis> tcAxis = new List<Axis>();
        private List<int> count = new List<int>();
        private List<TcAdsClient> axClient = new List<TcAdsClient>();
        private XDocument tcConnectorConfig;

        #region Properties
        public int get_plcAxs_Count(int PLC)
        {
            return tcAxis[PLC].count;
        }
        #endregion
        #region Connect / Initialise
        public tcFunctionResult Connect()
        {
            tcConnectorConfig = XDocument.Load("UI Config/tcConnectorConfig.xml");
            if (tcConnectorConfig == null)
            {
                Connector_LogManager.LogMessage("AX", "Failed to load configuration file", tcLogType.ERRORS);
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }
            IEnumerable<XElement> _plcList = tcConnectorConfig.Root.Descendants("Plc");
            if (_plcList.Count<XElement>() <= 0)
            {
                Connector_LogManager.LogMessage("AX", "No plc configurations found");
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }

            int _plcTotal = 0;

            foreach (XElement _plc in _plcList)
            {
                axClient.Add(new TcAdsClient());
                tcAxis.Add(new Axis());
                try
                {
                    axClient[_plcTotal].Connect(_plc.Attribute("AmsNetId").Value, int.Parse(_plc.Attribute("AmsPort").Value));
                }
                catch (Exception Ex)
                {
                    Connector_LogManager.LogMessage("AX", string.Format("{0}{1} {2}", "Failed to connect to PLC ", _plcTotal.ToString(), Ex.Message), tcLogType.ERRORS);
                }
                if (axClient[_plcTotal].IsConnected)
                {
                    XElement _varConfig = _plc.Element("AxsVar");
                    if (_varConfig == null)
                    {
                        tcAxis[_plcTotal].count = 0;
                        tcAxis[_plcTotal].sizeIn = 0;
                        tcAxis[_plcTotal].sizeOut = 0;
                        Connector_LogManager.LogMessage("AX", "No axis configured for plc " + _plcTotal.ToString());
                    }
                    else
                    {
                        try
                        {
                            int handle = axClient[_plcTotal].CreateVariableHandle(_varConfig.Attribute("Count").Value);
                            int buffer = (int)(short)axClient[_plcTotal].ReadAny(handle, typeof(short));
                            if (buffer > 0)
                            {
                                tcAxis[_plcTotal].count = buffer; 
                                for (int i = 0; i < tcAxis[_plcTotal].count; i++)
                                {
                                    AxisHandles axishandle = new AxisHandles() { handleIn = 0, handleOut = 0 };
                                    try
                                    {
                                        axishandle.handleIn = axClient[_plcTotal].CreateVariableHandle(_varConfig.Attribute("PlcToHmi").Value);
                                        try
                                        {
                                            axishandle.handleOut = axClient[_plcTotal].CreateVariableHandle(_varConfig.Attribute("HmiToPlc").Value);
                                            tcAxis[_plcTotal].handles[i] = axishandle;
                                        }
                                        catch (Exception ex)
                                        {
                                            Connector_LogManager.LogMessage("AX", "Failed to create handle for HMI2PLC " + ex.Message, tcLogType.ERRORS);
                                            axishandle.handleOut = 0;
                                            break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Connector_LogManager.LogMessage("AX", "Failed to create handle for PLC2HMI " + ex.Message, tcLogType.ERRORS);
                                        axishandle.handleIn = 0;
                                        break;
                                    }
                                }
                                try
                                {
                                    tcAxis[_plcTotal].sizeIn = int.Parse(_varConfig.Attribute("P2HSize").Value);
                                    tcAxis[_plcTotal].sizeOut = int.Parse(_varConfig.Attribute("H2PSize").Value);
                                }
                                catch (Exception ex)
                                {
                                    Connector_LogManager.LogMessage("AX", "Failed to load config for data size " + ex.Message, tcLogType.ERRORS);
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Connector_LogManager.LogMessage("AX", "Failed to obtain the total axis count " + ex.Message, tcLogType.ERRORS);
                            tcAxis[_plcTotal].count = 0;
                            break;
                        }
                    }
                    _plcTotal++;
                }

            }
            return tcFunctionResult.TC_SUCCESS;
        }
        #endregion
        #region Dispose
        public void IDisposable()
        {
            for (int i = 0; i < axClient.Count; i++)
            {
                for (int j = 0; j < tcAxis[i].count; j++)
                {
                    try
                    {
                        axClient[i].DeleteVariableHandle(tcAxis[i].handles[j].handleIn);
                        axClient[i].DeleteVariableHandle(tcAxis[i].handles[j].handleOut);
                    }
                    catch (Exception ex)
                    {
                        Connector_LogManager.LogMessage("AX","Failed to properly dispose handles " + ex.Message,tcLogType.ERRORS);
                    }
                    axClient[i].Dispose();
                }
            }
        }
        #endregion
        #region IsConnected
        public bool IsConnected(int PLC)
        {
            return axClient[PLC].IsConnected;
        }
        #endregion
        public tcFunctionResult tcSetAxisCommand(int PLC, int AXIS, Axis_HmiToPlc COMMAND)
        {
            if (!axClient[PLC].IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            if (PLC < 0 || PLC >= axClient.Count) return tcFunctionResult.TC_AXIS_OUTOFBOUND;
            if (tcAxis[PLC].count <= 0) return tcFunctionResult.TC_NO_AXIS;
            if (AXIS < 0 || AXIS >= tcAxis[PLC].count) return tcFunctionResult.TC_AXIS_OUTOFBOUND;

            AdsStream _dataStream = new AdsStream(tcAxis.Count * tcAxis[PLC].sizeOut);
            AdsBinaryWriter _dataWriter = new AdsBinaryWriter(_dataStream);
            _dataWriter.Write(COMMAND.CONTROLLER_OVERRIDE);
            _dataWriter.Write(COMMAND.TARGET_VELOCITY);
            _dataWriter.Write(COMMAND.TARGET_ACCELERATION);
            _dataWriter.Write(COMMAND.TARGET_DECELERATION);
            _dataWriter.Write(COMMAND.TARGET_JERK);
            _dataWriter.Write(COMMAND.CONTROLLER_OVERRIDE);
            _dataWriter.Write(COMMAND.SERVO_ON);
            _dataWriter.Write(COMMAND.SERVO_ON_BW);
            _dataWriter.Write(COMMAND.SERVO_ON_FW);
            _dataWriter.Write(COMMAND.SERVO_OFF);
            _dataWriter.Write(COMMAND.SERVO_MOVE_ABS);
            _dataWriter.Write(COMMAND.SERVO_MOVE_REL);
            _dataWriter.Write(COMMAND.SERVO_HALT);
            _dataWriter.Write(COMMAND.SERVO_HOME);
            _dataWriter.Write(COMMAND.SERVO_RESET);
            _dataWriter.Write(COMMAND.SERVO_JOG_MODE);
            _dataWriter.Write(COMMAND.SERVO_JOG_FW_FAST);
            _dataWriter.Write(COMMAND.SERVO_JOG_BW_FAST);
            _dataWriter.Write(COMMAND.SERVO_JOG_FW_SLOW);
            _dataWriter.Write(COMMAND.SERVO_JOG_BW_SLOW);

            try
            {
                axClient[PLC].Write(tcAxis[PLC].handles[AXIS].handleOut, _dataStream);
            }
            catch (Exception ex)
            {
                Connector_LogManager.LogMessage("AX", "Fail to send axis command to PLC " + ex.Message, tcLogType.ERRORS);
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }

            _dataWriter.Close();
            return tcFunctionResult.TC_SUCCESS;
        }
        public tcFunctionResult tcTryGetAxisFeedback(int PLC, int AXIS, ref Axis_PlcToHmi RESULT)
        {
            if (!axClient[PLC].IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            if (PLC < 0 || PLC >= axClient.Count) return tcFunctionResult.TC_AXIS_OUTOFBOUND;
            if (tcAxis[PLC].count <= 0) return tcFunctionResult.TC_NO_AXIS;
            if (AXIS < 0 || AXIS >= tcAxis[PLC].count) return tcFunctionResult.TC_AXIS_OUTOFBOUND;

            AdsStream _dataStream = new AdsStream(tcAxis[PLC].sizeOut);
            AdsBinaryReader _dataReader = new AdsBinaryReader(_dataStream);

            try
            {
                axClient[PLC].Read(tcAxis[PLC].handles[AXIS].handleIn, _dataStream);
                RESULT.actualPosition = _dataReader.ReadDouble();
                RESULT.actualVelocity = _dataReader.ReadDouble();
                RESULT.setPosition = _dataReader.ReadDouble();
                RESULT.setVelocity = _dataReader.ReadDouble();
                RESULT.controlleroverride = _dataReader.ReadDouble();
                RESULT.ErrorID = _dataReader.ReadUInt32();
                RESULT.hasError = _dataReader.ReadBoolean();
                RESULT.isDisabled = _dataReader.ReadBoolean();
                RESULT.isFwDisabled = _dataReader.ReadBoolean();
                RESULT.isBwDisabled = _dataReader.ReadBoolean();
                RESULT.isCalibrated = _dataReader.ReadBoolean();
                RESULT.hasJob = _dataReader.ReadBoolean();
                RESULT.isNotMoving = _dataReader.ReadBoolean();
                RESULT.isPositiveDirection = _dataReader.ReadBoolean();
                RESULT.isNegativeDirection = _dataReader.ReadBoolean();
                RESULT.isInTarget = _dataReader.ReadBoolean();
                RESULT.isInRange = _dataReader.ReadBoolean();
            }
            catch (Exception ex)
            {
                Connector_LogManager.LogMessage("AX", "Fail to get axis feddback from PLC " + ex.Message, tcLogType.ERRORS);
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }

            _dataReader.Close();
            return tcFunctionResult.TC_SUCCESS;
        }
        public Axis_PlcToHmi tcGetAxisFeedback(int PLC, int AXIS)
        {
            if (!axClient[PLC].IsConnected) return null;
            if (PLC < 0 || PLC >= axClient.Count) return null;
            if (tcAxis[PLC].count <= 0) return null;
            if (AXIS < 0 || AXIS >= tcAxis[PLC].count) return null;

            AdsStream _dataStream = new AdsStream(tcAxis[PLC].sizeOut);
            AdsBinaryReader _dataReader = new AdsBinaryReader(_dataStream);

            Axis_PlcToHmi RESULT = new Axis_PlcToHmi();
            try
            {
                axClient[PLC].Read(tcAxis[PLC].handles[AXIS].handleIn, _dataStream);

                RESULT.actualPosition = _dataReader.ReadDouble();
                RESULT.actualVelocity = _dataReader.ReadDouble();
                RESULT.setPosition = _dataReader.ReadDouble();
                RESULT.setVelocity = _dataReader.ReadDouble();
                RESULT.controlleroverride = _dataReader.ReadDouble();
                RESULT.ErrorID = _dataReader.ReadUInt32();
                RESULT.hasError = _dataReader.ReadBoolean();
                RESULT.isDisabled = _dataReader.ReadBoolean();
                RESULT.isFwDisabled = _dataReader.ReadBoolean();
                RESULT.isBwDisabled = _dataReader.ReadBoolean();
                RESULT.isCalibrated = _dataReader.ReadBoolean();
                RESULT.hasJob = _dataReader.ReadBoolean();
                RESULT.isNotMoving = _dataReader.ReadBoolean();
                RESULT.isPositiveDirection = _dataReader.ReadBoolean();
                RESULT.isNegativeDirection = _dataReader.ReadBoolean();
                RESULT.isInTarget = _dataReader.ReadBoolean();
                RESULT.isInRange = _dataReader.ReadBoolean();
            }
            catch (Exception ex)
            {
                Connector_LogManager.LogMessage("AX", "Fail to get axis feddback from PLC " + ex.Message, tcLogType.ERRORS);
                return null;
            }

            _dataReader.Close();
            return RESULT;
        }
    }
}
