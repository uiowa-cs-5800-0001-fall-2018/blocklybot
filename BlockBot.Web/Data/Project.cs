using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Web.Data
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public virtual List<ProjectStep> ProjectSteps { get; set; }
    }
}
