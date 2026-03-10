# 缓存模块

本文档详细介绍 Skywalker 框架的缓存模块。

---

## 目录

1. [模块概述](#模块概述)
2. [Skywalker.Caching.Abstractions](#skywalkercachingabstractions)
3. [Skywalker.Caching.Redis](#skywalkercachingredis)

---

## 模块概述

| 模块 | NuGet 包 | 说明 |
|------|----------|------|
| Skywalker.Caching.Abstractions | `Skywalker.Caching.Abstractions` | 缓存抽象接口 |
| Skywalker.Caching.Redis | `Skywalker.Caching.Redis` | Redis 缓存实现 |

### 依赖关系

```
Skywalker.Caching.Redis
├── Skywalker.Caching.Abstractions
├── Skywalker.Serialization.NewtonsoftJson
└── StackExchange.Redis
```

---

## Skywalker.Caching.Abstractions

### 简介

缓存抽象模块，定义缓存的核心接口。

### 安装

```bash
dotnet add package Skywalker.Caching.Abstractions
```

### 核心类型

#### ICaching - 缓存接口

```csharp
namespace Skywalker.Caching.Abstractions;

public interface ICaching : IDisposable
{
    /// <summary>
    /// 缓存名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 默认过期时间（默认 60 分钟）
    /// </summary>
    TimeSpan? DefaultExpireTime { get; set; }
    
    // 获取缓存
    byte[]? Get(string key);
    TValue? Get<TValue>(string key);
    ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default);
    IEnumerable<TValue?> GetMany<TValue>(IEnumerable<string> keys);
    ValueTask<IEnumerable<TValue?>> GetManyAsync<TValue>(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    
    // 设置缓存
    void Set(string key, byte[] value, TimeSpan? expireTime = null);
    void Set<TValue>(string key, TValue value, TimeSpan? expireTime = null);
    Task SetAsync<TValue>(string key, TValue value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default);
    void SetMany<TValue>(IEnumerable<KeyValuePair<string, TValue>> items, TimeSpan? expireTime = null);
    Task SetManyAsync<TValue>(IEnumerable<KeyValuePair<string, TValue>> items, TimeSpan? expireTime = null, CancellationToken cancellationToken = default);
    
    // 获取或设置缓存
    TValue GetOrSet<TValue>(string key, Func<TValue> factory);
    Task<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, CancellationToken cancellationToken = default);
    
    // 删除缓存
    void Remove(string key);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveManyAsync(string[] keys, CancellationToken cancellationToken = default);
    
    // 清空缓存
    void Clear();
    Task ClearAsync(CancellationToken cancellationToken = default);
}
```

#### ICachingProvider - 缓存提供者接口

```csharp
namespace Skywalker.Caching.Abstractions;

public interface ICachingProvider
{
    /// <summary>
    /// 获取指定名称的缓存实例
    /// </summary>
    ICaching GetCaching(string name);
}
```

#### ICachingSerializer - 缓存序列化器接口

```csharp
namespace Skywalker.Caching.Abstractions;

public interface ICachingSerializer
{
    /// <summary>
    /// 序列化对象为字节数组
    /// </summary>
    byte[] Serialize<T>(T @object);
    
    /// <summary>
    /// 反序列化字节数组为对象
    /// </summary>
    object? Deserialize(Type type, byte[]? bytes);
}
```

#### CacheAttribute - 缓存特性

```csharp
namespace Skywalker.Caching.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CacheAttribute : Attribute
{
    /// <summary>
    /// 缓存键
    /// </summary>
    public string Key { get; }
    
    /// <summary>
    /// 过期时间（秒）
    /// </summary>
    public int Expiry { get; set; }
    
    public CacheAttribute(string key)
    {
        Key = key;
    }
}
```

### 使用示例

```csharp
public class ProductService
{
    private readonly ICachingProvider _cachingProvider;
    private readonly IRepository<Product, Guid> _productRepository;
    
    public ProductService(
        ICachingProvider cachingProvider,
        IRepository<Product, Guid> productRepository)
    {
        _cachingProvider = cachingProvider;
        _productRepository = productRepository;
    }
    
    public async Task<ProductDto?> GetProductAsync(Guid id)
    {
        var cache = _cachingProvider.GetCaching("products");
        var cacheKey = $"product:{id}";
        
        // 尝试从缓存获取
        var product = await cache.GetAsync<ProductDto>(cacheKey);
        if (product != null)
        {
            return product;
        }
        
        // 从数据库获取
        var entity = await _productRepository.FindAsync(id);
        if (entity == null)
        {
            return null;
        }
        
        product = new ProductDto(entity.Id, entity.Name, entity.Price);
        
        // 写入缓存
        await cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
        
        return product;
    }
}
```

---

## Skywalker.Caching.Redis

### 简介

基于 Redis 的分布式缓存实现，使用 StackExchange.Redis 客户端。

### 安装

```bash
dotnet add package Skywalker.Caching.Redis
```

### 配置

#### appsettings.json

```json
{
  "RedisOptions": {
    "ConnectionString": "localhost:6379,password=your_password,defaultDatabase=0"
  }
}
```

### 注册服务

```csharp
// 方式 1：从配置文件读取
builder.Services.AddRedisCaching();

// 方式 2：代码配置
builder.Services.AddRedisCaching(options =>
{
    options.ConnectionString = "localhost:6379,password=your_password";
});
```

### 核心类型

#### RedisCaching - Redis 缓存实现

```csharp
namespace Skywalker.Caching.Redis;

internal class RedisCaching : ICaching
{
    private readonly IDatabase _database;
    private readonly ICachingSerializer _serializer;

    public string Name { get; }
    public TimeSpan? DefaultExpireTime { get; set; }

    internal RedisCaching(string name, IRedisDatabaseProvider redisCacheProvider, ICachingSerializer serializer)
    {
        _database = redisCacheProvider.GetDatabase();
        _serializer = serializer;
        Name = name;
        DefaultExpireTime = TimeSpan.FromHours(1);
    }

    public byte[]? Get(string key)
    {
        return _database.StringGet($"{Name}@{key}");
    }

    public TValue? Get<TValue>(string key)
    {
        var bytes = Get(key);
        return bytes == null ? default : _serializer.Deserialize<TValue>(bytes);
    }

    public void Set(string key, byte[] value, TimeSpan? expireTime = null)
    {
        _database.StringSet($"{Name}@{key}", value, expireTime ?? DefaultExpireTime);
    }

    public void Set<TValue>(string key, TValue value, TimeSpan? expireTime = null)
    {
        var bytes = _serializer.Serialize(value);
        Set(key, bytes, expireTime);
    }

    public TValue GetOrSet<TValue>(string key, Func<TValue> factory)
    {
        var value = Get<TValue>(key);
        if (value == null)
        {
            value = factory();
            if (value != null)
            {
                Set(key, value);
            }
        }
        return value;
    }

    public void Remove(string key)
    {
        _database.KeyDelete($"{Name}@{key}");
    }

    public void Clear()
    {
        // 清除当前缓存名称下的所有键
    }
}
```

#### RedisCachingProvider - Redis 缓存提供者

```csharp
namespace Skywalker.Caching.Redis;

public class RedisCachingProvider : CachingProvider
{
    private readonly IServiceProvider _serviceProvider;

    public RedisCachingProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override ICaching CreateCacheImplementation(string name)
    {
        return new RedisCaching(
            name,
            _serviceProvider.GetRequiredService<IRedisDatabaseProvider>(),
            _serviceProvider.GetRequiredService<ICachingSerializer>());
    }
}
```

#### RedisOptions - Redis 配置选项

```csharp
namespace Skywalker.Caching.Redis;

public class RedisOptions
{
    /// <summary>
    /// Redis 连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = "127.0.0.1:6379";
}
```

### 使用示例

#### 基础用法

```csharp
public class ProductService
{
    private readonly ICachingProvider _cachingProvider;
    private readonly IRepository<Product, Guid> _productRepository;

    public ProductService(
        ICachingProvider cachingProvider,
        IRepository<Product, Guid> productRepository)
    {
        _cachingProvider = cachingProvider;
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> GetProductAsync(Guid id)
    {
        var cache = _cachingProvider.GetCaching("products");
        var cacheKey = $"product:{id}";

        // 尝试从缓存获取
        var product = await cache.GetAsync<ProductDto>(cacheKey);
        if (product != null)
        {
            return product;
        }

        // 从数据库获取
        var entity = await _productRepository.FindAsync(id);
        if (entity == null)
        {
            return null;
        }

        product = new ProductDto(entity.Id, entity.Name, entity.Price);

        // 写入缓存（30分钟过期）
        await cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));

        return product;
    }

    public async Task<List<ProductDto>> GetProductsAsync(List<Guid> ids)
    {
        var cache = _cachingProvider.GetCaching("products");
        var cacheKeys = ids.Select(id => $"product:{id}").ToList();

        // 批量获取
        var cachedProducts = await cache.GetManyAsync<ProductDto>(cacheKeys);

        // 找出缓存未命中的 ID
        var missedIds = ids
            .Where((id, index) => cachedProducts.ElementAt(index) == null)
            .ToList();

        if (missedIds.Any())
        {
            // 从数据库获取缺失的数据
            var entities = await _productRepository.GetListAsync(p => missedIds.Contains(p.Id));
            var products = entities.Select(e => new ProductDto(e.Id, e.Name, e.Price)).ToList();

            // 批量写入缓存
            var cacheItems = products.Select(p => new KeyValuePair<string, ProductDto>($"product:{p.Id}", p));
            await cache.SetManyAsync(cacheItems, TimeSpan.FromMinutes(30));
        }

        // 返回完整结果
        return cachedProducts.Where(p => p != null).ToList()!;
    }
}
```

#### GetOrSet 模式

```csharp
public class CategoryService
{
    private readonly ICachingProvider _cachingProvider;
    private readonly IRepository<Category, int> _categoryRepository;

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var cache = _cachingProvider.GetCaching("categories");

        // 使用 GetOrSet 模式
        return await cache.GetOrSetAsync("all", async () =>
        {
            var categories = await _categoryRepository.GetListAsync();
            return categories.Select(c => new CategoryDto(c.Id, c.Name, c.ParentId)).ToList();
        });
    }
}
```

#### 缓存失效

```csharp
public class ProductService
{
    private readonly ICachingProvider _cachingProvider;
    private readonly IRepository<Product, Guid> _productRepository;

    public async Task UpdateProductAsync(Guid id, UpdateProductDto input)
    {
        var product = await _productRepository.GetAsync(id);
        product.UpdateName(input.Name);
        product.UpdatePrice(input.Price);
        await _productRepository.UpdateAsync(product, autoSave: true);

        // 删除缓存
        var cache = _cachingProvider.GetCaching("products");
        await cache.RemoveAsync($"product:{id}");
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id, autoSave: true);

        // 删除缓存
        var cache = _cachingProvider.GetCaching("products");
        await cache.RemoveAsync($"product:{id}");
    }

    public async Task BatchDeleteProductsAsync(List<Guid> ids)
    {
        await _productRepository.DeleteManyAsync(ids, autoSave: true);

        // 批量删除缓存
        var cache = _cachingProvider.GetCaching("products");
        var cacheKeys = ids.Select(id => $"product:{id}").ToArray();
        await cache.RemoveManyAsync(cacheKeys);
    }
}
```

#### 使用不同的缓存实例

```csharp
public class MultiCacheService
{
    private readonly ICachingProvider _cachingProvider;

    public MultiCacheService(ICachingProvider cachingProvider)
    {
        _cachingProvider = cachingProvider;
    }

    public async Task DemoAsync()
    {
        // 产品缓存（长期缓存）
        var productCache = _cachingProvider.GetCaching("products");
        productCache.DefaultExpireTime = TimeSpan.FromHours(24);

        // 会话缓存（短期缓存）
        var sessionCache = _cachingProvider.GetCaching("sessions");
        sessionCache.DefaultExpireTime = TimeSpan.FromMinutes(30);

        // 配置缓存（永久缓存）
        var configCache = _cachingProvider.GetCaching("configs");
        configCache.DefaultExpireTime = null; // 不过期

        // 使用不同的缓存实例
        await productCache.SetAsync("product:1", new { Name = "Product 1" });
        await sessionCache.SetAsync("session:abc", new { UserId = 1 });
        await configCache.SetAsync("app:settings", new { Theme = "dark" });
    }
}
```

---

## 最佳实践

### 1. 缓存键设计

```csharp
// ✅ 好的实践：使用命名空间和有意义的键
var cacheKey = $"product:{productId}";
var cacheKey = $"user:{userId}:orders";
var cacheKey = $"category:{categoryId}:products:page:{page}";

// ❌ 不好的实践：使用无意义的键
var cacheKey = productId.ToString();
var cacheKey = "data";
```

### 2. 过期时间设置

```csharp
// ✅ 好的实践：根据数据特性设置合适的过期时间
// 频繁变化的数据：短过期时间
await cache.SetAsync("stock:123", stock, TimeSpan.FromMinutes(5));

// 相对稳定的数据：长过期时间
await cache.SetAsync("product:123", product, TimeSpan.FromHours(24));

// 配置数据：可以不设置过期时间
await cache.SetAsync("app:config", config);
```

### 3. 缓存穿透防护

```csharp
public async Task<ProductDto?> GetProductAsync(Guid id)
{
    var cache = _cachingProvider.GetCaching("products");
    var cacheKey = $"product:{id}";

    // 检查是否是空值标记
    var cached = await cache.GetAsync<CacheWrapper<ProductDto>>(cacheKey);
    if (cached != null)
    {
        return cached.Value; // 可能是 null
    }

    var entity = await _productRepository.FindAsync(id);
    var product = entity == null ? null : new ProductDto(entity.Id, entity.Name, entity.Price);

    // 即使是 null 也缓存，防止缓存穿透
    await cache.SetAsync(cacheKey, new CacheWrapper<ProductDto> { Value = product }, TimeSpan.FromMinutes(5));

    return product;
}

public class CacheWrapper<T>
{
    public T? Value { get; set; }
}
```

### 4. 缓存雪崩防护

```csharp
public async Task<ProductDto?> GetProductAsync(Guid id)
{
    var cache = _cachingProvider.GetCaching("products");
    var cacheKey = $"product:{id}";

    var product = await cache.GetAsync<ProductDto>(cacheKey);
    if (product != null)
    {
        return product;
    }

    var entity = await _productRepository.FindAsync(id);
    if (entity == null)
    {
        return null;
    }

    product = new ProductDto(entity.Id, entity.Name, entity.Price);

    // 添加随机过期时间，防止缓存雪崩
    var random = new Random();
    var expireTime = TimeSpan.FromMinutes(30 + random.Next(0, 10));
    await cache.SetAsync(cacheKey, product, expireTime);

    return product;
}
```

---

## 相关文档

- [使用指南 - 缓存](../guide/README.md#缓存)
- [API 文档 - 缓存 API](../api/README.md#缓存-api)
- [架构设计](../architecture/README.md)
```

