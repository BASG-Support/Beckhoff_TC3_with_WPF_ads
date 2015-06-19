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
    class IO_Connector : Connector_LogManager
    {
        #region Internal data
        private struct tcPlcVar
        {
            public string VariableName;
            public string VariableType;
            public int DataSize;
            public int Count;
            public object Data;
            public int Handle;
        }
        private XDocument tcConnectorConfig;
        private int _plcTotal = 0;
        private List<TcAdsClient> ioClient = new List<TcAdsClient>();
        private List<List<tcPlcVar>> tcVarList = new List<List<tcPlcVar>>();
        #endregion

        #region Properties
        int PLC_Count
        {
            get { return _plcTotal; }
        }
        int get_plcVar_Count(int PLC)
        {
            if (PLC < 0 && PLC > _plcTotal) return -1;
            return tcVarList[PLC].Count;
        }
        #endregion

        #region Disposal
        public void IDisposable()
        {
            if (_plcTotal <= 0) return;
            for (int i = 0; i < _plcTotal; i++)
            {
                if (ioClient[i].IsConnected)
                {
                    int j = tcVarList[i].Count;
                    if (j > 0)
                    {
                        for (int k = 0; k < j; k++)
                        {
                            ioClient[i].DeleteVariableHandle(tcVarList[i][0].Handle);
                            tcVarList[i].RemoveAt(0);
                        }
                    }
                }
            }
        }
        #endregion

        #region Connect to PLC and create handles
        public tcFunctionResult Connect()
        {
            tcConnectorConfig = XDocument.Load("UI Config/tcConnectorConfig.xml");
            if (tcConnectorConfig == null)
            {
                LogMessage("IO", "Failed to load configuration file", tcLogType.ERRORS);
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }
            IEnumerable<XElement> _plcList = tcConnectorConfig.Root.Descendants("Plc");
            if (_plcList.Count<XElement>() <= 0)
            {
                LogMessage("IO", "No plc configurations found");
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }
            
            foreach (XElement _plc in _plcList)
            {
                ioClient.Add(new TcAdsClient());
                tcVarList.Add(new List<tcPlcVar>());
                try
                {
                    ioClient[_plcTotal].Connect(_plc.Attribute("AmsNetId").Value, int.Parse(_plc.Attribute("AmsPort").Value));
                }
                catch (Exception Ex)
                {
                    LogMessage("IO", string.Format("{0}{1} {2}", "Failed to connect to PLC ", _plcTotal.ToString(), Ex.Message), tcLogType.ERRORS);
                }
                if (ioClient[_plcTotal].IsConnected)
                {
                    IEnumerable<XElement> _varConfig = _plc.Descendants("PlcVar");
                    if (_varConfig.Count<XElement>() <= 0)
                    {
                        LogMessage("IO", "No variables configured for plc " + _plcTotal.ToString());
                    }
                    else
                    {
                        int i = 0;
                        foreach (XElement _var in _varConfig)
                        {
                            try
                            {
                                int _handle = ioClient[_plcTotal].CreateVariableHandle(_var.Attribute("Name").Value);
                                tcVarList[_plcTotal].Add(new tcPlcVar() { Count = int.Parse(_var.Attribute("Count").Value), Data = new object(), DataSize = int.Parse(_var.Attribute("Size").Value), VariableName = _var.Attribute("Name").Value, VariableType = _var.Attribute("Type").Value, Handle = _handle });
                            }
                            catch (Exception ex)
                            {
                                LogMessage("IO", "Failed to create variable handle " + ex.Message, tcLogType.ERRORS);
                                break;
                            }
                            i++;
                        }
                    }
                    _plcTotal++;
                }

            }
            return tcFunctionResult.TC_SUCCESS;
        }
        #endregion

        #region Client read data
        public tcFunctionResult IO_ReadPlc(int PLC)
        {
            if (PLC < 0 || PLC > _plcTotal) return tcFunctionResult.TC_VARLIST_OUTOFBOUND;
            if (!ioClient[PLC].IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            int _errors = 0;
            for (int i = 0; i < tcVarList[PLC].Count; i++)
            {
                tcPlcVar _varBuffer = tcVarList[PLC][i];
                AdsStream _dataStream = new AdsStream(_varBuffer.DataSize);
                AdsBinaryReader _dataReader = new AdsBinaryReader(_dataStream);
                try
                {
                    ioClient[PLC].Read(_varBuffer.Handle, _dataStream);
                    switch (_varBuffer.VariableType.ToLower())
                    {
                        case "bool":
                            _varBuffer.Data = (object)_dataReader.ReadBoolean();
                            break;
                        case "int":
                            _varBuffer.Data = (object)_dataReader.ReadInt16();
                            break;
                        case "dint":
                            _varBuffer.Data = (object)_dataReader.ReadInt32();
                            break;
                        case "real":
                            _varBuffer.Data = (object)_dataReader.ReadSingle();
                            break;
                        case "lreal":
                            _varBuffer.Data = (object)_dataReader.ReadDouble();
                            break;
                        default:
                            break;
                 
                    }
                    tcVarList[PLC][i] = _varBuffer;
                }
                catch (Exception ex)
                {
                    LogMessage("IO", string.Format("Fail to read from PLC {0} {1}", PLC.ToString(), ex.Message), tcLogType.ERRORS);
                    _errors++;
                }
                _dataReader.Close();
            }
            if (_errors == 0) return tcFunctionResult.TC_SUCCESS;
            else return tcFunctionResult.TC_PARTIAL_FAILURE;
        }
        public object IO_ReadData(int PLC, int VAR)
        {
            if (PLC < 0 || PLC > _plcTotal) return null;
            if (VAR < 0 || VAR > tcVarList[PLC].Count) return null;
            if (!ioClient[PLC].IsConnected) return null;
            tcPlcVar _varBuffer = tcVarList[PLC][VAR];
            AdsStream _dataStream = new AdsStream(_varBuffer.DataSize);
            AdsBinaryReader _dataReader = new AdsBinaryReader(_dataStream);
            try
            {
                ioClient[PLC].Read(_varBuffer.Handle, _dataStream);
                switch (_varBuffer.VariableType.ToLower())
                {
                    case "bool":
                        _varBuffer.Data = (object)_dataReader.ReadBoolean();
                        break;
                    case "int":
                        _varBuffer.Data = (object)_dataReader.ReadInt16();
                        break;
                    case "dint":
                        _varBuffer.Data = (object)_dataReader.ReadInt32();
                        break;
                    case "real":
                        _varBuffer.Data = (object)_dataReader.ReadSingle();
                        break;
                    case "lreal":
                        _varBuffer.Data = (object)_dataReader.ReadDouble();
                        break;
                    default:
                        break;
                }
                tcVarList[PLC][VAR] = _varBuffer;
                _dataReader.Close();
                return _varBuffer.Data;
            }
            catch (Exception ex)
            {
                LogMessage("IO", string.Format("Fail to read from PLC {0} {1}", PLC.ToString(), ex.Message), tcLogType.ERRORS);
                _dataReader.Close();
                return null;
            }
        }
        public tcFunctionResult IO_SendData(int PLC, int VAR, object DATA)
        {
            if (PLC < 0 || PLC > _plcTotal) return tcFunctionResult.TC_VARLIST_OUTOFBOUND;
            if (VAR < 0 || VAR > tcVarList[PLC].Count) return tcFunctionResult.TC_VARLIST_OUTOFBOUND;
            if (!ioClient[PLC].IsConnected) return tcFunctionResult.TC_NOT_CONNECTED;
            AdsStream _dataStream = new AdsStream(tcVarList[PLC][VAR].DataSize);
            AdsBinaryWriter _dataWriter = new AdsBinaryWriter(_dataStream);
            try
            {
                switch (tcVarList[PLC][VAR].VariableType.ToLower())
                {
                    case "bool":
                        _dataWriter.Write((bool)DATA);
                        break;
                    case "int":
                        _dataWriter.Write((short)DATA);
                        break;
                    case "dint":
                        _dataWriter.Write((int)DATA);
                        break;
                    case "real":
                        _dataWriter.Write((float)DATA);
                        break;
                    case "lreal":
                        _dataWriter.Write((double)DATA);
                        break;
                    default:
                        break;
                }
                ioClient[PLC].Write(tcVarList[PLC][VAR].Handle, _dataStream);
                _dataWriter.Close();
                return tcFunctionResult.TC_SUCCESS;
            }
            catch (Exception ex)
            {
                LogMessage("IO", string.Format("Fail to read from PLC {0} {1}", PLC.ToString(), ex.Message), tcLogType.ERRORS);
                _dataWriter.Close();
                return tcFunctionResult.TC_GENERAL_FAILURE_1;
            }
        }
        #endregion
    }
}
