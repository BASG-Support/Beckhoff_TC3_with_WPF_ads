using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASG.TwinCATConnector
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
        public bool SERVO_RESET { get; set; }
        public bool SERVO_JOG_MODE { get; set; }
        public bool SERVO_JOG_FW_FAST { get; set; }
        public bool SERVO_JOG_BW_FAST { get; set; }
        public bool SERVO_JOG_FW_SLOW { get; set; }
        public bool SERVO_JOG_BW_SLOW { get; set; }
    }
}
