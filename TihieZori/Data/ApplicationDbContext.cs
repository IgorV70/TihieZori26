using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace TihieZori.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        private readonly DbContextOptions<ApplicationDbContext> _options = options;
        //protected readonly IConfiguration _configuration;
        //public ApplicationDbContext(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (_configuration != null)
            //{
            //    object value = optionsBuilder.UseNpgsql(_configuration.GetConnectionString("db")
            //    , x => x.MigrationsHistoryTable("__EgrulMigrationsHistory", "plain"));
            //    //optionsBuilder.LogTo(s => Debug.Print(s));

            //}
            //else
            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tihiezori;Username=postgres;Password=postgres;");
            if (this._options != null)
            { 
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
