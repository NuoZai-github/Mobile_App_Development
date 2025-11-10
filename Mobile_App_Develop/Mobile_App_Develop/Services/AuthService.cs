using UserModel = Mobile_App_Develop.Models.User;
using Supabase;

namespace Mobile_App_Develop.Services
{
    public class AuthService : IAuthService
    {
        private UserModel? _currentUser;
        private readonly Client _client;
        private Task? _initTask;
        
        public event EventHandler<UserChangedEventArgs>? UserChanged;

        public AuthService(Client client)
        {
            _client = client;
        }

        private async Task EnsureInitialized()
        {
            _initTask ??= _client.InitializeAsync();
            await _initTask;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            await EnsureInitialized();

            try
            {
                var session = await _client.Auth.SignIn(email, password);
                if (session != null && session.User != null)
                {
                    var u = session.User;
                    _currentUser = new UserModel
                    {
                        // Supabase 的 User.Id 为字符串 UUID；此处不映射到 int。
                        Id = 0,
                        Email = u.Email ?? email,
                        FirstName = string.Empty,
                        LastName = string.Empty,
                        StudentId = string.Empty,
                        AvatarUrl = string.Empty,
                        LastLoginAt = DateTime.Now,
                        IsActive = true
                    };

                    UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
                    return true;
                }
            }
            catch
            {
                // 忽略并返回失败，交由 UI 提示
            }

            return false;
        }

        public async Task<bool> RegisterAsync(Mobile_App_Develop.Models.User user)
        {
            await EnsureInitialized();

            try
            {
                var options = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object?>
                    {
                        ["first_name"] = user.FirstName,
                        ["last_name"] = user.LastName,
                        ["student_id"] = user.StudentId,
                        ["avatar_url"] = user.AvatarUrl
                    }
                };

                var session = await _client.Auth.SignUp(user.Email, user.Password, options);

                // 如果项目开启了邮箱确认，SignUp 后可能没有会话，此处尝试直接登录。
                if (session == null || session.User == null)
                {
                    try
                    {
                        session = await _client.Auth.SignIn(user.Email, user.Password);
                    }
                    catch { /* 尝试登录失败则继续按未登录处理 */ }
                }

                if (session != null && session.User != null)
                {
                    var u = session.User;
                    try
                    {
                        var profiles = _client.From<Mobile_App_Develop.Models.Profile>();
                        var profile = new Mobile_App_Develop.Models.Profile
                        {
                            Id = Guid.Parse(u.Id),
                            Email = u.Email ?? user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            StudentId = user.StudentId,
                            AvatarUrl = user.AvatarUrl,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = true
                        };
                        await profiles.Upsert(profile);
                    }
                    catch { }

                    _currentUser = new UserModel
                    {
                        Id = 0,
                        Email = u.Email ?? user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        StudentId = user.StudentId,
                        AvatarUrl = user.AvatarUrl,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
                    return true;
                }
            }
            catch (Exception)
            {
                // 注册失败（如邮箱已存在或策略限制）
            }

            return false;
        }

        public async Task<bool> LogoutAsync()
        {
            await EnsureInitialized();
            try
            {
                await _client.Auth.SignOut();
            }
            catch { /* 忽略 */ }

            _currentUser = null;
            UserChanged?.Invoke(this, new UserChangedEventArgs(null));
            return true;
        }

        public async Task<Mobile_App_Develop.Models.User?> GetCurrentUserAsync()
        {
            if (_currentUser != null)
                return _currentUser;

            await EnsureInitialized();
            var u = _client.Auth.CurrentUser;
            if (u != null)
            {
                _currentUser = new UserModel
                {
                    Id = 0,
                    Email = u.Email ?? string.Empty,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    StudentId = string.Empty,
                    AvatarUrl = string.Empty,
                    IsActive = true
                };
                return _currentUser;
            }

            return null;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            await EnsureInitialized();
            return _client.Auth.CurrentUser != null;
        }

        public async Task<bool> UpdateUserAsync(Mobile_App_Develop.Models.User user)
        {
            await EnsureInitialized();
            try
            {
                if (_client.Auth.CurrentUser != null)
                {
                    try
                    {
                        var u = _client.Auth.CurrentUser;
                        var profiles = _client.From<Mobile_App_Develop.Models.Profile>();
                        var profile = new Mobile_App_Develop.Models.Profile
                        {
                            Id = Guid.Parse(u!.Id),
                            Email = u.Email ?? user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            StudentId = user.StudentId,
                            AvatarUrl = user.AvatarUrl,
                            IsActive = user.IsActive
                        };
                        await profiles.Upsert(profile);
                    }
                    catch { }

                    _currentUser = user;
                    UserChanged?.Invoke(this, new UserChangedEventArgs(_currentUser));
                    return true;
                }
            }
            catch { }
            return false;
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            await EnsureInitialized();
            try
            {
                if (_client.Auth.CurrentUser != null)
                {
                    var attrs = new Supabase.Gotrue.UserAttributes { Password = newPassword };
                    await _client.Auth.Update(attrs);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            await EnsureInitialized();
            try
            {
                await _client.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    // 事件参数类
    public class UserChangedEventArgs : EventArgs
    {
        public Mobile_App_Develop.Models.User? User { get; }

        public UserChangedEventArgs(Mobile_App_Develop.Models.User? user)
        {
            User = user;
        }
    }
}