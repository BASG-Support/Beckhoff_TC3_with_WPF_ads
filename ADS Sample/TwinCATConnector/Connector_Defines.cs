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
}
