using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BotBuilder.Web.Data
{
    public class ProjectStep
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectStepId { get; set; }

        [MaxLength]
        public string StepXML { get; set; }

        public int ProjectStepOrder { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
