using System.ComponentModel.DataAnnotations;

namespace BookingService.Entities
{
    public class BookingEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        public DateTime EventDateTime { get; set; }

        [Required]
        public string QrCode { get; set; } = null!;
    }
}
