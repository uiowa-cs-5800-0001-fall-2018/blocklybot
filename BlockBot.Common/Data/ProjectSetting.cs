using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Common.Data
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
