using Mobile_App_Develop.Models;

namespace Mobile_App_Develop.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(User user);
        Task<bool> LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        Task<bool> IsLoggedInAsync();
        Task<bool> UpdateUserAsync(User user);
        Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email);
        event EventHandler<UserChangedEventArgs> UserChanged;
    }
}