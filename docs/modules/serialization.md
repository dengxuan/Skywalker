# 序列化模块

本文档详细介绍 Skywalker 框架的序列化模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Serialization.Abstractions](#skywalkerserializationabstractions)
3. [Skywalker.Serialization.NewtonsoftJson](#skywalkerserializationnewtonsoft)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Serialization.Abstractions | `Skywalker.Serialization.Abstractions` | 序列化抽象接口 |
| Skywalker.Serialization.NewtonsoftJson | `Skywalker.Serialization.NewtonsoftJson` | Newtonsoft.Json 实现 |

### 依赖关系

```
Skywalker.Serialization.NewtonsoftJson
├── Skywalker.Serialization.Abstractions
├── Skywalker.Extensions.Timezone
└── Newtonsoft.Json (13.0.1)
```

---

## Skywalker.Serialization.Abstractions

### 简介

序列化抽象模块，定义序列化的核心接口。

### 安装

```bash
dotnet add package Skywalker.Serialization.Abstractions
```

### 核心类型

#### ISerializer - 序列化器接口

```csharp
namespace Skywalker.Serialization.Abstractions;

public interface ISerializer
{
    /// <summary>
    /// 将对象序列化为 JSON 字符串
    /// </summary>
    /// <param name="object">要序列化的对象</param>
    /// <param name="camelCase">是否使用驼峰命名</param>
    /// <param name="indented">是否格式化输出</param>
    string Serialize(object @object, bool camelCase = true, bool indented = false);
    
    /// <summary>
    /// 将字节数组反序列化为对象
    /// </summary>
    T? Deserialize<T>(byte[] bytes, bool camelCase = true);
    
    /// <summary>
    /// 将字节数组反序列化为指定类型的对象
    /// </summary>
    object? Deserialize(Type type, byte[] bytes, bool camelCase = true);
}
```

---

## Skywalker.Serialization.NewtonsoftJson

### 简介

基于 Newtonsoft.Json 的序列化实现，支持时区转换。

### 安装

```bash
dotnet add package Skywalker.Serialization.NewtonsoftJson
```

### 核心类型

#### NewtonsoftJsonSerializer

```csharp
namespace Skywalker.Serialization.NewtonsoftJson;

public class NewtonsoftJsonSerializer : ISerializer
{
    public string Serialize(object @object, bool camelCase = true, bool indented = false);
    public T? Deserialize<T>(byte[] bytes, bool camelCase = true);
    public object? Deserialize(Type type, byte[] bytes, bool camelCase = true);
}
```

#### NewtonsoftJsonIsoDateTimeConverter

```csharp
namespace Skywalker.Serialization.NewtonsoftJson;

/// <summary>
/// 支持时区转换的 DateTime 转换器
/// </summary>
public class NewtonsoftJsonIsoDateTimeConverter : IsoDateTimeConverter
{
    public NewtonsoftJsonIsoDateTimeConverter(IClock clock, IOptions<SerializationOptions> options);
    
    public override bool CanConvert(Type objectType);
    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer);
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer);
}
```

### 服务注册

```csharp
services.AddJsonSerializer();
```

### 使用示例

```csharp
public class OrderService
{
    private readonly ISerializer _serializer;
    
    public OrderService(ISerializer serializer)
    {
        _serializer = serializer;
    }
    
    public string SerializeOrder(Order order)
    {
        // 使用驼峰命名，格式化输出
        return _serializer.Serialize(order, camelCase: true, indented: true);
    }
    
    public Order? DeserializeOrder(byte[] data)
    {
        return _serializer.Deserialize<Order>(data);
    }
}
```



