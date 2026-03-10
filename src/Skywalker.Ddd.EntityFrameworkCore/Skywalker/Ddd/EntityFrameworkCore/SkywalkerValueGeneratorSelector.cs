using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skywalker.Ddd.EntityFrameworkCore.ValueGeneration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public class SkywalkerValueGeneratorSelector(ValueGeneratorSelectorDependencies dependencies) : ValueGeneratorSelector(dependencies)
{
    public override bool TrySelect(IProperty property, ITypeBase typeBase, out ValueGenerator? valueGenerator)
    {
        // 只对主键属性生成值，其他属性（如 ConcurrencyStamp）不使用值生成器
        if (!property.IsPrimaryKey())
        {
            valueGenerator = null;
            return base.TrySelect(property, typeBase, out valueGenerator);
        }

        valueGenerator = property.ClrType switch
        {
            Type type when type == typeof(Guid) => new GuidIdValueGenerator(),
            Type type when type == typeof(string) => new StringIdValueGenerator(),
            _ => null
        };

        if (valueGenerator != null)
        {
            return true;
        }

        return base.TrySelect(property, typeBase, out valueGenerator);
    }
}
