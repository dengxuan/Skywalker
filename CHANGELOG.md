# Changelog

本项目所有显著变更将记录在本文件中。

格式参考 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/)，
版本号遵循 [Semantic Versioning](https://semver.org/lang/zh-CN/)。

## [2.0.0] - 规划中

v2.0 是 Skywalker 的差异化战役，定位 **小、快、易用**：

- 全面 Source Generator 化：消灭运行时反射、消灭 `Reflection.Emit`。
- 移除 `Castle.Core` 依赖（~600 KB）。
- NativeAOT publish 零警告。
- `AddSkywalker()` 一行启动，零样板注册。

设计与规划文档：

- [`docs/architecture/v2.0-roadmap.md`](docs/architecture/v2.0-roadmap.md)
- [`docs/architecture/source-generators-spec.md`](docs/architecture/source-generators-spec.md)
- [`docs/architecture/source-generators-quality.md`](docs/architecture/source-generators-quality.md)

发版策略：`main` → `2.0.0-alpha.x` → `2.0.0-preview.x` → `2.0.0-rc.x` → `2.0.0`。
每个阶段最低浸泡 2 周，详见质量规范。

## [Unreleased]

### Removed

- **BREAKING**：移除 `Skywalker.Ddd.Application` 对 [AutoMapper](https://github.com/AutoMapper/AutoMapper) 的依赖（AutoMapper 自 v15 起转向商业授权）。
- 删除 `SkywalkerProfile`。
- 移除 `ApplicationService` 的 `IMapper Mapper` 属性与构造函数参数。
- 移除 `eng/Versions.props` 中的 `AutoMapperVersion`。

### Changed

- `ApplicationService` 现在为无参基类，业务派生类请自行注入所需服务。
- 全部文档（README、CONTRIBUTING、`docs/`、`samples/`）的对象映射示例改为使用 [Riok.Mapperly](https://github.com/riok/mapperly) 源生成器。

### Migration Guide

派生 `ApplicationService` 的业务代码需要按以下步骤迁移：

1. **移除构造函数中的 `IMapper` 参数**

   ```diff
   - public OrderAppService(IMapper mapper, IOrderRepository repo) : base(mapper)
   + public OrderAppService(IOrderRepository repo)
     {
         _repo = repo;
     }
   ```

2. **业务项目添加 Mapperly 包引用**

   ```xml
   <PackageReference Include="Riok.Mapperly" Version="4.1.1" />
   ```

3. **将 `Profile` 改为 `[Mapper] partial class`**

   ```diff
   - public class OrderProfile : Profile
   - {
   -     public OrderProfile() => CreateMap<Order, OrderDto>();
   - }
   + [Mapper]
   + public partial class OrderMapper
   + {
   +     public partial OrderDto ToDto(Order entity);
   +     public partial List<OrderDto> ToDtoList(IEnumerable<Order> entities);
   + }
   ```

4. **替换调用点**

   ```diff
   - return Mapper.Map<OrderDto>(order);
   + return _mapper.ToDto(order);
   ```

5. **移除 DI 中对 AutoMapper 的注册**（如有）：删除 `services.AddAutoMapper(...)`。

### Why

- AutoMapper 自 v15 起转向商业授权，框架不应将商业许可成本传递给使用者。
- Mapperly 是 MIT 许可的源生成器，编译期生成映射代码，零运行时反射开销，对原生 AOT 友好，重构时编译器即可发现字段不匹配。
- 框架原本就未在 DI 容器中注册 AutoMapper，派生 `ApplicationService` 的服务实际无法解析 `IMapper`，移除此依赖同时修复该潜在缺陷。
