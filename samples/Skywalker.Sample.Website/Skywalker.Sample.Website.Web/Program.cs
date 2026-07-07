using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Caching.Abstractions;
using Skywalker.Extensions.DynamicProxies;
using Skywalker.Extensions.VirtualFileSystem;
using Skywalker.Localization;
using Skywalker.Localization.AspNetCore;
using Skywalker.Localization.Json;
using Skywalker.Permissions;
using Skywalker.Permissions.Abstractions;
using Skywalker.Sample.Website.Application;
using Skywalker.Sample.Website.EntityFrameworkCore;
using Skywalker.Sample.Website.Web.Infrastructure;
using Skywalker.Security.Claims;
using Skywalker.Security.Users;
using Skywalker.Settings.Abstractions;
using Skywalker.Settings;
using Microsoft.EntityFrameworkCore;

var smoke = args.Contains("--smoke");

var builder = WebApplication.CreateBuilder(args);
if (smoke)
{
    builder.WebHost.UseUrls("http://127.0.0.1:0");
}

var services = builder.Services;

// ── DDD 内核 + DI SG（[ApplicationService]/[Service]/[EventHandler] 编译期生成注册）──
services.AddSkywalker(typeof(Program).Assembly)
    .AddAspNetCore();

// ── 拦截器（DynamicProxy SG 静态代理）──
// 注意：需在 AddSkywalkerDbContext 之前调用——AddInterceptedServices 只代理此刻已注册、
// 且拥有生成代理的服务；EF SG 生成的 IDomainService<,> 闭合泛型注册暂无代理支持（见 #307）。
services.AddTransient<IInterceptor, AuditInterceptor>();
services.AddInterceptedServices();

// ── EF Core：EF SG 生成仓储/领域服务注册（零反射）──
services.AddSkywalkerDbContext<WebsiteDbContext>(options =>
{
    options.Configure(context => context.DbContextOptions.UseInMemoryDatabase("Skywalker.Sample.Website"));
});

// ── 事件总线（本地 channel）：订阅领域事件处理器 ──
services.AddTransient<MemberRegisteredEventHandler>();
services.AddTransient<OrderConfirmedEventHandler>();
services.AddEventBusLocal(options =>
{
    options.AddEventHandler<MemberRegisteredEventHandler>();
    options.AddEventHandler<OrderConfirmedEventHandler>();
});

// ── 邮件（NullEmailSender 兜底，生产换 AddSmtpEmailing）/ 短信（NullSmsSender）──
services.AddEmailing(options =>
{
    options.DefaultFromAddress = "noreply@shop.local";
    options.DefaultFromDisplayName = "Skywalker Shop";
});
services.AddSms();

// ── 虚拟文件系统 + 模板引擎（Scriban）：模板内容是站点资产（嵌入资源）──
services.AddVirtualFileSystem();
services.Configure<SkywalkerVirtualFileSystemOptions>(options =>
{
    options.FileSets.AddEmbedded<Program>(baseNamespace: "Skywalker.Sample.Website.Web");
});
// Localization.Json 解析的是 IFileProvider，把 VFS 桥接过去
services.AddSingleton<Microsoft.Extensions.FileProviders.IFileProvider>(sp => sp.GetRequiredService<IVirtualFileProvider>());
services.AddTemplate(options =>
{
    options.DefinitionProviders.Add<WebsiteTemplateDefinitionProvider>();
    options.ContentContributors.Add<Skywalker.Template.VirtualFiles.VirtualFileTemplateContentContributor>();
});
services.AddScribanTemplate();
// Template 管线按具体类型从 DI 解析 options 里登记的引擎/贡献者，补注册具体类型
services.AddTransient<Skywalker.Template.Scriban.ScribanTemplateRenderingEngine>();
services.AddTransient<Skywalker.Template.VirtualFiles.VirtualFileTemplateContentContributor>();
services.AddSingleton<WebsiteTemplateDefinitionProvider>();

// ── 本地化（JSON 资源，中英双语）──
// Scriban 模板引擎依赖微软的 IStringLocalizerFactory，两套本地化并存
services.AddLocalization();
services.AddSkywalkerLocalization(options =>
{
    options.DefaultResourceType = typeof(WebsiteResource);
    options.Resources.Add<WebsiteResource>("en").AddJson("/Localization");
    options.Languages.Add(new LanguageInfo("en", "English", isDefault: true));
    options.Languages.Add(new LanguageInfo("zh-CN", "简体中文"));
});

// ── 设置（定义默认值 + appsettings 覆盖）──
services.AddSettings(options =>
{
    options.ValueProviders.Add<ConfigurationSettingValueProvider>();
    options.ValueProviders.Add<DefaultValueSettingValueProvider>();
});
services.AddSingleton<ISettingDefinitionProvider, WebsiteSettingDefinitionProvider>();

// ── 权限（角色/用户值提供者 + 内存授权）──
services.AddPermissions();
services.Configure<PermissionOptions>(options =>
{
    options.DefinitionProviders.Add<WebsitePermissionDefinitionProvider>();
    options.ValueProviders.Add<RolePermissionValueProvider>();
    options.ValueProviders.Add<UserPermissionValueProvider>();
});
services.AddSingleton<WebsitePermissionDefinitionProvider>();
services.AddSingleton<RolePermissionValueProvider>();
services.AddSingleton<UserPermissionValueProvider>();

// ── 安全（ICurrentUser 读取 HttpContext.User）──
services.AddSecurity();
services.AddHttpContextAccessor();
services.Replace(ServiceDescriptor.Singleton<ICurrentPrincipalAccessor, HttpContextCurrentPrincipalAccessor>());

// ── 验证（FluentValidation 扫描 Application 程序集）──
services.AddFluentValidation<PlaceOrderInputValidator>();

// ── 缓存（进程内实现；生产换 AddRedisCaching）──
services.AddSingleton<ICachingProvider, MemoryCachingProvider>();

// ── 健康检查 ──
services.AddSkywalkerHealthChecks();

// ── MVC 控制器（UoW 中间件按 ActionDescriptor 识别工作单元边界；响应经 ResultFilter 包装为 AjaxResponse）──
services.AddControllers();

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

// ── 中间件管线：请求本地化 → 演示认证 → 异常处理 + 工作单元 ──
app.UseSkywalkerRequestLocalization();
app.UseMiddleware<DemoAuthenticationMiddleware>();
app.UseSkywalker();

// ── 端点（见 Controllers.cs）──
app.MapControllers();
app.MapSkywalkerHealthChecks();

if (!smoke)
{
    await app.RunAsync();
    return 0;
}

// ── smoke 模式：进程内跑通全站流程并断言 ──
await app.StartAsync();
try
{
    var exitCode = await SmokeRunner.RunAsync(app);
    Console.WriteLine(exitCode == 0
        ? "Sample: Website — full-stack smoke passed."
        : "Sample: Website — smoke FAILED.");
    return exitCode;
}
finally
{
    await app.StopAsync();
}
