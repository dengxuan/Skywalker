using System.Linq.Expressions;
using Skywalker.Extensions.Specifications;

namespace Skywalker.Ddd.Domain.Tests.Repositories;

public class TestSpecification : Specification<TestEntity>
{
    private readonly string? _nameFilter;

    public TestSpecification(string? nameFilter = null)
    {
        _nameFilter = nameFilter;
    }

    public override Expression<Func<TestEntity, bool>> ToExpression()
    {
        if (string.IsNullOrEmpty(_nameFilter))
        {
            return entity => true;
        }
        return entity => entity.Name.Contains(_nameFilter);
    }
}

