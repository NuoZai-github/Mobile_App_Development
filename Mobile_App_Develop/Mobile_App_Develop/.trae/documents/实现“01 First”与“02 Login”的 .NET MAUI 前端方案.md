## 页面与资源
- 新增 `Views/SplashPage.xaml` 与 `SplashPage.xaml.cs`
- 复用 `Views/LoginPage.xaml`（按设计校准样式与布局）
- 资源放入 `Resources/Images/`：
  - 背景波浪：`bg_first.png`
  - 品牌 Logo：`logo_brand.png`（白色版本，透明背景，约 75×60，2x/3x 由 MAUI 自动缩放）
  - 眼睛/社交图标：`eye.png`、`google.png`、`apple.png`、`facebook.png`
- 如需严格排版：可选添加字体 `Inter`、`Poppins`（放入 `Resources/Fonts/`，在 `MauiProgram.cs` 注册）

## 路由与启动
- 在 `AppShell.xaml` 顶部添加路由：`Route="splash"` 指向 `SplashPage`
- 应用启动展示 `SplashPage`；`OnAppearing` 中 1.5s 动画后 `Shell.GoToAsync("login")`
- 点击任意处可提前跳转（TapGesture）

## 01 First（启动画面）实现
- 布局使用 `Grid`（三行：状态栏、内容、底部指示条），全屏背景 `Image Source="bg_first.png"`，`Aspect="AspectFill"`
- 居中放置 Logo：`Image Source="logo_brand.png" Aspect="AspectFit" HeightRequest≈60 WidthRequest≈75`
- 底部白色指示条：`BoxView CornerRadius=2 HeightRequest=4 WidthRequest=120` 居中
- 动画：
  - Logo 由 `Opacity=0` 渐入 `FadeTo(1, 600)` + 轻微 `ScaleTo(1.05, 500, Easing.CubicOut)`
  - 底部指示条自下向上 `TranslateTo(0, -8, 500)` 并 `FadeTo(1, 400)`
- 适配：使用相对布局（`Grid` + `HorizontalOptions="Center"`、`VerticalOptions="End"`）确保不同分辨率一致视觉比例

## Logo 替换策略
- 将您提供的品牌 Logo 命名为 `logo_brand.png` 放入 `Resources/Images/`
- 保持 `AspectFit` 与固定 `WidthRequest/HeightRequest`，在不同设备由 MAUI 自动缩放，位置由 `Grid` 居中控制
- 若需深色模式，准备 `logo_brand_dark.png` 并在 `AppThemeBinding` 中切换

## 02 Login（登录页）实现
- 复核并校准现有 `Views/LoginPage.xaml`：
  - 欢迎标题 `Welcome Back` 使用 `Primary #0095FF`、字体权重 700
  - 描述文案次级文本色 `#000000`、字号 15
  - 输入框：使用 `Frame` + `Entry`，背景 `#EAF6FF`，圆角 `10`，内边距 `25,18`
  - 密码框右侧添加 `ImageButton Source="eye.png"` 切换 `Entry.IsPassword`
  - 记住我：`CheckBox` + 文本；忘记密码文本使用 `Primary` 并 `FontAttributes=Medium`
  - 登录按钮：`Button BackgroundColor=#0095FF TextColor=White CornerRadius=10 HeightRequest=60`
  - 分隔线与社交图标区：水平 `Grid` + 三个 `Image`（60×60，圆角 28）
  - 底部文案：`Don’t have an account? Create Account`，其中 `Create Account` 加粗
- 保持布局为纵向 `StackLayout/Grid`，使用间距（`RowSpacing`/`Margin`）匹配 Figma 数值

## 导航与页面切换动画
- `SplashPage` → `LoginPage`：
  - 自动：`await Task.Delay(1500); await Shell.GoToAsync("login", animate:true);`
  - 点击：为根容器添加 `TapGestureRecognizer`，回调中执行同样导航
- 进入登录页时执行入场动画：容器 `TranslateTo(0, -10, 350)` + `FadeTo(1, 350)`，按钮 `ScaleTo(1.02, 200)` 后回弹

## 示例代码（节选）
- `AppShell.xaml`
  ```xml
  <Shell ... FlyoutBehavior="Disabled">
    <ShellContent Route="splash" ContentTemplate="{DataTemplate views:SplashPage}" />
    <ShellContent Route="login" ContentTemplate="{DataTemplate views:LoginPage}" />
    <!-- 现有 TabBar 保持不变 -->
  </Shell>
  ```
- `Views/SplashPage.xaml`
  ```xml
  <ContentPage BackgroundColor="#0095FF" x:Class="Mobile_App_Develop.Views.SplashPage">
    <Grid RowDefinitions="Auto,*,Auto">
      <!-- 状态栏模拟：可选保留或使用系统状态栏 -->
      <Grid Padding="20,0" HeightRequest="44" />
      <Grid>
        <Image Source="bg_first.png" Aspect="AspectFill" />
        <Image x:Name="Logo" Source="logo_brand.png" Aspect="AspectFit" WidthRequest="75" HeightRequest="60" HorizontalOptions="Center" VerticalOptions="Center" Opacity="0" />
      </Grid>
      <Grid HeightRequest="30" HorizontalOptions="Center" VerticalOptions="End">
        <BoxView x:Name="Handle" Color="White" WidthRequest="120" HeightRequest="4" CornerRadius="2" Opacity="0" TranslationY="12" />
      </Grid>
    </Grid>
  </ContentPage>
  ```
- `Views/SplashPage.xaml.cs`
  ```csharp
  public partial class SplashPage : ContentPage
  {
    public SplashPage()
    {
      InitializeComponent();
      var tap = new TapGestureRecognizer();
      tap.Tapped += async (_, __) => await NavigateAsync();
      this.GestureRecognizers.Add(tap);
    }
    protected override async void OnAppearing()
    {
      await Logo.FadeTo(1, 600);
      await Logo.ScaleTo(1.05, 500, Easing.CubicOut);
      await Handle.TranslateTo(0, -8, 500);
      await Handle.FadeTo(1, 400);
      await Task.Delay(1500);
      await NavigateAsync();
    }
    Task NavigateAsync() => Shell.Current.GoToAsync("login", true);
  }
  ```
- `Views/LoginPage.xaml`（关键控件节选）
  ```xml
  <ContentPage x:Class="Mobile_App_Develop.Views.LoginPage" BackgroundColor="White">
    <ScrollView>
      <Grid Padding="25" RowSpacing="30">
        <!-- 标题区、输入框、按钮、社交登录区，按 Figma 数值设置 Margin/HeightRequest/CornerRadius -->
      </Grid>
    </ScrollView>
  </ContentPage>
  ```

## 设备适配与最佳实践
- 使用 `AspectFit/Fill`、居中与栅格比例避免绝对定位；不依赖固定像素，保证在 iOS/Android/Windows 上一致视觉
- 颜色统一在 `Resources/Styles/Colors.xaml` 管理：`Primary=#0095FF`、`White=#FFFFFF`、`GrayLine=#EBEBEB`
- 动画均使用 MAUI 原生 `ViewExtensions` 方法（`FadeTo/TranslateTo/ScaleTo`）
- 访问性：为按钮与图标添加 `SemanticProperties.Description`

## 交付内容
- 新增 `SplashPage.xaml/.cs`，对齐“01 First”设计
- 校准后的 `LoginPage.xaml` 与必要图片资源
- 说明文档（简明步骤与配置点）
- 所有改动仅限 XAML 与 C#，不引入外部库，保持 Shell 结构与现有导航

请确认以上方案；确认后我将按此计划在现有项目中落地实现并提交完整页面与资源。