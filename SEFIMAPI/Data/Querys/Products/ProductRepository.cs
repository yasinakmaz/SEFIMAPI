namespace SEFIMAPI.Data.Querys.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly ILogger<ProductRepository>? _logger;
        private readonly AppDbContext _context;

        public ProductRepository(ILogger<ProductRepository>? logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            List<Product> products = new List<Product>();

            try
            {
                products = await _context.Product.AsNoTracking().OrderBy(p => p.ProductName).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
            }
            return products;
        }

        public async Task<List<string?>> GetAllGroupsAsync()
        {
            List<string?> group = new List<string?>();

            try
            {
                group = await _context.Product
                    .AsNoTracking()
                    .Select(p => p.ProductGroup)
                    .Distinct()
                    .ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical(ex, "SQL Hatası");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Genel Hata");
            }

            return group;
        }

        public async Task<List<Product>> GetProductByIdAsync(int IND)
        {
            List<Product> product = new List<Product>();

            try
            {
                product = await _context.Product.AsNoTracking().Where(p => p.Id == IND).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
            }
            return product;
        }

        public async Task<List<Product>> GetGroupsByProductAsync(string group)
        {
            List<Product> product = new List<Product>();

            try
            {
                product = await _context.Product.AsNoTracking().Where(p => p.ProductGroup == group).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
            }
            return product;
        }

        public async Task<List<Product>> GetPageProductAsync(int pagesize, int pagenumber, string group)
        {
            List<Product> product = new List<Product>();

            try
            {
                product = await _context.Product
                    .AsNoTracking()
                    .Where(p => p.ProductGroup == group)
                    .OrderBy(p => p.ProductName)
                    .Skip((pagenumber -1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
            }
            return product;
        }

        public async Task<List<Product>> AddProductAsync(Product product)
        {
            List<Product> newproduct = new List<Product>();

            try
            {
                var newadd = (new Product
                {
                    ProductName = product.ProductName,
                    ProductGroup = product.ProductGroup,
                    ProductCode = product.ProductCode,
                    Order = product.Order,
                    VatRate = product.VatRate,
                    Price = product.Price,
                    ProductType = product.ProductType
                });

                await _context.Product.AddAsync(newadd);
                int i = await _context.SaveChangesAsync();
                if (i > 0)
                {
                    _logger?.LogInformation($"İşlem Tamamlandı {newadd.Id}");
                }
                else
                    _logger?.LogError("İşlem Başarısız");
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
            }
            return newproduct;
        }
    }
}
