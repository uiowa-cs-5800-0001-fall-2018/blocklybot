using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Common.Data
{
    public class Project
    {
        [ScaffoldColumn(false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [ScaffoldColumn(false)]
        [Required]
        public Guid OwnerId { get; set; }

        [ScaffoldColumn(false)]
        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser Owner { get; set; }

        [MinLength(1)]
        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        [MaxLength]
        public string XML { get; set; }


        [ScaffoldColumn(false)]
        public string RestApiId { get; set; }

        [Required]
        [Display(Name = "Is Public")]
        public bool IsPublic { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Description { get; set; }

        public string S3BucketName()
        {
            return "blockbot-" + Id;
        }
    }
}