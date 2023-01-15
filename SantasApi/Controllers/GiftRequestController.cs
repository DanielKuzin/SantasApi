using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SantaApi.Data;
using SantaApi.Models;
using Xunit;

namespace SantaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftRequestController : ControllerBase
    {
        private static List<string> giftNames = new List<string> { "PSP", "Rocket", "RC Car", "Lego", "Barbie", "Cryon’s", "Candies", "Mittens" };
        private static List<int> giftPrices = new List<int> { 50, 45, 25, 15, 10, 10, 5, 3 };

        public SantaDbContext _context;

        public GiftRequestController(SantaDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> AddGiftRequest([FromBody] GiftRequest giftRequest)
        {
            // Validate the incoming request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the gift request already exists to enable for kids to
            // update the address or color of a gift that was already requested
            var existingGiftRequest = await _context.GiftRequests.FirstOrDefaultAsync(
                gr => gr.Name == giftRequest.Name && gr.Age == giftRequest.Age);
            if (existingGiftRequest != null)
            {
                // Update the existing gift request
                existingGiftRequest.Address = giftRequest.Address;
                existingGiftRequest.GiftsWanted = giftRequest.GiftsWanted;
                _context.GiftRequests.Update(existingGiftRequest);
            }
            else
            {
                // Add the gift request to the database
                _context.GiftRequests.Add(giftRequest);
            }

            await _context.SaveChangesAsync();

            // Return a 201 Created status code and the created gift
            return Created("", giftRequest);
        }


        [HttpGet]
        public async Task<IActionResult> GetGiftRequests()
        {
            var giftRequests = await _context.GiftRequests.Include(g => g.GiftsWanted).ToListAsync();
            var giftList = giftRequests.Select(gr => new GiftList()
            {
                Address = gr.Address,
                // Take the most expensive gifts for each child while not exceeding 50 coins per child
                Gifts = gr.GiftsWanted.OrderByDescending(gift => getGiftCost(gift))
                                    .Select((gift, gIndex) => new { Gift = gift, Index = gIndex })
                                    .Where(g_i => gr.GiftsWanted.Take(g_i.Index + 1).Sum(g => g.Cost) <= 50)
                                    .Select(gift => gift.Gift.Name).ToList()
            });
            return Ok(giftList);
        }

        private int getGiftCost(Gift gift)
        {
            int index = giftNames.FindIndex(name => name == gift.Name);
            return index == -1 ? 0 : giftPrices[index];
        }

        //Tests, placed here for lack of time

        [Fact]
        public async Task TestAddGiftRequestValidRequest()
        {
            // Create mock request
            var request = new GiftRequest
            {

                Name = "Andrew",
                Age = 5,
                Address = "123, wotch drive",
                GiftsWanted = new List<Gift>
                {
                    new Gift { Name = "Rocket", Color = "Red" },
                    new Gift { Name = "RC Car", Color = "Black" }
                }
            };
            var controller = new GiftRequestController(_context);

            // AddGift
            var result = await controller.AddGiftRequest(request);

            // Check return val
            Assert.IsType<CreatedResult>(result);
        }
    }
}
