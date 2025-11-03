# UTS Bus Tracker - Mobile App

一个为UTS学生设计的校园巴士实时追踪移动应用，使用.NET MAUI框架开发。

## 项目概述

UTS Bus Tracker是一个跨平台移动应用，帮助UTS学生实时追踪校园巴士位置、查看路线信息、接收通知并管理个人出行记录。

### 主要功能

- 🔐 **用户认证系统** - 登录、注册、密码管理
- 🚌 **实时巴士追踪** - 查看巴士位置和状态
- 🗺️ **交互式地图** - 显示巴士路线和站点
- 🔔 **智能通知** - 到站提醒和服务更新
- 👤 **用户资料管理** - 个人信息和偏好设置
- 📊 **出行统计** - 个人出行数据分析

## 技术栈

- **.NET MAUI** - 跨平台移动应用框架
- **C#** - 主要编程语言
- **XAML** - UI标记语言
- **CommunityToolkit.Maui** - UI组件库
- **Plugin.LocalNotification** - 本地通知插件

## 项目结构

```
Mobile_App_Develop/
├── Models/                 # 数据模型
│   ├── User.cs            # 用户模型
│   ├── Bus.cs             # 巴士模型
│   ├── Route.cs           # 路线模型
│   ├── BusStop.cs         # 巴士站点模型
│   └── Notification.cs    # 通知模型
├── Services/              # 服务层
│   ├── IAuthService.cs    # 认证服务接口
│   ├── AuthService.cs     # 认证服务实现
│   ├── IBusService.cs     # 巴士服务接口
│   ├── BusService.cs      # 巴士服务实现
│   ├── INotificationService.cs  # 通知服务接口
│   └── NotificationService.cs   # 通知服务实现
├── Views/                 # 页面视图
│   ├── LoginPage.xaml     # 登录页面
│   ├── RegisterPage.xaml  # 注册页面
│   ├── DashboardPage.xaml # 仪表板页面
│   ├── MapPage.xaml       # 地图页面
│   ├── NotificationsPage.xaml # 通知页面
│   └── ProfilePage.xaml   # 用户资料页面
├── AppShell.xaml          # 应用导航结构
├── MauiProgram.cs         # 应用配置和依赖注入
└── MainPage.xaml          # 主页面
```

## 核心功能实现

### 1. 用户认证系统

#### 登录功能
- 邮箱和密码验证
- 记住登录状态
- 错误处理和用户反馈
- 演示账户支持

```csharp
// 演示账户
Email: john.doe@student.uts.edu.au
Password: password123

Email: jane.smith@student.uts.edu.au  
Password: password456
```

#### 注册功能
- 表单验证（姓名、学号、邮箱、密码）
- 学号格式验证（8位数字）
- 密码强度检查
- 条款和隐私政策确认

### 2. 巴士追踪系统

#### 实时位置更新
- 模拟巴士GPS位置
- 自动位置更新（每5秒）
- 巴士状态管理（运行中、延误、维护中）
- 到站时间估算

#### 路线管理
- 多条巴士路线支持
- 站点信息管理
- 路线可视化显示

### 3. 地图功能

#### 静态地图实现
- 巴士位置标记
- 路线和站点显示
- 交互式巴士信息
- 地图控制按钮

#### 巴士交互
- 点击巴士查看详情
- 到站时间查询
- 设置到站提醒
- 查看路线详情

### 4. 通知系统

#### 本地通知
- 到站提醒
- 服务中断通知
- 路线更新提醒
- 自定义通知设置

#### 通知管理
- 通知历史记录
- 标记已读/未读
- 批量操作（全部已读、清空）
- 通知设置开关

### 5. 用户资料管理

#### 个人信息
- 用户资料编辑
- 头像显示（首字母）
- 学生信息管理

#### 出行统计
- 本周出行次数
- 收藏路线数量
- 总出行距离
- 历史记录查看

#### 应用设置
- 推送通知开关
- 位置服务设置
- 深色模式切换
- 密码修改

## 依赖注入配置

应用使用依赖注入模式管理服务和页面：

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // 注册服务
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IBusService, BusService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<AppShell>();

        // 注册页面
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<ProfilePage>();

        return builder.Build();
    }
}
```

## 导航系统

应用使用Shell导航模式，支持：

- 基于认证状态的动态导航
- 底部标签栏导航
- 页面路由注册
- 深度链接支持

```csharp
// AppShell.xaml.cs 中的路由注册
Routing.RegisterRoute("login", typeof(LoginPage));
Routing.RegisterRoute("register", typeof(RegisterPage));
Routing.RegisterRoute("dashboard", typeof(DashboardPage));
Routing.RegisterRoute("map", typeof(MapPage));
Routing.RegisterRoute("notifications", typeof(NotificationsPage));
Routing.RegisterRoute("profile", typeof(ProfilePage));
```

## 数据模型

### User（用户模型）
```csharp
public class User
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string StudentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
```

### Bus（巴士模型）
```csharp
public class Bus
{
    public string Id { get; set; }
    public string Number { get; set; }
    public string RouteId { get; set; }
    public BusStatus Status { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int PassengerCount { get; set; }
    public int Capacity { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

### Route（路线模型）
```csharp
public class Route
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<BusStop> BusStops { get; set; }
    public bool IsActive { get; set; }
    public string Color { get; set; }
}
```

## 服务架构

### 认证服务（AuthService）
- 用户登录/注册
- 会话管理
- 密码操作
- 用户信息更新

### 巴士服务（BusService）
- 巴士位置追踪
- 路线信息管理
- 到站时间估算
- 实时数据更新

### 通知服务（NotificationService）
- 本地通知发送
- 通知历史管理
- 通知设置管理
- 事件处理

## UI设计特点

### 深色主题
- 统一的深色配色方案
- 高对比度文本
- 现代化UI元素
- 良好的视觉层次

### 响应式布局
- 适配不同屏幕尺寸
- 灵活的网格系统
- 自适应字体大小
- 触摸友好的交互元素

### 用户体验
- 加载状态指示
- 错误处理和反馈
- 直观的导航流程
- 一致的交互模式

## 安装和运行

### 环境要求
- Visual Studio 2022 或 Visual Studio Code
- .NET 7.0 或更高版本
- Android SDK（Android开发）
- Xcode（iOS开发，仅Mac）

### 安装步骤

1. **克隆项目**
   ```bash
   git clone [项目地址]
   cd Mobile_App_Develop
   ```

2. **还原NuGet包**
   ```bash
   dotnet restore
   ```

3. **构建项目**
   ```bash
   dotnet build
   ```

4. **运行应用**
   ```bash
   # Android
   dotnet build -t:Run -f net7.0-android
   
   # iOS (仅Mac)
   dotnet build -t:Run -f net7.0-ios
   
   # Windows
   dotnet build -t:Run -f net7.0-windows10.0.19041.0
   ```

## 测试账户

应用包含以下测试账户：

| 姓名 | 邮箱 | 密码 | 学号 |
|------|------|------|------|
| John Doe | john.doe@student.uts.edu.au | password123 | 12345678 |
| Jane Smith | jane.smith@student.uts.edu.au | password456 | 87654321 |

## 功能演示

### 1. 用户认证流程
1. 启动应用，显示登录页面
2. 使用测试账户登录
3. 成功登录后进入仪表板

### 2. 巴士追踪
1. 在仪表板查看巴士列表
2. 点击地图标签查看巴士位置
3. 点击巴士标记查看详细信息

### 3. 通知功能
1. 进入通知页面
2. 点击"发送测试通知"按钮
3. 查看通知历史和管理选项

### 4. 用户资料
1. 进入资料页面
2. 查看个人信息和统计数据
3. 编辑资料或修改设置

## 已知限制

1. **地图功能**：当前使用静态地图实现，未集成真实地图API
2. **数据持久化**：使用内存存储，应用重启后数据重置
3. **实时通信**：巴士位置更新为模拟数据
4. **网络功能**：所有数据为本地模拟，无后端API集成

## 未来改进计划

1. **集成真实地图API**（Google Maps/Apple Maps）
2. **实现数据持久化**（SQLite/Entity Framework）
3. **添加后端API集成**
4. **实现推送通知**
5. **添加离线功能支持**
6. **性能优化和缓存机制**
7. **单元测试和集成测试**
8. **多语言支持**

## 开发团队

- **开发者**：UTS Mobile Development Team
- **项目类型**：学术项目
- **开发时间**：2024年
- **版本**：1.0.0

## 许可证

本项目仅用于学术目的，版权归UTS所有。

## 联系信息

如有问题或建议，请联系：
- 邮箱：support@utsbus.edu.au
- 电话：1800 UTS BUS

---

*最后更新：2024年*