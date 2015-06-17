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

namespace ADS_Sample
{
    public partial class DiagnosticPage : Page
    {
        private ObservableCollection<Control.DigitalIndicator> DigitalOutputs = new ObservableCollection<Control.DigitalIndicator>();
        private ObservableCollection<Control.DigitalIndicator> DigitalInputs = new ObservableCollection<Control.DigitalIndicator>();
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private int _AXIS_VIEW = -1;

        public DiagnosticPage()
        {
            InitializeComponent();
            LogView.ItemsSource = TwincatConnector.tcLogList;
        }

        #region Page Init and End
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            XDocument PageConfig = XDocument.Load(@"UI Config/DiagPageConfig.xml");

            #region I/O
            foreach (XElement do_element in PageConfig.Root.Elements("Do"))
            {
                Control.DigitalIndicator LED = new Control.DigitalIndicator();
                LED.Caption = do_element.Attribute("Name").Value;
                LED.DigitalIndicatorClicked += DigitalOutput_Clicked;
                LED.Index = int.Parse(do_element.Attribute("Index").Value);
                DigitalOutputs.Add(LED);
            }
            foreach (XElement di_element in PageConfig.Root.Elements("Di"))
            {
                Control.DigitalIndicator LED = new Control.DigitalIndicator();
                LED.Caption = di_element.Attribute("Name").Value;
                //LED.DigitalIndicatorClicked += DigitalIndicator_Clicked;
                LED.Index = int.Parse(di_element.Attribute("Index").Value);
                DigitalInputs.Add(LED);
            }
            for (int i = 0; i < DigitalOutputs.Count; i++)
            {
                DoContent.Children.Add(DigitalOutputs[i]);
            }
            for (int i = 0; i < DigitalInputs.Count; i++)
            {
                DiContent.Children.Add(DigitalInputs[i]);
            }
            #endregion

            #region AXIS
            for (int i = 0; i < TwincatConnector.Axis_Count; i++)
            {
                TabItem AxisTab = new TabItem();
                AxisTab.Header = "Axis " + i.ToString();
                AxisDiagnostic AxisDiagObj = new AxisDiagnostic();
                AxisDiagObj.ID = i;
                AxisDiagObj.AxisName = "Axis " + (i + 1).ToString();
                AxisDiagObj.ControlSetClicked += AxDiagSetControl_Clicked;
                AxisDiagObj.JogBwFastClicked += AxisDiagObj_JogBwFastClicked;
                AxisDiagObj.JogBwSlowClicked += AxisDiagObj_JogBwSlowClicked;
                AxisDiagObj.JogFwFastClicked += AxisDiagObj_JogFwFastClicked;
                AxisDiagObj.JogFwSlowClicked += AxisDiagObj_JogFwSlowClicked;
                AxisDiagObj.ResetClicked += AxisDiagObj_ResetClicked;
                AxisTab.Content = AxisDiagObj;
                MainContent.Items.Add(AxisTab);
            }
            #endregion

            if (TwincatConnector.IsConnected)
            {
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                dispatcherTimer.Start();
            }
            else TwincatConnector.LogMessage("Not connected to device. Polling not started.");
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }
        #endregion

        #region Timer poll
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (TwincatConnector.IsConnected && TwincatConnector.Count > 0)
            {
                TwincatConnector.tcReadAll();
                for (int i = 0; i < DigitalOutputs.Count; i++)
                {
                    if ((bool)TwincatConnector.tcGetData(DigitalOutputs[i].Index)) DigitalOutputs[i].State = 1;
                    else DigitalOutputs[i].State = 0;
                }
                for (int i = 0; i < DigitalInputs.Count; i++)
                {
                    if ((bool)TwincatConnector.tcGetData(DigitalInputs[i].Index)) DigitalInputs[i].State = 1;
                    else DigitalInputs[i].State = 0;
                }
            }
            if (_AXIS_VIEW >= 0)
            {
                ((AxisDiagnostic)((TabItem)MainContent.SelectedItem).Content).AxisStatus = TwincatConnector.tcGetAxsPlcToHmi(_AXIS_VIEW);
            }
        }
        #endregion

        #region Axis Diagnostic Events
        private void AxDiagSetControl_Clicked(object sender, ADS_Sample.AxisDiagnostic.AxisDiagnosticClickEventArgs e)
        {
            TwincatConnector.tcSetAxisCommand(e.ID, e.COMMAND);
        }
        private void AxisDiagObj_JogBwFastClicked(object sender, ADS_Sample.AxisDiagnostic.AxisDiagnosticClickEventArgs e)
        {
            TwincatConnector.tcSetAxisCommand(e.ID, e.COMMAND);
        }
        private void AxisDiagObj_JogFwFastClicked(object sender, ADS_Sample.AxisDiagnostic.AxisDiagnosticClickEventArgs e)
        {
            TwincatConnector.tcSetAxisCommand(e.ID, e.COMMAND);
        }
        private void AxisDiagObj_JogBwSlowClicked(object sender, ADS_Sample.AxisDiagnostic.AxisDiagnosticClickEventArgs e)
        {
            TwincatConnector.tcSetAxisCommand(e.ID, e.COMMAND);
        }
        private void AxisDiagObj_JogFwSlowClicked(object sender, ADS_Sample.AxisDiagnostic.AxisDiagnosticClickEventArgs e)
        {
            TwincatConnector.tcSetAxisCommand(e.ID, e.COMMAND);
        }
        private void AxisDiagObj_ResetClicked(object sender, ADS_Sample.AxisDiagnostic.AxisDiagnosticClickEventArgs e)
        {
            TwincatConnector.tcSetAxisCommand(e.ID, e.COMMAND);
        }
        #endregion

        #region IO Diagnostic Events
        private void DigitalOutput_Clicked(object sender, ADS_Sample.Control.DigitalIndicator.DigitalIndicationEventArgs e)
        {
            TwincatConnector.tcWriteData(e.Index, !e.State);
            TwincatConnector.LogMessage(string.Format("{0}\t: {1}", "Report","Digital output triggered " + e.Index.ToString()));
        }
        #endregion

        private void MainContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainContent.SelectedIndex > 0) _AXIS_VIEW = ((AxisDiagnostic)((TabItem)MainContent.SelectedItem).Content).ID;
            else _AXIS_VIEW = -1;
        }

    }
}
