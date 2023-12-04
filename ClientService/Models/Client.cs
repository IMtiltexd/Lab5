using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClientService.Models
{
    public class Client
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastname")]
        public string LastName { get; set; }
        [JsonPropertyName("patronymic")]
        public string patronymic { get; set; }
        [JsonPropertyName("Email")]
        public string Email { get; set; }
        [JsonPropertyName("phonenumber")]
        public string PhoneNumber { get; set; }
    }
}
