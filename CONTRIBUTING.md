# 贡献指南

感谢你对 Skywalker 框架的关注！我们欢迎各种形式的贡献。

---

## 目录

1. [贡献方式](#贡献方式)
2. [分支策略](#-分支策略)
3. [开发流程](#-开发流程)
4. [提交规范](#-提交规范)
5. [PR 检查清单](#-pr-检查清单)
6. [开发环境](#-开发环境)
7. [项目结构](#-项目结构)
8. [编码规范](#-编码规范)
9. [测试规范](#-测试规范)
10. [文档规范](#-文档规范)
11. [获取帮助](#-获取帮助)

---

## 贡献方式

我们欢迎以下形式的贡献：

| 贡献类型 | 说明 |
|----------|------|
| 🐛 **Bug 报告** | 发现问题？请提交 Issue |
| 💡 **功能建议** | 有好想法？欢迎讨论 |
| 📝 **文档改进** | 完善文档、修正错误 |
| 🔧 **代码贡献** | 修复 Bug、实现新功能 |
| 🧪 **测试用例** | 增加测试覆盖率 |
| 🌐 **国际化** | 翻译文档和资源 |

---

## 🌿 分支策略

我们采用 Git Flow 分支策略：

| 分支 | 用途 | 说明 |
|------|------|------|
| `main` | 稳定发布版本 | 只接受 release 分支合并 |
| `dev` | 开发主线 | 所有 PR 的目标分支 |
| `feature/*` | 功能开发 | 如 `feature/issue-123-add-caching` |
| `fix/*` | Bug 修复 | 如 `fix/issue-456-null-reference` |
| `refactor/*` | 代码重构 | 如 `refactor/issue-789-repository` |
| `docs/*` | 文档更新 | 如 `docs/issue-29-readme` |
| `test/*` | 测试相关 | 如 `test/issue-100-unit-tests` |
| `release/*` | 发布准备 | 如 `release/v1.0.0` |

### 分支命名规范

```
<类型>/issue-<编号>-<简短描述>
```

示例：
- `feature/issue-123-add-redis-caching`
- `fix/issue-456-fix-null-reference`
- `docs/issue-29-update-readme`

---

## 🚀 开发流程

### 1. Fork 仓库

点击右上角的 Fork 按钮，将仓库 Fork 到你的账号下。

### 2. 克隆仓库

```bash
git clone https://github.com/你的用户名/Skywalker.git
cd Skywalker
```

### 3. 添加上游仓库

```bash
git remote add upstream https://github.com/L8CHAT/Skywalker.git
```

### 4. 同步最新代码

```bash
git fetch upstream
git checkout dev
git merge upstream/dev
```

### 5. 创建功能分支

```bash
# 从 dev 分支创建
git checkout dev
git pull upstream dev
git checkout -b feature/issue-编号-简短描述
```

### 6. 开发与提交

```bash
# 开发完成后
git add .
git commit -m "feat: 添加xxx功能 (#issue编号)"
```

### 7. 推送并创建 PR

```bash
git push origin feature/issue-编号-简短描述
```

然后在 GitHub 上创建 Pull Request，**目标分支为 `dev`**（不是 `main`）。

### 8. 代码审查

- 等待维护者审查代码
- 根据反馈进行修改
- 所有检查通过后合并

---

## 📝 提交规范

我们使用 [Conventional Commits](https://www.conventionalcommits.org/) 规范：

### 提交类型

| 类型 | 说明 | 示例 |
|------|------|------|
| `feat` | 新功能 | `feat: 添加 Redis 缓存支持` |
| `fix` | Bug 修复 | `fix: 修复空引用异常` |
| `docs` | 文档更新 | `docs: 更新 API 文档` |
| `style` | 代码格式（不影响功能） | `style: 格式化代码` |
| `refactor` | 重构（不是新功能或修复） | `refactor: 重构仓储实现` |
| `perf` | 性能优化 | `perf: 优化查询性能` |
| `test` | 测试相关 | `test: 添加单元测试` |
| `chore` | 构建/工具/依赖 | `chore: 更新依赖版本` |
| `ci` | CI/CD 相关 | `ci: 添加 GitHub Actions` |

### 提交格式

```
<类型>(<范围>): <描述> (#issue编号)

[可选的正文]

[可选的脚注]
```

### 示例

```bash
# 简单提交
feat: 添加多租户支持 (#23)

# 带范围的提交
fix(caching): 修复缓存过期时间计算错误 (#45)

# 带正文的提交
feat(eventbus): 添加 RabbitMQ 事件总线支持 (#67)

实现了基于 RabbitMQ 的分布式事件总线：
- 支持发布/订阅模式
- 支持消息持久化
- 支持死信队列

Closes #67

# 破坏性变更
feat(repository)!: 重构仓储接口 (#89)

BREAKING CHANGE: IRepository 接口签名已更改
- FindAsync 返回类型从 TEntity 改为 TEntity?
- 移除了 GetAllAsync 方法，使用 GetListAsync 替代
```

---

## ✅ PR 检查清单

提交 PR 前，请确保：

### 代码质量

- [ ] 代码已自测通过
- [ ] 代码符合项目编码规范
- [ ] 没有引入新的编译警告
- [ ] 没有引入新的代码异味

### 测试

- [ ] 已添加/更新相关单元测试
- [ ] 所有测试通过 (`dotnet test`)
- [ ] 测试覆盖率没有下降

### 文档

- [ ] 已更新相关 XML 注释
- [ ] 已更新相关文档（如适用）
- [ ] README 已更新（如适用）

### PR 信息

- [ ] PR 标题符合提交规范
- [ ] PR 描述清晰说明了变更内容
- [ ] 已关联相关 Issue

---

## 🛠 开发环境

### 必需工具

| 工具 | 版本 | 说明 |
|------|------|------|
| .NET SDK | 8.0+ | [下载](https://dotnet.microsoft.com/download) |
| Git | 2.30+ | 版本控制 |
| IDE | - | VS 2022 / VS Code / Rider |

### 推荐工具

| 工具 | 用途 |
|------|------|
| Docker | 运行 MySQL、Redis、RabbitMQ |
| MySQL Workbench | 数据库管理 |
| Redis Desktop Manager | Redis 管理 |

### 环境设置

```bash
# 1. 克隆仓库
git clone https://github.com/L8CHAT/Skywalker.git
cd Skywalker

# 2. 还原依赖
dotnet restore

# 3. 构建项目
dotnet build

# 4. 运行测试
dotnet test

# 5. 启动依赖服务（可选）
docker-compose up -d
```

### docker-compose.yml 示例

```yaml
version: '3.8'
services:
  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: root
      MYSQL_DATABASE: skywalker
    ports:
      - "3306:3306"

  redis:
    image: redis:7
    ports:
      - "6379:6379"

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
```

---

## 📁 项目结构

```
Skywalker/
├── src/                              # 源代码
│   ├── Skywalker.Ddd/               # DDD 整合包
│   ├── Skywalker.Ddd.Domain/        # 领域层核心
│   │   └── Skywalker/Ddd/Domain/
│   │       ├── Entities/            # 实体基类
│   │       ├── Repositories/        # 仓储接口
│   │       └── Services/            # 领域服务
│   ├── Skywalker.Ddd.Application/   # 应用层核心
│   │   └── Skywalker/Ddd/Application/
│   │       └── Services/            # 应用服务基类
│   ├── Skywalker.Ddd.Uow/           # 工作单元
│   ├── Skywalker.Ddd.EntityFrameworkCore/  # EF Core 集成
│   ├── Skywalker.EventBus.Abstractions/    # 事件总线抽象
│   ├── Skywalker.EventBus.Local/           # 本地事件总线
│   ├── Skywalker.EventBus.RabbitMQ/        # RabbitMQ 事件总线
│   ├── Skywalker.Caching.Abstractions/     # 缓存抽象
│   ├── Skywalker.Caching.Memory/           # 内存缓存
│   ├── Skywalker.Caching.Redis/            # Redis 缓存
│   ├── Skywalker.Settings.Abstractions/    # 设置管理
│   ├── Skywalker.Permissions.Abstractions/ # 权限管理
│   ├── Skywalker.Localization.Abstractions/# 本地化
│   ├── Skywalker.Validation.Abstractions/  # 数据验证
│   ├── Skywalker.ObjectMapping.Abstractions/# 对象映射
│   ├── Skywalker.Template.Abstractions/    # 模板引擎
│   └── ...
├── tests/                            # 测试项目
│   ├── Skywalker.Ddd.Domain.Tests/
│   ├── Skywalker.Ddd.Application.Tests/
│   └── ...
├── docs/                             # 文档
│   ├── api/                          # API 文档
│   ├── guide/                        # 使用指南
│   └── architecture/                 # 架构设计
├── samples/                          # 示例项目
├── eng/                              # 构建配置
│   ├── Versions.props               # 版本管理
│   └── Directory.Build.props        # 全局构建配置
├── .github/                          # GitHub 配置
│   ├── workflows/                    # CI/CD 工作流
│   ├── ISSUE_TEMPLATE/              # Issue 模板
│   └── PULL_REQUEST_TEMPLATE.md     # PR 模板
├── Skywalker.sln                    # 解决方案文件
├── README.md                        # 项目说明
├── CONTRIBUTING.md                  # 贡献指南
└── LICENSE                          # 许可证
```

---

## 📏 编码规范

### C# 编码规范

#### 基本规则

- 使用 4 空格缩进（不使用 Tab）
- 使用 `var` 当类型明显时
- 每行最多 120 个字符
- 文件末尾保留一个空行
- 使用 UTF-8 编码

#### 命名约定

| 类型 | 约定 | 示例 |
|------|------|------|
| 命名空间 | PascalCase | `Skywalker.Ddd.Domain` |
| 类/结构体 | PascalCase | `OrderService` |
| 接口 | I + PascalCase | `IOrderService` |
| 方法 | PascalCase | `GetOrderAsync` |
| 属性 | PascalCase | `OrderId` |
| 私有字段 | _camelCase | `_orderRepository` |
| 参数 | camelCase | `orderId` |
| 局部变量 | camelCase | `orderCount` |
| 常量 | PascalCase | `MaxRetryCount` |
| 枚举 | PascalCase | `OrderStatus` |
| 枚举值 | PascalCase | `Pending`, `Confirmed` |
| 泛型参数 | T + PascalCase | `TEntity`, `TKey` |

#### 代码组织

```csharp
// 1. using 语句（按字母顺序）
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Repositories;

/// <summary>
/// 仓储接口
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : notnull
{
    // 2. 常量

    // 3. 静态字段

    // 4. 实例字段

    // 5. 构造函数

    // 6. 属性

    // 7. 方法（按功能分组）
}
```

#### 异步编程

```csharp
// ✅ 正确：异步方法以 Async 结尾
public async Task<Order> GetOrderAsync(Guid id)
{
    return await _repository.GetAsync(id);
}

// ✅ 正确：使用 CancellationToken
public async Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
{
    return await _repository.GetAsync(id, cancellationToken);
}

// ❌ 错误：不要使用 .Result 或 .Wait()
public Order GetOrder(Guid id)
{
    return _repository.GetAsync(id).Result;  // 可能导致死锁
}
```

### XML 注释规范

```csharp
/// <summary>
/// 获取订单信息
/// </summary>
/// <param name="id">订单ID</param>
/// <param name="cancellationToken">取消令牌</param>
/// <returns>订单详情，如果不存在则抛出 EntityNotFoundException</returns>
/// <exception cref="EntityNotFoundException">当订单不存在时抛出</exception>
/// <example>
/// <code>
/// var order = await orderService.GetAsync(orderId);
/// </code>
/// </example>
public async Task<OrderDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
{
    // ...
}
```

---

## 🧪 测试规范

### 测试项目命名

```
{项目名}.Tests
```

示例：`Skywalker.Ddd.Domain.Tests`

### 测试类命名

```
{被测试类名}Tests
```

示例：`OrderTests`, `OrderRepositoryTests`

### 测试方法命名

```
{方法名}_{场景}_{预期结果}
```

示例：
- `AddItem_WhenOrderIsPending_ShouldAddItemSuccessfully`
- `AddItem_WhenOrderIsConfirmed_ShouldThrowInvalidOperationException`
- `GetAsync_WhenOrderExists_ShouldReturnOrder`
- `GetAsync_WhenOrderNotExists_ShouldThrowEntityNotFoundException`

### 测试结构 (AAA 模式)

```csharp
[Fact]
public async Task GetAsync_WhenOrderExists_ShouldReturnOrder()
{
    // Arrange - 准备测试数据
    var orderId = Guid.NewGuid();
    var order = new Order(orderId, "ORD001", Guid.NewGuid(), new Address(...));
    _mockRepository.Setup(r => r.GetAsync(orderId, default))
        .ReturnsAsync(order);

    // Act - 执行被测试方法
    var result = await _orderAppService.GetAsync(orderId);

    // Assert - 验证结果
    Assert.NotNull(result);
    Assert.Equal(orderId, result.Id);
    Assert.Equal("ORD001", result.OrderNo);
}
```

### 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定项目的测试
dotnet test tests/Skywalker.Ddd.Domain.Tests

# 运行带覆盖率的测试
dotnet test --collect:"XPlat Code Coverage"

# 运行特定测试
dotnet test --filter "FullyQualifiedName~OrderTests"
```

---

## 📚 文档规范

### 文档类型

| 类型 | 位置 | 说明 |
|------|------|------|
| API 文档 | `docs/api/` | 接口和类的详细说明 |
| 使用指南 | `docs/guide/` | 功能使用教程 |
| 架构文档 | `docs/architecture/` | 设计决策和架构说明 |
| 示例代码 | `samples/` | 完整的示例项目 |

### Markdown 规范

- 使用 ATX 风格标题（`#`）
- 代码块指定语言（如 ` ```csharp`）
- 表格对齐
- 链接使用相对路径

### 代码示例规范

```csharp
// ✅ 好的示例：完整、可运行
public class OrderAppService : ApplicationService, IOrderAppService
{
    private readonly IOrderRepository _orderRepository;

    public OrderAppService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        return ObjectMapper.Map<Order, OrderDto>(order);
    }
}

// ❌ 不好的示例：不完整、有省略
public class OrderAppService
{
    public async Task<OrderDto> GetAsync(Guid id)
    {
        // ... 省略实现
    }
}
```

---

## 💬 获取帮助

如有问题，请通过以下方式联系：

| 渠道 | 用途 |
|------|------|
| [Issues](https://github.com/L8CHAT/Skywalker/issues) | Bug 报告、功能请求 |
| [Discussions](https://github.com/L8CHAT/Skywalker/discussions) | 问题讨论、使用帮助 |
| [Pull Requests](https://github.com/L8CHAT/Skywalker/pulls) | 代码贡献 |

### 提交 Issue 前

1. 搜索是否已有相同的 Issue
2. 使用 Issue 模板
3. 提供详细的复现步骤
4. 附上相关的错误信息和日志

---

## 🙏 致谢

感谢所有贡献者的付出！

再次感谢你的贡献！🎉
