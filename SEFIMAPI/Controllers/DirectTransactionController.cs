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

        // Aktif işlemleri listeleme
        [HttpGet("active")]
        public async Task<ActionResult<List<DirectTransaction>>> GetDirectTransaction([FromQuery] int pagesize, [FromQuery] int pagenumber)
        {
            try
            {
                var products = await _directTransactionRepository.GetDirectTransactions(pagesize, pagenumber);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Silinmiş işlemleri listeleme
        [HttpGet("deleted")]
        public async Task<ActionResult<List<DirectTransaction>>> GetDeletedDirectTransactions([FromQuery] int pagesize, [FromQuery] int pagenumber)
        {
            try
            {
                var products = await _directTransactionRepository.GetDeletedDirectTransactions(pagesize, pagenumber);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Yeni işlem ekleme
        [HttpPost("add")]
        public async Task<IActionResult> AddTransaction([FromBody] DirectTransaction transaction)
        {
            if (transaction == null)
                return BadRequest("Transaction verisi boş olamaz.");

            try
            {
                var result = await _directTransactionRepository.AddDirectTransaction(transaction);
                return result != null ? Ok("İşlem Başarılı") : StatusCode(500, "Kayıt yapılamadı.");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Aktif işlem silme
        [HttpDelete("active/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var result = await _directTransactionRepository.DeleteDirectTransaction(id);
                return result ? Ok("İşlem Başarılı") : BadRequest("Kayıt bulunamadı.");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Silinmiş işlem silme
        [HttpDelete("deleted/{id}")]
        public async Task<IActionResult> DeleteDeletedTransaction(int id)
        {
            try
            {
                var result = await _directTransactionRepository.DeleteDeletedDirectTransaction(id);
                return result ? Ok("İşlem Başarılı") : BadRequest("Kayıt bulunamadı.");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Sayfa sayısı hesaplama (aktif işlemler)
        [HttpGet("active/pagecount")]
        public async Task<IActionResult> GetDirectTransactionPageNumber([FromQuery] int pagesize)
        {
            try
            {
                int result = await _directTransactionRepository.GetDirectTransactionPageNumber(pagesize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Sayfa sayısı hesaplama (silinmiş işlemler)
        [HttpGet("deleted/pagecount")]
        public async Task<IActionResult> GetDeletedDirectTransactionPageNumber([FromQuery] int pagesize)
        {
            try
            {
                int result = await _directTransactionRepository.GetDeletedDirectTransactionPageNumber(pagesize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Silinmiş işlemler tablosu oluşturma
        [HttpPost("deleted/init")]
        public async Task<IActionResult> DeletedTableCreate()
        {
            try
            {
                await _directTransactionRepository.DeletedTransactionTableCreate();
                return Ok("İşlem Tamamlandı");
            }
            catch (Exception ex)
            {
                _logger?.LogCritical($"Hata: {ex.Message}");
                return StatusCode(500, "Sunucu hatası");
            }
        }
    }
}
