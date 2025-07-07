namespace SEFIMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<User>>> GetByIdUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
                return StatusCode(500, "DbUpdate Hatası");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
                return StatusCode(500, "Sql Hatası");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
                return StatusCode(500, "Genel Hatası");
            }
        }

        [HttpGet("password")]
        public async Task<ActionResult<bool>> UserByPasswordLogin(string password)
        {
            try
            {
                var user = await _userRepository.UserByPasswordLoginAsync(password);
                return Ok(user);
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
                return StatusCode(500, "DbUpdate Hatası");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Sql Hatası : {ex.Message}");
                return StatusCode(500, "Sql Hatası");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Genel Hata : {ex.Message}");
                return StatusCode(500, "Genel Hatası");
            }
        }
    }
}
