namespace Skywalker.Tokenizer;

public abstract class Either<TA, TB> : IEquatable<Either<TA, TB>> where TA : notnull where TB : notnull
{
    private Either() { }

    public static Either<TA, TB> A(TA value) => value == null ? throw new ArgumentNullException(nameof(value)) : new AImpl(value);

    public static Either<TA, TB> B(TB value) => value == null ? throw new ArgumentNullException(nameof(value)) : new BImpl(value);

    public abstract override bool Equals(object? obj);

    public abstract bool Equals(Either<TA, TB>? other);

    public abstract override int GetHashCode();

    public abstract override string? ToString();

    public abstract TResult Fold<TResult>(Func<TA, TResult> a, Func<TB, TResult> b);

    private sealed class AImpl : Either<TA, TB>
    {
        private readonly TA _value;

        public AImpl(TA value)
        {
            _value = value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object? other)
        {
            return Equals(other as AImpl);
        }

        public override bool Equals(Either<TA, TB>? obj)
        {
            return obj is AImpl a && EqualityComparer<TA>.Default.Equals(_value, a._value);
        }

        public override TResult Fold<TResult>(Func<TA, TResult> a, Func<TB, TResult> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            return a(_value);
        }

        public override string? ToString()
        {
            return _value.ToString();
        }
    }

    private sealed class BImpl : Either<TA, TB>
    {
        private readonly TB _value;

        public BImpl(TB value)
        {
            _value = value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object? other)
        {
            return Equals(other as BImpl);
        }

        public override bool Equals(Either<TA, TB>? other)
        {
            return other is BImpl b
                   && EqualityComparer<TB>.Default.Equals(_value, b._value);
        }

        public override TResult Fold<TResult>(Func<TA, TResult> a, Func<TB, TResult> b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            return b(_value);
        }

        public override string? ToString()
        {
            return _value.ToString();
        }
    }
}
