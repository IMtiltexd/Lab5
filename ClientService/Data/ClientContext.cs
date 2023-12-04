using Microsoft.EntityFrameworkCore;
using ClientService.Models;
namespace ClientService.Data
{
    public class ClientContext : DbContext
    {
        public ClientContext(DbContextOptions<ClientContext> options) : base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
    }
}
