namespace SEFIMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProductAsync();
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            try
            {
                var products = await _productRepository.GetProductByIdAsync(id);
                var product = products.FirstOrDefault();

                if (product == null)
                {
                    return NotFound($"ID: {id} olan ürün bulunamadı");
                }

                return Ok(product);
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

        [HttpGet("groups")]
        public async Task<ActionResult<List<Product>>> GetAllGroups()
        {
            try
            {
                var groups = await _productRepository.GetAllGroupsAsync();
                return Ok(groups);
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

        [HttpGet("group/{groupName}")]
        public async Task<ActionResult<List<Product>>> GetProductsByGroup(string groupName)
        {
            try
            {
                var products = await _productRepository.GetGroupsByProductAsync(groupName);

                if (!products.Any())
                {
                    return NotFound($"'{groupName}' grubunda ürün bulunamadı");
                }

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

        [HttpGet("{pagesize},{pagenumber},{group}")]
        public async Task<ActionResult<List<Product>>> GetProductsPageByGroups(int pagesize, int pagenumber, string group)
        {
            try
            {
                var result = await _productRepository.GetPageProductAsync(pagesize, pagenumber, group);

                if (result == null || !result.Any())
                {
                    return NotFound($"{group} Grubunda Ürün Bulunamadı");
                }

                return Ok(result);
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
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Ürün verisi boş olamaz.");
            }

            try
            {
                var result = await _productRepository.AddProductAsync(product);

                if (result == null)
                {
                    return NotFound("İşlem Başarısız");
                }

                return Ok(result);
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