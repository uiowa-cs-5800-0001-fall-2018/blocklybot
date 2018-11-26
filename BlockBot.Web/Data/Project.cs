using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Web.Data
{
    public class Project
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ScaffoldColumn(false)]
        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        [ScaffoldColumn(false)]
        [MaxLength]
        public string XML { get; set; }

        [ScaffoldColumn(false)]
        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser Owner { get; set; }

        [ScaffoldColumn(false)]
        public string RestApiId { get; set; }

        public bool IsPublic { get; set; }
    }
}