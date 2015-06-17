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

namespace ADS_Sample
{

    public partial class AxisDiagnostic : UserControl
    {
        private Axis_PlcToHmi _internAxisRef = new Axis_PlcToHmi();
        //private Axis_HmiToPlc _internHmiToPlc = new Axis_HmiToPlc();

        #region Properties
        public Axis_PlcToHmi AxisStatus
        {
            set
            {
                _internAxisRef.actualPosition = value.actualPosition;
                AxisActualPosition.Text = _internAxisRef.actualPosition.ToString();

                _internAxisRef.actualVelocity = value.actualVelocity;
                AxisActualVelocity.Text = _internAxisRef.actualVelocity.ToString();

                _internAxisRef.setPosition = value.setPosition;
                AxisSetVelocity.Text = _internAxisRef.setPosition.ToString();
                _internAxisRef.setVelocity = value.setVelocity;
                AxisSetPosition.Text = _internAxisRef.setVelocity.ToString();

                _internAxisRef.controlleroverride = value.controlleroverride;
                AxisOverrideDisplay.Text = _internAxisRef.controlleroverride.ToString();

                _internAxisRef.hasJob = value.hasJob;
                AxisHasJob.IsChecked = _internAxisRef.hasJob;

                _internAxisRef.isBwDisabled = value.isBwDisabled;
                AxisFeedBw.IsChecked = !_internAxisRef.isBwDisabled;
                _internAxisRef.isFwDisabled = value.isFwDisabled;
                AxisFeedFw.IsChecked = !_internAxisRef.isFwDisabled;

                _internAxisRef.isCalibrated = value.isCalibrated;
                AxisCalibrated.IsChecked = _internAxisRef.isCalibrated;

                _internAxisRef.isDisabled = value.isDisabled;
                AxisReady.IsChecked = !_internAxisRef.isDisabled;


                _internAxisRef.isInRange = value.isInRange;
                AxisInRange.IsChecked = _internAxisRef.isInRange;

                _internAxisRef.isInTarget = value.isInTarget;
                AxisInTarget.IsChecked = _internAxisRef.isInTarget;

                _internAxisRef.isNegativeDirection = value.isNegativeDirection;
                AxisMovingBw.IsChecked = _internAxisRef.isNegativeDirection;

                _internAxisRef.isPositiveDirection = value.isPositiveDirection;
                AxisMovingFw.IsChecked = _internAxisRef.isPositiveDirection;

                _internAxisRef.isNotMoving = value.isNotMoving;
                AxisNotMoving.IsChecked = _internAxisRef.isNotMoving;

                _internAxisRef.hasError = value.hasError;
                AxisError.IsChecked = _internAxisRef.hasError;

                _internAxisRef.ErrorID = value.ErrorID;
                AxisErrorID.Text = _internAxisRef.ErrorID.ToString();

            }
        }
        public string AxisName
        {
            get { return AxisLabel.Content.ToString(); }
            set { AxisLabel.Content = value; }
        }
        public int ID { get; set; }
        #endregion


        public AxisDiagnostic()
        {
            InitializeComponent();
        }


        public class AxisDiagnosticClickEventArgs : EventArgs
        {
            public Axis_HmiToPlc COMMAND { get; set; }
            public int ID { get; set; }
        }

        public event EventHandler<AxisDiagnosticClickEventArgs> ControlSetClicked;
        private void ControlSet_Click(object sender, RoutedEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (ControlSetClicked != null) ControlSetClicked(this, _AxisDiagnosticEvent);
        }

        #region Jog modes
        public event EventHandler<AxisDiagnosticClickEventArgs> JogFwFastClicked;
        private void JogFwFast_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogFwFastClicked != null) JogFwFastClicked(this, _AxisDiagnosticEvent);
        }
        private void JogFwFast_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogFwFastClicked != null) JogFwFastClicked(this, _AxisDiagnosticEvent);
        }

        public event EventHandler<AxisDiagnosticClickEventArgs> JogBwFastClicked;
        private void JogBwFast_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogBwFastClicked != null) JogBwFastClicked(this, _AxisDiagnosticEvent);
        }
        private void JogBwFast_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogBwFastClicked != null) JogBwFastClicked(this, _AxisDiagnosticEvent);
        }

        public event EventHandler<AxisDiagnosticClickEventArgs> JogFwSlowClicked;
        private void JogFwSlow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = true;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogFwSlowClicked != null) JogFwSlowClicked(this, _AxisDiagnosticEvent);
        }
        private void JogFwSlow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogFwSlowClicked != null) JogFwSlowClicked(this, _AxisDiagnosticEvent);
        }

        public event EventHandler<AxisDiagnosticClickEventArgs> JogBwSlowClicked;
        private void JogBwSlow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = true;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogBwSlowClicked != null) JogBwSlowClicked(this, _AxisDiagnosticEvent);
        }
        private void JogBwSlow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = false;
            #endregion

            if (JogBwSlowClicked != null) JogBwSlowClicked(this, _AxisDiagnosticEvent);
        }
        #endregion

        public event EventHandler<AxisDiagnosticClickEventArgs> ResetClicked;
        private void AxisReset_Click(object sender, RoutedEventArgs e)
        {
            AxisDiagnosticClickEventArgs _AxisDiagnosticEvent = new AxisDiagnosticClickEventArgs();
            _AxisDiagnosticEvent.COMMAND = new Axis_HmiToPlc();
            _AxisDiagnosticEvent.ID = this.ID;

            #region Servo Enable/Disable
            _AxisDiagnosticEvent.COMMAND.SERVO_OFF = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON = (bool)EnableControl.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_BW = (bool)EnableBw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.SERVO_ON_FW = (bool)EnableFw.IsChecked;
            _AxisDiagnosticEvent.COMMAND.CONTROLLER_OVERRIDE = AxisOverride.Value;
            #endregion
            #region Servo motion dynamics
            _AxisDiagnosticEvent.COMMAND.TARGET_ACCELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_DECELERATION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_JERK = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_POSITION = 0;
            _AxisDiagnosticEvent.COMMAND.TARGET_VELOCITY = 0;
            #endregion
            #region Jog mode
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_MODE = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_BW_SLOW = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_FAST = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_JOG_FW_SLOW = false;
            #endregion
            #region Servo movement commands
            _AxisDiagnosticEvent.COMMAND.SERVO_HOME = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_ABS = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_MOVE_REL = false;
            _AxisDiagnosticEvent.COMMAND.SERVO_RESET = true;
            #endregion

            if (ResetClicked != null) ResetClicked(this, _AxisDiagnosticEvent);
        }
    }
}
