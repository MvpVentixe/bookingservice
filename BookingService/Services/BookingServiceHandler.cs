using BookingService.Data;
using BookingService.Dtos;
using BookingService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace BookingService.Services
{
    public class BookingServiceHandler
    {
        public BookingServiceHandler( DataContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _dbSet = _context.Set<BookingEntity>();
            _httpClientFactory = httpClientFactory;

        }

        private readonly DataContext _context;
        private readonly DbSet<BookingEntity> _dbSet;
        private readonly IHttpClientFactory _httpClientFactory;

        public async Task<IEnumerable<BookingEntity>> GetBookingsByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<BookingEntity> CreateBookingAsync(Guid userId, Guid eventId)
        {

            var alreadyBooked = await _dbSet.AnyAsync(b => b.UserId == userId && b.EventId == eventId);

            if (alreadyBooked)
                throw new InvalidOperationException("You have already booked this event.");

            var client = _httpClientFactory.CreateClient("AuthService");
            var userInfo = await client.GetFromJsonAsync<AuthDto>($"api/auth/booking/{userId}");

            if (userInfo == null)
                throw new Exception("User not found");

            var eventClient = _httpClientFactory.CreateClient("EventService");
            var eventInfo = await eventClient.GetFromJsonAsync<EventDto>($"api/event/{eventId}");

            if (eventInfo == null)
                throw new Exception("Event not found");

            var combinedData = $"{userInfo.FirstName} {userInfo.LastName} - {eventInfo.Title} - {eventInfo.EventDateTime}";
            var qrCode = GenerateQrCodeString(combinedData);

            var booking = new BookingEntity
            {
                UserId = userId,
                EventId = eventId,
                FullName = $"{userInfo.FirstName} {userInfo.LastName}",
                Title = eventInfo.Title,
                Location = eventInfo.Location,
                EventDateTime = eventInfo.EventDateTime,
                QrCode = qrCode
            };

            _context.Add(booking);
            await _context.SaveChangesAsync();

            return booking;

        }

        private string GenerateQrCodeString(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return Convert.ToBase64String(qrCode.GetGraphic(20));
        }


        public async Task<bool> DeleteBookingAsync(Guid bookingId)
        {
            var booking = await _dbSet.FindAsync(bookingId);

            if (booking == null)
                return false;

            _dbSet.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
