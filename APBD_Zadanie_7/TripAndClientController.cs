using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.DTOs;

namespace APBD_Zadanie_7
{
    [Route("api/trips/{idTrip}/clients")]
    [ApiController]
    public class TripClientsController : ControllerBase
    {
        private readonly YourDbContext _context;

        public TripClientsController(YourDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] AssignClientDto dto)
        {
            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found.");
            }

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.PESEL == dto.PESEL);
            if (client == null)
            {
                client = new Client { PESEL = dto.PESEL, Name = dto.Name };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            var existingAssignment = await _context.ClientTrips
                .FirstOrDefaultAsync(ct => ct.TripId == idTrip && ct.ClientId == client.ClientId);
            if (existingAssignment != null)
            {
                return BadRequest("Client is already assigned to this trip.");
            }

            var clientTrip = new ClientTrip
            {
                TripId = idTrip,
                ClientId = client.ClientId,
                RegisteredAt = DateTime.UtcNow,
                PaymentDate = dto.PaymentDate
            };

            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
