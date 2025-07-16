namespace SEFIMAPI.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public AppDbContext()
        {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<DirectTransaction> DirectTransaction { get; set; }
        public DbSet<DeletedDirectTransaction> DeletedDirectTransaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).HasMaxLength(200);
                entity.Property(e => e.ProductGroup).HasMaxLength(100);
                entity.Property(e => e.ProductCode).HasMaxLength(50);
                entity.Property(e => e.ProductType).HasMaxLength(100);
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });
        }
    }
}
