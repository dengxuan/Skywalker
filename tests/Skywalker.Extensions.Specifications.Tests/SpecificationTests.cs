using Skywalker.Extensions.Specifications;

namespace Skywalker.Extensions.Specifications.Tests;

public class AnySpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_ShouldAlwaysReturnTrue()
    {
        var spec = new AnySpecification<int>();
        Assert.True(spec.IsSatisfiedBy(0));
        Assert.True(spec.IsSatisfiedBy(42));
        Assert.True(spec.IsSatisfiedBy(-1));
    }

    [Fact]
    public void ToExpression_ShouldReturnTrueExpression()
    {
        var spec = new AnySpecification<string>();
        var expr = spec.ToExpression();
        var func = expr.Compile();
        Assert.True(func("anything"));
        Assert.True(func(string.Empty));
    }
}

public class NoneSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_ShouldAlwaysReturnFalse()
    {
        var spec = new NoneSpecification<int>();
        Assert.False(spec.IsSatisfiedBy(0));
        Assert.False(spec.IsSatisfiedBy(42));
    }

    [Fact]
    public void ToExpression_ShouldReturnFalseExpression()
    {
        var spec = new NoneSpecification<string>();
        var expr = spec.ToExpression();
        var func = expr.Compile();
        Assert.False(func("anything"));
    }
}

public class ExpressionSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_WithMatchingPredicate_ShouldReturnTrue()
    {
        var spec = new ExpressionSpecification<int>(x => x > 10);
        Assert.True(spec.IsSatisfiedBy(20));
        Assert.False(spec.IsSatisfiedBy(5));
    }

    [Fact]
    public void ToExpression_ShouldReturnSameExpression()
    {
        System.Linq.Expressions.Expression<Func<int, bool>> expr = x => x > 0;
        var spec = new ExpressionSpecification<int>(expr);
        var result = spec.ToExpression();
        Assert.Same(expr, result);
    }
}

public class NotSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_ShouldNegateInner()
    {
        var inner = new ExpressionSpecification<int>(x => x > 10);
        var spec = new NotSpecification<int>(inner);
        Assert.True(spec.IsSatisfiedBy(5));
        Assert.False(spec.IsSatisfiedBy(20));
    }
}

public class AndSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_BothTrue_ShouldReturnTrue()
    {
        var left = new ExpressionSpecification<int>(x => x > 0);
        var right = new ExpressionSpecification<int>(x => x < 100);
        var spec = new AndSpecification<int>(left, right);
        Assert.True(spec.IsSatisfiedBy(50));
    }

    [Fact]
    public void IsSatisfiedBy_OneFalse_ShouldReturnFalse()
    {
        var left = new ExpressionSpecification<int>(x => x > 0);
        var right = new ExpressionSpecification<int>(x => x < 100);
        var spec = new AndSpecification<int>(left, right);
        Assert.False(spec.IsSatisfiedBy(-1));
        Assert.False(spec.IsSatisfiedBy(200));
    }

    [Fact]
    public void Left_And_Right_ShouldBeSet()
    {
        var left = new ExpressionSpecification<int>(x => x > 0);
        var right = new ExpressionSpecification<int>(x => x < 100);
        var spec = new AndSpecification<int>(left, right);
        Assert.Same(left, spec.Left);
        Assert.Same(right, spec.Right);
    }
}

public class OrSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_AnyTrue_ShouldReturnTrue()
    {
        var left = new ExpressionSpecification<int>(x => x < 0);
        var right = new ExpressionSpecification<int>(x => x > 100);
        var spec = new OrSpecification<int>(left, right);
        Assert.True(spec.IsSatisfiedBy(-1));
        Assert.True(spec.IsSatisfiedBy(200));
    }

    [Fact]
    public void IsSatisfiedBy_BothFalse_ShouldReturnFalse()
    {
        var left = new ExpressionSpecification<int>(x => x < 0);
        var right = new ExpressionSpecification<int>(x => x > 100);
        var spec = new OrSpecification<int>(left, right);
        Assert.False(spec.IsSatisfiedBy(50));
    }
}

public class AndNotSpecificationTests
{
    [Fact]
    public void IsSatisfiedBy_LeftTrueRightFalse_ShouldReturnTrue()
    {
        var left = new ExpressionSpecification<int>(x => x > 0);
        var right = new ExpressionSpecification<int>(x => x > 100);
        var spec = new AndNotSpecification<int>(left, right);
        Assert.True(spec.IsSatisfiedBy(50));
    }

    [Fact]
    public void IsSatisfiedBy_LeftTrueRightTrue_ShouldReturnFalse()
    {
        var left = new ExpressionSpecification<int>(x => x > 0);
        var right = new ExpressionSpecification<int>(x => x > 10);
        var spec = new AndNotSpecification<int>(left, right);
        Assert.False(spec.IsSatisfiedBy(50));
    }

    [Fact]
    public void IsSatisfiedBy_LeftFalse_ShouldReturnFalse()
    {
        var left = new ExpressionSpecification<int>(x => x > 100);
        var right = new ExpressionSpecification<int>(x => x > 10);
        var spec = new AndNotSpecification<int>(left, right);
        Assert.False(spec.IsSatisfiedBy(5));
    }
}

public class SpecificationExtensionsTests
{
    [Fact]
    public void And_ShouldCombineSpecifications()
    {
        ISpecification<int> left = new ExpressionSpecification<int>(x => x > 0);
        ISpecification<int> right = new ExpressionSpecification<int>(x => x < 100);
        var combined = left.And(right);
        Assert.True(combined.IsSatisfiedBy(50));
        Assert.False(combined.IsSatisfiedBy(-1));
    }

    [Fact]
    public void Or_ShouldCombineSpecifications()
    {
        ISpecification<int> left = new ExpressionSpecification<int>(x => x < 0);
        ISpecification<int> right = new ExpressionSpecification<int>(x => x > 100);
        var combined = left.Or(right);
        Assert.True(combined.IsSatisfiedBy(-5));
        Assert.True(combined.IsSatisfiedBy(200));
        Assert.False(combined.IsSatisfiedBy(50));
    }

    [Fact]
    public void AndNot_ShouldCombineSpecifications()
    {
        ISpecification<int> left = new ExpressionSpecification<int>(x => x > 0);
        ISpecification<int> right = new ExpressionSpecification<int>(x => x > 100);
        var combined = left.AndNot(right);
        Assert.True(combined.IsSatisfiedBy(50));
        Assert.False(combined.IsSatisfiedBy(200));
    }

    [Fact]
    public void Not_ShouldNegateSpecification()
    {
        ISpecification<int> spec = new ExpressionSpecification<int>(x => x > 0);
        var negated = spec.Not();
        Assert.True(negated.IsSatisfiedBy(-1));
        Assert.False(negated.IsSatisfiedBy(1));
    }

    [Fact]
    public void ImplicitConversion_ShouldWork()
    {
        var spec = new ExpressionSpecification<int>(x => x > 5);
        System.Linq.Expressions.Expression<Func<int, bool>> expr = spec;
        var compiled = expr.Compile();
        Assert.True(compiled(10));
        Assert.False(compiled(3));
    }

    [Fact]
    public void Specification_True_ShouldReturnTrueExpression()
    {
        var spec = new ExpressionSpecification<int>(x => x > 0);
        var trueExpr = spec.True();
        Assert.True(trueExpr.Compile()(0));
    }

    [Fact]
    public void Specification_False_ShouldReturnFalseExpression()
    {
        var spec = new ExpressionSpecification<int>(x => x > 0);
        var falseExpr = spec.False();
        Assert.False(falseExpr.Compile()(0));
    }
}
