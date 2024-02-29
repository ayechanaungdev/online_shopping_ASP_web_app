using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Enums
{
    public static class StatusEnum
    {
        public enum NoticeStatus
        {
            [Description("Save Successfully")]
            Success = 1,
            [Description("Update Successfully")]
            Edit = 2,
            [Description("Delete Successfully")]
            Delete = 3,
            [Description("Delivered Successfully")]
            Deliver = 4,
            Fail = 6,
            [Description("Changed Successfully")]
            Change = 5,
            [Description("Changed Successfully")]
            EditProfile = 7,

        }
    }
}
