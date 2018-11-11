using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Web.Data
{
    public class Project
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        [MaxLength] public string XML { get; set; }

        [ForeignKey(nameof(OwnerId))] public virtual ApplicationUser Owner { get; set; }
    }
}