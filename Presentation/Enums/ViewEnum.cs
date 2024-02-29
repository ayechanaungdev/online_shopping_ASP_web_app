using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Enums
{
    public class ViewEnum
    {
        public enum Action
        {
            [Description("Create")]
            Create = 1,
            [Description("Edit")]
            Edit = 2,
            [Description("Delete")]
            Delete = 3,
        }
    }
}
