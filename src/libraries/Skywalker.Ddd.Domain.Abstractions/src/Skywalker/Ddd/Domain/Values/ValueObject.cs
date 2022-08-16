namespace Skywalker.Ddd.Domain.Values;

//Inspired from https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects

public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetAtomicValues();

    public bool ValueEquals(object? @object)
    {
        if (@object == null || @object.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)@object;

        var thisValues = GetAtomicValues().GetEnumerator();
        var otherValues = other.GetAtomicValues().GetEnumerator();

        while (thisValues.MoveNext() && otherValues.MoveNext())
        {
            if (thisValues.Current is null ^ otherValues.Current is null)
            {
                return false;
            }

            if (thisValues.Current != null &&
                !thisValues.Current.Equals(otherValues.Current))
            {
                return false;
            }
        }

        return !thisValues.MoveNext() && !otherValues.MoveNext();
    }
}
