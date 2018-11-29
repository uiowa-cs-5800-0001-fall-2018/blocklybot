using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Web.Data
{
    public class Integration
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ScaffoldColumn(false)]
        public Guid ProjectId { get; set; }

        public Guid ServiceId { get; set; }


        [ScaffoldColumn(false)]
        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        [ScaffoldColumn(false)]
        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; }

        /// <summary>
        ///     The URL of the integration lambda function
        /// </summary>
        [ScaffoldColumn(false)]
        public string Url { get; set; }

        public string FunctionName()
        {
            return ProjectId + "-" + Service.Name.ToLowerInvariant();
        }
    }
}