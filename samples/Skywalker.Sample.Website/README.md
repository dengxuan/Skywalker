# Skywalker.Sample.Website — 全功能参考站点

一个小型商城站点，按 DDD 分层组织，**组合 Skywalker 的全部功能族**，是框架的端到端参考实现，也是 CI 的全栈冒烟门禁（`dotnet run -- --smoke`）。

## 分层结构

```
Skywalker.Sample.Website.Domain              ← 聚合根（Member/Order）/实体/领域事件
Skywalker.Sample.Website.Application         ← [ApplicationService] 应用服务/DTO/验证器/事件处理器（DI SG + DynamicProxy SG）
Skywalker.Sample.Website.EntityFrameworkCore ← WebsiteDbContext（EF SG 生成仓储注册）
Skywalker.Sample.Website.Web                 ← Host：Program.cs 组合全家桶 + MVC 控制器 + smoke
```

## 功能 → 框架能力映射

| 站点功能 | 框架能力 |
|---|---|
| 会员注册 → 验证码短信 + 验证邮件 → 输码激活 | 聚合根领域事件 → EventBus.Local → `ISmsSender` + `IEmailSender` + Scriban 模板渲染 |
| 商品目录（带缓存） | EF SG 仓储 + `ICachingProvider`（进程内实现，可换 Redis） |
| 管理端改价（admin） | `IPermissionChecker` + 角色值提供者 + `ICurrentPrincipalAccessor` |
| 下单 → 免邮计算 → 确认 → 确认邮件 | FluentValidation + `ISettingProvider` + `IPricingService` 静态代理拦截 + UoW 事务 + 提交后事件 |
| 中英双语 | `IStringLocalizer<WebsiteResource>` + JSON 资源（VFS 嵌入） |
| 错误契约 | 异常处理中间件/过滤器 + `AjaxResponse` 信封（`x-wrap-result: false` 取原始契约） |
| 运维 | `MapSkywalkerHealthChecks()` |

## 运行

```bash
# 常规启动（http://localhost:5000 由 ASP.NET Core 默认决定）
dotnet run --project Skywalker.Sample.Website.Web

# CI 自验证：进程内按真实用户路径打全部端点并断言（20 项）
dotnet run --project Skywalker.Sample.Website.Web -- --smoke
```

演示认证：请求头 `X-Demo-User: u-1001`、`X-Demo-Roles: admin`（真实站点替换为 JWT/Cookie 认证，下游组件不感知差异）。

## 值得注意的组合要点（踩坑记录）

1. **`AddInterceptedServices()` 必须在 `AddSkywalkerDbContext` 之前**：EF SG 生成的 `IDomainService<,>` 闭合泛型注册暂无生成代理（见 #307）。
2. 应用服务接口应继承 `Skywalker.Ddd.Application.Abstractions.IApplicationService`：UoW 中间件只**预留**工作单元，由静态代理上的 `UnitOfWorkInterceptor` 在应用服务方法边界接管。
3. 单文件模板需 `WithVirtualFilePath(..., isInlineLocalized: true)`，并给 `TemplateDefinition` 指定 `LocalizationResource` 类型。
4. Template/Settings/Permissions 的 options 类型列表按**具体类型**从 DI 解析，登记的引擎/贡献者/提供者需同时注册进 DI。
5. `Localization.Json` 解析 `IFileProvider`，需把 `IVirtualFileProvider` 桥接注册。
