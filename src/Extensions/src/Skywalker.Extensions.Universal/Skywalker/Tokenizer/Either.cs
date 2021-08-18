using System;
using System.Collections.Generic;

namespace Skywalker.Tokenizer
{
    public abstract class Either<TA, TB> : IEquatable<Either<TA, TB>>
    {
        private Either() { }

        public static Either<TA, TB> A(TA value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new AImpl(value);
        }

        public static Either<TA, TB> B(TB value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return new BImpl(value);
        }

        public abstract override bool Equals(object obj);
        public abstract bool Equals(Either<TA, TB> obj);
        public abstract override int GetHashCode();
        public abstract override string ToString();
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

            public override bool Equals(object obj)
            {
                return Equals(obj as AImpl);
            }

            public override bool Equals(Either<TA, TB> obj)
            {
                return obj is AImpl a
                       && EqualityComparer<TA>.Default.Equals(_value, a._value);
            }

            public override TResult Fold<TResult>(Func<TA, TResult> a, Func<TB, TResult> b)
            {
                if (a == null) throw new ArgumentNullException("a");
                if (b == null) throw new ArgumentNullException("b");
                return a(_value);
            }

            public override string ToString()
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

            public override bool Equals(object obj)
            {
                return Equals(obj as BImpl);
            }

            public override bool Equals(Either<TA, TB> obj)
            {
                return obj is BImpl b
                       && EqualityComparer<TB>.Default.Equals(_value, b._value);
            }

            public override TResult Fold<TResult>(Func<TA, TResult> a, Func<TB, TResult> b)
            {
                if (a == null) throw new ArgumentNullException("a");
                if (b == null) throw new ArgumentNullException("b");
                return b(_value);
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }
    }
}
