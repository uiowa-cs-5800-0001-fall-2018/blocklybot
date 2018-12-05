using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;

namespace BlockBot.Web.Models
{
    public class User_Projects_Model
    {
        public ApplicationUser User { get; set; }
        public List<Project> Projects { get; set; }
    }
}
