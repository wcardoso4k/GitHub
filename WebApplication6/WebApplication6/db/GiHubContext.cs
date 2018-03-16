using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebApplication6.Models;

namespace WebApplication6.db
{
    public class GiHubContext : DbContext
    {
        public GiHubContext()
            : base("dbAzure")
        {
        }

        public DbSet<RepositoryGitHub> Repositories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}