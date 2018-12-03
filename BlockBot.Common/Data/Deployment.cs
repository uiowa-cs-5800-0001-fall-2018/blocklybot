using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockBot.Common.Data
{
    public class Deployment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid IntegrationId { get; set; }

        [ForeignKey(nameof(IntegrationId))] public virtual Integration Integration { get; set; }
    }
}