namespace SEFIMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectTransactionController : ControllerBase
    {
        private readonly IDirectTransactionRepository _directTransactionRepository;
        private readonly ILogger<DirectTransactionController> _logger;

        public DirectTransactionController(IDirectTransactionRepository directRepository, ILogger<DirectTransactionController> logger)
        {
            _directTransactionRepository = directRepository;
            _logger = logger;
        }

        [HttpGet("{pagesize},{pagenumber}")]
        public async Task<ActionResult<List<DirectTransaction>>> GetDirectTransaction(int pagesize, int pagenumber)
        {
            try
            {
                var products = await _directTransactionRepository.GetDirectTransactions(pagesize, pagenumber);
                return Ok(products);
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
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddTransaction([FromBody] DirectTransaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction verisi boş olamaz.");
            }

            try
            {
                var result = await _directTransactionRepository.AddDirectTransaction(transaction);

                if (result != null)
                {
                    return Ok("İşlem Başarılı");
                }
                else
                    return StatusCode(500, "Kayıt yapılamadı.");
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var result = await _directTransactionRepository.DeleteDirectTransaction(id);
                if (result)
                    return Ok("İşlem Başarılı");
                else
                    return BadRequest("Silme işlemi başarısız oldu - Kayıt bulunamadı.");
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogCritical($"DbUpdate Hatası :{ex.InnerException?.Message}");
                return StatusCode(500, $"DbUpdate Hatası: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (SqlException ex)
            {
                _logger?.LogCritical($"Controller Sql Hatası : {ex.Message}");
                return BadRequest($"Sql Hatası: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Controller Genel Hata : {ex.Message}");
                return StatusCode(500, $"Genel Hata: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletedTableCreate()
        {
            try
            {
               var result = await _directTransactionRepository.DeletedTransactionTableCreate();
                return Ok("İşlem Tamamlandı");
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
