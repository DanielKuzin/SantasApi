using System.ComponentModel.DataAnnotations;
namespace SantaApi.Models
{
    public class GiftRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        public string Address { get; set; }
        public List<Gift> GiftsWanted { get; set; }
    }
}
