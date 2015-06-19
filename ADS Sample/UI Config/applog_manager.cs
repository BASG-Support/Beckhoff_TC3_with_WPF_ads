using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace ADS_Sample.UI_Config
{
    #region INTERNAL DATA
    enum appLogType
    {
        REPORT = 0,
        ERRORS
    }
    public class appLogEntry
    {
        public string Time { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string Message { get; set; }
    }
    #endregion
    class applog_manager
    {
        #region PUBLIC DATA
        public static ObservableCollection<appLogEntry> appLogList = new ObservableCollection<appLogEntry>();
        #endregion
        #region Write to log
        public static void appLogMessage(string _location, string _message, appLogType _type = 0)
        {
            string _logtype = (_type == 0) ? "REPORT" : "ERRORS";
            if (appLogList.Count == 50) appLogList.RemoveAt(49);
            appLogList.Insert(0, new appLogEntry() { Time = DateTime.Now.ToString(), Type = _logtype, Location = _location, Message = _message });
            StreamWriter tcLogger = File.AppendText("ui_log.txt");
            tcLogger.WriteLine(string.Format("{0} : {1} \t {2} \t {3}", DateTime.Now.ToString(), _logtype, _location, _message));
            tcLogger.Close();
        }
        #endregion
    }
}
