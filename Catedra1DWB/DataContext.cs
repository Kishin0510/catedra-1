using Microsoft.EntityFrameworkCore;

namespace Catedra1DWB;

/// <summary>
/// Represents the data context for the application.
/// </summary>
public class DataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the DataContext class.
    /// </summary>
    /// <param name="options"></param>
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

    /// <summary>
    /// Represents the eBooks table in the database.
    /// </summary>
    public DbSet<Book> Books { get; set; }
}