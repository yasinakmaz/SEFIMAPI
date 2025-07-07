namespace SEFIMAPI.Data.Querys.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository>? _logger;
        private readonly AppDbContext _context;
        public UserRepository(ILogger<UserRepository>? logger, AppDbContext context) 
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<User>> GetUserByIdAsync(int ID)
        {
            List<User> user = new List<User>();
            try
            {
                user = await _context.User.AsNoTracking().Where(p => p.Id == ID).ToListAsync();
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
            return user;
        }

        public async Task<bool> UserByPasswordLoginAsync(string password)
        {
            bool user = false;
            try
            {
                int count = await _context.User.AsNoTracking().Where(p => p.Password == password).CountAsync();

                if (count == 1)
                {
                    return true;
                }
                else
                {
                    _logger?.LogWarning("Şifre birden fazla kullanıcıda bulundu veya hiç bulunamadı.");
                }
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata: {ex.Message}");
            }
            return user;
        }
    }
}
