﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using Google.Apis.Calendar.v3.Data;

namespace BlockBot.Web.Models
{
    public class WorkspaceModel
    {
        public Project Project { get; set; }
        public IList<CalendarListEntry> CalendarList { get; set; }
    }
}
