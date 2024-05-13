using Microsoft.EntityFrameworkCore;

namespace Catedra1DWB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}
        
        public  DbSet<Book> Books => Set<Book>();
    }
}