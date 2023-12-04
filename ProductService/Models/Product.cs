using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductService.Models
{
    public class Product
    {
    

        [JsonPropertyName("id")]
       [Key] public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("availability")]
        public int Availability { get; set; }

        [JsonPropertyName("cost")]
        public int Cost { get; set; }
    }
}
