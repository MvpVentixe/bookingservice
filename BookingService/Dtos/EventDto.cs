using System.ComponentModel.DataAnnotations;

namespace BookingService.Dtos
{
    public class EventDto
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Location { get; set; } = null!;

        [Required]
        public DateTime EventDateTime { get; set; }
    }
}
