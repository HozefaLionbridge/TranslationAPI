using Microsoft.EntityFrameworkCore;


public class TranslationContext : DbContext
{
    public TranslationContext(DbContextOptions<TranslationContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
}

