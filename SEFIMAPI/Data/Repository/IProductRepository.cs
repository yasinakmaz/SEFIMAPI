namespace SEFIMAPI.Data.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductAsync();
        Task<List<string?>> GetAllGroupsAsync();
        Task<List<Product>> GetProductByIdAsync(int ind);
        Task<List<Product>> GetGroupsByProductAsync(string group);
        Task<List<Product>> GetPageProductAsync(int pagesize, int pagenumber, string group);
        Task<List<Product>> AddProductAsync(Product product);
        Task<int> GetProductPageNumber(int pagesize);
    }
}
