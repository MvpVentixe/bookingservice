namespace BookingService.Dtos
{
    public class BookingRequestDto
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }

}
