using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_Zadanie_7
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly YourDbContext _context;

        public ClientsController(YourDbContext context)
        {
            _context = context;
        }
        
        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = await _context.Clients.FindAsync(idClient);

            if (client == null)
            {
                return NotFound();
            }

            var trips = _context.ClientTrips.Where(ct => ct.ClientId == idClient);

            if (trips.Any())
            {
                return BadRequest("Client has assigned trips.");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
