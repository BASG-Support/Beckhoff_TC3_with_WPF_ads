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

using TwinCAT.Ads;

namespace ADS_Sample.Control
{
    /// <summary>
    /// Interaction logic for IO.xaml
    /// </summary>
    public partial class DigitalIndicator : UserControl
    {
        private short _ObjectState = 0;
        private string _SymbolName = "";
        private int _SymbolIndex = 0;
        private SolidColorBrush _StateOn = Brushes.Green, _StateOff = Brushes.Red, _StateError = Brushes.Yellow, _StateInvalid = Brushes.Gray;

        #region Properties
        public int Index 
        { 
            get {return _SymbolIndex;} 
            set { _SymbolIndex = value;}
        }
        public short State
        { 
            get { return _ObjectState; } 
            set 
            { 
                _ObjectState = value;
                switch (_ObjectState)
                {
                    case 0:
                        StateIndicator.Fill = _StateOff;
                        break;
                    case 1:
                        StateIndicator.Fill = _StateOn;
                        break;
                    case -1:
                        StateIndicator.Fill = _StateError;
                        break;
                    default:
                        StateIndicator.Fill = _StateInvalid;
                        break;
                }
            } 
        }
        public string Caption
        {
            get { return _SymbolName; }
            set
            {
                _SymbolName = value;
                IndicatorLabel.Content = _SymbolName;
            }
        }
        public SolidColorBrush ColorOn
        {
            get { return _StateOn; }
            set { _StateOn = value; } 
        }
        public SolidColorBrush ColorOff
        {
            get { return _StateOff; }
            set { _StateOff = value; }
        }
        public SolidColorBrush ColorError
        {
            get {  return _StateError; }
            set { _StateError = value; }
        }
        public SolidColorBrush ColorInvalid
        {
            get { return _StateInvalid; }
            set { _StateInvalid = value; }
        }
        #endregion

        public DigitalIndicator()
        {
            InitializeComponent();
            StateIndicator.Fill = _StateInvalid;
        }

        #region Digit Indicator Event
        public class DigitalIndicationEventArgs : EventArgs
        {
            public int Index { get; set;  }
            public bool State { get; set;  }
        }
        
        public event EventHandler<DigitalIndicationEventArgs> DigitalIndicatorClicked;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DigitalIndicationEventArgs _event = new DigitalIndicationEventArgs();
            _event.Index = _SymbolIndex;
            _event.State = (_ObjectState == 1) ? true : false;
            if (DigitalIndicatorClicked != null) DigitalIndicatorClicked(this, _event);
        }
        #endregion
    }
}
