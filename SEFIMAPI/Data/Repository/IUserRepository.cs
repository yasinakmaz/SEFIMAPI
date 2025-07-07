namespace SEFIMAPI.Data.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> GetUserByIdAsync(int id);
        Task<bool> UserByPasswordLoginAsync(string parola);
        /*Task<List<User>> UserLoginAsync();
        Task<List<User>> UserLogoutAsync();
        Task<List<User>> UserRegisterAsync();
        Task<List<User>> UserUpdatePasswordAsync();*/
    }
}