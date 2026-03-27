# Skywalker 模块文档

本目录包含 Skywalker 框架所有模块的详细文档。

---

## 模块分类

| 分类 | 文档 | 包含模块 |
|------|------|----------|
| **DDD 核心** | [ddd.md](./ddd.md) | Domain、Application、Uow、EntityFrameworkCore |
| **事件总线** | [eventbus.md](./eventbus.md) | Local、RabbitMQ |
| **缓存** | [caching.md](./caching.md) | Memory、Redis |
| **设置管理** | [settings.md](./settings.md) | Settings 全系列 |
| **权限管理** | [permissions.md](./permissions.md) | Permissions 全系列 |
| **本地化** | [localization.md](./localization.md) | Json、EntityFrameworkCore |
| **模板引擎** | [template.md](./template.md) | Scriban、Razor |
| **数据验证** | [validation.md](./validation.md) | FluentValidation |
| **序列化** | [serialization.md](./serialization.md) | Json |
| **短信服务** | [sms.md](./sms.md) | Aliyun |
| **邮件服务** | [emailing.md](./emailing.md) | Emailing、Template |
| **健康检查** | [healthchecks.md](./healthchecks.md) | AspNetCore、EF、Redis、RabbitMQ |
| **扩展模块** | [extensions.md](./extensions.md) | Threading、Timezone、VirtualFileSystem 等 |

---

## 模块依赖关系图

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         Skywalker.Ddd (整合包)                           │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
        ┌───────────────────────────┼───────────────────────────┐
        │                           │                           │
        ▼                           ▼                           ▼
┌───────────────────┐   ┌───────────────────┐   ┌───────────────────┐
│   Ddd.Domain      │   │  Ddd.Application  │   │     Ddd.Uow       │
└─────────┬─────────┘   └─────────┬─────────┘   └─────────┬─────────┘
          │                       │                       │
          ▼                       ▼                       ▼
┌───────────────────┐   ┌───────────────────┐   ┌───────────────────┐
│    Ddd.Core       │   │ App.Abstractions  │   │Extensions.Universal│
└───────────────────┘   └───────────────────┘   └───────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                    Ddd.EntityFrameworkCore                               │
└─────────────────────────────────────────────────────────────────────────┘
          │
          ├──────────────────────────┬──────────────────────────┐
          ▼                          ▼                          ▼
┌───────────────────┐    ┌───────────────────┐    ┌───────────────────┐
│  EFCore.MySQL     │    │ EFCore.SqlServer  │    │     Ddd.Uow       │
└───────────────────┘    └───────────────────┘    └───────────────────┘
```

---

## 快速导航

### 核心模块

- [Skywalker.Ddd](./ddd.md#skywalkerddd) - DDD 整合包
- [Skywalker.Ddd.Domain](./ddd.md#skywalkerddd-domain) - 领域层核心
- [Skywalker.Ddd.Application](./ddd.md#skywalkerddd-application) - 应用层核心
- [Skywalker.Ddd.Uow](./ddd.md#skywalkerddd-uow) - 工作单元
- [Skywalker.Ddd.EntityFrameworkCore](./ddd.md#skywalkerddd-entityframeworkcore) - EF Core 集成

### 基础设施模块

- [Skywalker.EventBus.Local](./eventbus.md#skywalkeventbuslocal) - 本地事件总线
- [Skywalker.EventBus.RabbitMQ](./eventbus.md#skywalkeventbusrabbitmq) - 分布式事件总线
- [Skywalker.Caching.Redis](./caching.md#skywalkercachingredis) - Redis 缓存

### 功能模块

- [Skywalker.Settings](./settings.md) - 设置管理
- [Skywalker.Permissions](./permissions.md) - 权限管理
- [Skywalker.Localization](./localization.md) - 本地化
- [Skywalker.Template](./template.md) - 模板引擎
- [Skywalker.Validation](./validation.md) - 数据验证

---

## 模块版本

所有模块版本统一管理，当前版本：**1.0.0-rc.1**

版本定义文件：[eng/Versions.props](../../eng/Versions.props)

