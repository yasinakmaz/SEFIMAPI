namespace SEFIMAPI.Data.Repository
{
    public interface IDirectTransactionRepository
    {
        Task<int> GetDirectTransactionPageNumber(int pagesize);
        Task<int> GetDeletedDirectTransactionPageNumber(int pagesize);
        Task<List<DirectTransaction>> GetDirectTransactions(int pagesize, int pagenumber);
        Task<List<DirectTransaction>> AddDirectTransaction(DirectTransaction directTransaction);
        Task<List<DeletedDirectTransaction>> GetDeletedDirectTransactions(int pagesize, int pagenumber);
        Task<bool> DeleteDirectTransaction(int id);
        Task<bool> DeletedTransactionTableCreate();
        Task<bool> DeleteDeletedDirectTransaction(int id);
    }
}
