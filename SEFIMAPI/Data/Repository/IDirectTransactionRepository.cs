namespace SEFIMAPI.Data.Repository
{
    public interface IDirectTransactionRepository
    {
        Task<List<DirectTransaction>> GetDirectTransactions(int pagesize, int pagenumber);
        Task<List<DirectTransaction>> AddDirectTransaction(DirectTransaction directTransaction);
        Task<bool> DeleteDirectTransaction(int id);
        Task<bool> DeletedTransactionTableCreate();
    }
}
