using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace BASG.TwinCATConnector
{    
    enum tcLogType
    {
        REPORT = 0,
        ERRORS
    }     
    partial class Connector_LogManager
    {
        #region INTERNAL DATA
        public class tcLogEntry
        {
            public string Time { get; set; }
            public string Type { get; set; }
            public string Location { get; set; }
            public string Message { get; set; }
        }
        #endregion
        #region PUBLIC DATA
        public static ObservableCollection<tcLogEntry> tcLogList = new ObservableCollection<tcLogEntry>();
        #endregion
        #region Write to log
        public static void LogMessage(string _location, string _message, tcLogType _type = 0)
        {
            string _logtype = (_type == 0) ? "REPORT" : "ERRORS";
            if (tcLogList.Count == 50) tcLogList.RemoveAt(49);
            tcLogList.Insert(0, new tcLogEntry() { Time = DateTime.Now.ToString(), Type = _logtype, Location = _location, Message = _message });
            StreamWriter tcLogger = File.AppendText("log.txt");
            tcLogger.WriteLine(string.Format("{0} : {1} \t {2} \t {3}", DateTime.Now.ToString(), _logtype, _location, _message));
            tcLogger.Close();
        }
        #endregion
    }
}
