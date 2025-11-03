using Mobile_App_Develop.Models;

namespace Mobile_App_Develop.Services
{
    public class AuthService : IAuthService
    {
        private User? _currentUser;
        private readonly List<User> _users;
        
        public event EventHandler<UserChangedEventArgs>? UserChanged;

        public AuthService()
        {
            // 初始化一些测试用户
            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Email = "student@uts.edu.my",
                    Password = "password123",
                    FirstName = "John",
                    LastName = "Doe",
                    StudentId = "12345678",
                    AvatarUrl = "https://via.placeholder.com/150",
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new User
                {
                    Id = 2,
                    Email = "jane.smith@uts.edu.my",
                    Password = "password456",
                    FirstName = "Jane",
                    LastName = "Smith",
                    StudentId = "87654321",
                    AvatarUrl = "https://via.placeholder.com/150",
                    CreatedAt = DateTime.Now.AddDays(-15)
                }
            };
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            await Task.Delay(1000); // 模拟网络延迟
            
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
            
            if (user != null && user.IsActive)
            {
                _currentUser = user;
                _currentUser.LastLoginAt = DateTime.Now;
                UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
                
                // 保存登录状态到本地存储
                await SecureStorage.SetAsync("user_id", user.Id.ToString());
                await SecureStorage.SetAsync("user_email", user.Email);
                
                return true;
            }
            
            return false;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            await Task.Delay(1500); // 模拟网络延迟
            
            // 检查邮箱是否已存在
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            
            // 生成新的用户ID
            user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;
            
            _users.Add(user);
            
            // 自动登录新注册的用户
            _currentUser = user;
            UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
            
            await SecureStorage.SetAsync("user_id", user.Id.ToString());
            await SecureStorage.SetAsync("user_email", user.Email);
            
            return true;
        }

        public async Task<bool> LogoutAsync()
        {
            await Task.Delay(500); // 模拟网络延迟
            
            _currentUser = null;
            UserChanged?.Invoke(this, new UserChangedEventArgs(null));
            
            // 清除本地存储的登录信息
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_email");
            
            return true;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }
            
            // 尝试从本地存储恢复登录状态
            var userIdStr = await SecureStorage.GetAsync("user_id");
            if (int.TryParse(userIdStr, out int userId))
            {
                _currentUser = _users.FirstOrDefault(u => u.Id == userId);
                return _currentUser;
            }
            
            return null;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var user = await GetCurrentUserAsync();
            return user != null;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            await Task.Delay(1000); // 模拟网络延迟
            
            var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.StudentId = user.StudentId;
                existingUser.AvatarUrl = user.AvatarUrl;
                
                if (_currentUser?.Id == user.Id)
                {
                    _currentUser = existingUser;
                    UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
                }
                
                return true;
            }
            
            return false;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            await Task.Delay(1000); // 模拟网络延迟
            
            if (_currentUser != null && _currentUser.Password == currentPassword)
            {
                var user = _users.FirstOrDefault(u => u.Id == _currentUser.Id);
                if (user != null)
                {
                    user.Password = newPassword;
                    _currentUser.Password = newPassword;
                    return true;
                }
            }
            
            return false;
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            await Task.Delay(2000); // 模拟网络延迟
            
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return user != null; // 在真实应用中，这里会发送重置密码邮件
        }
    }

    // 事件参数类
    public class UserChangedEventArgs : EventArgs
    {
        public User? User { get; }

        public UserChangedEventArgs(User? user)
        {
            User = user;
        }
    }
}