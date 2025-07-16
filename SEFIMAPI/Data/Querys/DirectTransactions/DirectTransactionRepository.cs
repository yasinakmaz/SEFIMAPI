namespace SEFIMAPI.Data.Querys.DirectTransactions
{
    public class DirectTransactionRepository : IDirectTransactionRepository
    {
        private readonly ILogger<DirectTransactionRepository>? _logger;
        private readonly AppDbContext _context;

        public DirectTransactionRepository(ILogger<DirectTransactionRepository>? logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<DirectTransaction>> GetDirectTransactions(int pagesize, int pagenumber)
        {
            List<DirectTransaction> kasa = new List<DirectTransaction>();

            try
            {
                kasa = await _context.DirectTransaction.OrderBy(p => p.Date).Skip((pagenumber - 1) * pagesize).Take(pagesize).ToListAsync();
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
            return kasa;
        }

        public async Task<int> GetDirectTransactionPageNumber(int pagesize)
        {

            try
            {
                int totalCount = await _context.DirectTransaction.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pagesize);

                return totalPages;
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
            return 0;
        }

        public async Task<int> GetDeletedDirectTransactionPageNumber(int pagesize)
        {

            try
            {
                int totalCount = await _context.DeletedDirectTransaction.CountAsync();
                int totalPages = (int)Math.Ceiling(totalCount / (double)pagesize);

                return totalPages;
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
            return 0;
        }

        public async Task<List<DeletedDirectTransaction>> GetDeletedDirectTransactions(int pagesize, int pagenumber)
        {
            List<DeletedDirectTransaction> kasa = new List<DeletedDirectTransaction>();

            try
            {
                kasa = await _context.DeletedDirectTransaction.OrderBy(p => p.Date).Skip((pagenumber - 1) * pagesize).Take(pagesize).ToListAsync();
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
            return kasa;
        }

        public async Task<List<DirectTransaction>> AddDirectTransaction(DirectTransaction directTransaction)
        {
            try
            {
                var yeni = await _context.DirectTransaction.AddAsync(new DirectTransaction
                {
                    Date = DateTime.UtcNow,
                    Description = directTransaction.Description,
                    Total = directTransaction.Total,
                    UserName = directTransaction.UserName,
                    CustomerName = directTransaction.CustomerName,
                });

                int i = await _context.SaveChangesAsync();

                if(i > 0)
                {
                    _logger?.LogInformation("Başarılı");
                }
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
            return new List<DirectTransaction>();
        }

        public async Task<bool> DeleteDirectTransaction(int id)
        {
            try
            {
                var kasa = await _context.DirectTransaction.Where(p => p.Id == id).ToListAsync();
                var deletedEntities = kasa.Select(k => new DeletedDirectTransaction
                {
                    Date = DateTime.Now,
                    Description = k.Description,
                    Total = k.Total,
                    UserName = k.UserName,
                    CustomerName = k.CustomerName,
                }).ToList();

                await _context.DeletedDirectTransaction.AddRangeAsync(deletedEntities);
                await _context.SaveChangesAsync();

                int i = await _context.DirectTransaction.Where(p => p.Id == id).ExecuteDeleteAsync();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    _logger?.LogError("İşlem Başarısız - Silinecek kayıt bulunamadı");
                    return false;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
                throw;
            }
        }
        public async Task<bool> DeleteDeletedDirectTransaction(int id)
        {
            try
            {
                var kasa = await _context.DirectTransaction.Where(p => p.Id == id).ToListAsync();
                var deletedEntities = kasa.Select(k => new DirectTransaction
                {
                    Date = DateTime.Now,
                    Description = k.Description,
                    Total = k.Total,
                    UserName = k.UserName,
                    CustomerName = k.CustomerName,
                }).ToList();

                await _context.DirectTransaction.AddRangeAsync(deletedEntities);
                await _context.SaveChangesAsync();

                int i = await _context.DeletedDirectTransaction.Where(p => p.Id == id).ExecuteDeleteAsync();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    _logger?.LogError("İşlem Başarısız - Silinecek kayıt bulunamadı");
                    return false;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeletedTransactionTableCreate()
        {
            bool success = false;
            try
            {
                string? commandtext = ("CREATE TABLE [dbo].[DeletedDirectTransaction](\r\n\t[Id] [int] IDENTITY(1,1) NOT NULL,\r\n\t[Date] [datetime] NULL,\r\n\t[Description] [nvarchar](500) NULL,\r\n\t[Total] [decimal](10, 2) NULL,\r\n\t[UserName] [nvarchar](50) NULL,\r\n\t[CustomerName] [nvarchar](250) NULL,\r\n\t[Aktarildi] [bit] NULL,\r\n\t[IsSynced] [bit] NULL,\r\n\t[IsUpdated] [bit] NULL,\r\n CONSTRAINT [PK_DeletedDirectTransaction] PRIMARY KEY CLUSTERED \r\n(\r\n\t[Id] ASC\r\n)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n) ON [PRIMARY]");
                SqlConnection sqlConnection = new SqlConnection("Data Source=.;Initial Catalog=sefim;Persist Security Info=True;User ID=sa;Password=123456a.A;Encrypt=True;Trust Server Certificate=True");
                await sqlConnection.OpenAsync();
                SqlCommand cmd = sqlConnection.CreateCommand();
                cmd.CommandText = commandtext;
                int i = await cmd.ExecuteNonQueryAsync();
                if (i > 0)
                {
                    success = true;
                    _logger?.LogInformation("İşlem Tamamlandı");
                }
                sqlConnection.Close();
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
            return success;
        }
    }
}
