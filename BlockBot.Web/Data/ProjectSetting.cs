using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlockBot.Web.Data
{
    public class ProjectSetting
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Value { get; set; }

        public Guid ProjectId { get; set; }
        [ScaffoldColumn(false)]
        public virtual Project Project { get; set; }

        public Guid ProjectSettingTypeId { get; set; }
        [ScaffoldColumn(false)]
        public virtual ProjectSettingType ProjectSettingType { get; set; }

    }
}
