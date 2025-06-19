//using BookingService.Dtos;
//using BookingService.Entities;
//using BookingService.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace BookingService.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BookingController(BookingServiceHandler bookingservice) : ControllerBase
//    {
//        private readonly BookingServiceHandler _bookingService = bookingservice;

//        [HttpPost("bookingCreation")]
//        public async Task<ActionResult<BookingEntity>> CreateBookingAsync([FromBody] BookingRequestDto request)
//        {

//            try
//            {
//                var booking = await _bookingService.CreateBookingAsync(request.UserId, request.EventId);
//                return NoContent();

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("❌ Exception: " + ex.Message);
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpGet("bookingsByUserId/{userId}")]
//        public async Task<ActionResult<IEnumerable<BookingEntity>>> GetBookingsByUserIdAsync(Guid userId)
//        {
//            try
//            {
//                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
//                return Ok(bookings);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//    }
//}

using BookingService.Dtos;
using BookingService.Entities;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController(BookingServiceHandler bookingservice) : ControllerBase
    {
        private readonly BookingServiceHandler _bookingService = bookingservice;

        [HttpPost("bookingCreation")]
        public async Task<ActionResult<BookingEntity>> CreateBookingAsync([FromBody] BookingRequestDto request)
        {
            Console.WriteLine($"📨 Received UserId: {request.UserId}");
            Console.WriteLine($"📨 Received EventId: {request.EventId}");


            if (request is null || request.UserId == Guid.Empty || request.EventId == Guid.Empty)
                return BadRequest("Invalid booking request payload.");

            try
            {
                Console.WriteLine($"📨 Creating booking: UserId={request.UserId}, EventId={request.EventId}");

                var booking = await _bookingService.CreateBookingAsync(request.UserId, request.EventId);

                return Ok(booking);

            }
            catch (InvalidOperationException ex)
            {
                // for business logic errors like duplicate booking
                Console.WriteLine("⚠️ Business error: " + ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("❌ HTTP call failed: " + ex.Message);
                return StatusCode(502, new { message = "Failed to fetch user or event info", detail = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Unexpected error: " + ex.Message);
                return StatusCode(500, new { message = "Unexpected error occurred", detail = ex.Message });
            }
        }

        [HttpGet("bookingsByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingEntity>>> GetBookingsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid userId.");

            try
            {
                var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to fetch bookings: " + ex.Message);
                return StatusCode(500, new { message = "Failed to get bookings", detail = ex.Message });
            }
        }

        [HttpDelete("deleteBooking/{bookingId}")]

        public async Task<ActionResult> DeleteBookingAsync(Guid bookingId) 
        {
            var result = await _bookingService.DeleteBookingAsync(bookingId);

            if (!result)
                return NotFound($"No booking found with ID: {bookingId}");

            return NoContent();
        }
    }
}
