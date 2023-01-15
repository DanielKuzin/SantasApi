using System.ComponentModel.DataAnnotations;
namespace SantaApi.Models
{
    public class Gift
    {
        [Key]
        public string Name { get; set; }
        public string Color { get; set; }
        public int Cost { get; set; }
    }
}
