using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Web.Data
{
    public class Project
    {
        // TODO convert id to Guid
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        [MaxLength]
        public string XML { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser Owner { get; set; }

        public string RestApiId { get; set; }
    }
}