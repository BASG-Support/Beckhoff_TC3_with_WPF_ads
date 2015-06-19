using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using System.Xml.Linq;

using ADS_Sample.UI_Config;
using BASG.TwinCATConnector;

namespace ADS_Sample
{
    /// <summary>
    /// Interaction logic for Diagnostics.xaml
    /// </summary>
    public partial class Diagnostics : Page
    {
        BASG.TwinCATConnector.IO_Connector Io_viewer = new BASG.TwinCATConnector.IO_Connector();
        private ObservableCollection<Control.DigitalIndicator> DigitalOutputs = new ObservableCollection<Control.DigitalIndicator>();
        private ObservableCollection<Control.DigitalIndicator> DigitalInputs = new ObservableCollection<Control.DigitalIndicator>();
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public Diagnostics()
        {
            InitializeComponent();
            LogView.ItemsSource = applog_manager.appLogList;
            LogView.IsReadOnly = true;
        }
        #region Page start up
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Io_viewer.Connect() == BASG.TwinCATConnector.tcFunctionResult.TC_SUCCESS)
            {
                XDocument PageConfig = XDocument.Load(@"UI Config/DiagPageConfig.xml");
                #region UI Elements
                #region DO
                foreach (XElement do_element in PageConfig.Root.Elements("Do"))
                {
                    Control.DigitalIndicator LED = new Control.DigitalIndicator();
                    LED.Caption = do_element.Attribute("Name").Value;
                    LED.DigitalIndicatorClicked += DigitalOutput_Clicked;
                    LED.Index = int.Parse(do_element.Attribute("Index").Value);
                    DigitalOutputs.Add(LED);
                }
                for (int i = 0; i < DigitalOutputs.Count; i++)
                {
                    DoContent.Children.Add(DigitalOutputs[i]);
                }
                #endregion
                #region DI
                foreach (XElement di_element in PageConfig.Root.Elements("Di"))
                {
                    Control.DigitalIndicator LED = new Control.DigitalIndicator();
                    LED.Caption = di_element.Attribute("Name").Value;
                    LED.Index = int.Parse(di_element.Attribute("Index").Value);
                    DigitalInputs.Add(LED);
                }
                for (int i = 0; i < DigitalInputs.Count; i++)
                {
                    DiContent.Children.Add(DigitalInputs[i]);
                }
                #endregion
                #endregion
                #region START Timer
                try
                {
                    int _m = int.Parse(PageConfig.Root.Element("Polling").Element("m").Value);
                    int _s = int.Parse(PageConfig.Root.Element("Polling").Element("s").Value);
                    int _ms = int.Parse(PageConfig.Root.Element("Polling").Element("ms").Value);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, _m, _s, _ms);
                }
                catch (Exception)
                {
                    applog_manager.appLogMessage("DG", "Failed to load polling config, default values used (500ms)");
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                }
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Start();
                #endregion
            }
        }
        #endregion
        #region Page stopping
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Io_viewer.IDisposable();
            dispatcherTimer.Stop();
        }
        #endregion
        #region Polling Update
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < DigitalOutputs.Count; i++)
            {
                object _buffer = Io_viewer.IO_ReadData(0, DigitalOutputs[i].Index);
                if (_buffer != null) DigitalOutputs[i].State = (bool)_buffer ? (short)1 : (short)0;
                else applog_manager.appLogMessage("DG",string.Format("Failed to get data for DO {0}",DigitalOutputs[i].Index));
            }
            for (int i = 0; i < DigitalInputs.Count; i++)
            {
                object _buffer = Io_viewer.IO_ReadData(0, DigitalInputs[i].Index);
                if (_buffer != null) DigitalInputs[i].State = (bool)_buffer ? (short)1 : (short)0;
                else applog_manager.appLogMessage("DG", string.Format("Failed to get data for DO {0}", DigitalInputs[i].Index));
            }
        }
        #endregion

        private void DigitalOutput_Clicked(object sender, ADS_Sample.Control.DigitalIndicator.DigitalIndicationEventArgs e)
        {
            if (Io_viewer.IO_SendData(0, e.Index, (e.State) ? false : true) != BASG.TwinCATConnector.tcFunctionResult.TC_SUCCESS) applog_manager.appLogMessage("SU", "Failed to trigger output");
        }
        private void MainContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
