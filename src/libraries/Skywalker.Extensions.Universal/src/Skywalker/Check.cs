using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Skywalker
{
    [DebuggerStepThrough]
    public static class Check
    {

        public static T Condition<T>(T value, Predicate<T> condition, string parameterName)
        {
            NotNull(condition, nameof(condition));

            if (!condition(value))
            {
                NotNullOrEmpty(parameterName, nameof(parameterName));

                throw new ArgumentOutOfRangeException(parameterName);
            }

            return value;
        }

        public static IList<T> HasNoNulls<T>(IList<T> value, string parameterName)
            where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotNullOrEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            return value;
        }

        public static Type AssignableTo<TBaseType>(Type type, string parameterName)
        {
            if (!type.NotNull(parameterName).IsAssignableTo<TBaseType>())
            {
                throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
            }

            return type;
        }

        public static T NotNull<T>(this T argument, string parameterName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, $"{parameterName} should not be null.");
            }
            return argument;
        }

        public static T NotNull<T>(this T argument, string parameterName, string message) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }
            return argument;
        }

        public static string NotNullOrEmpty(this string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(argumentName, $"{argumentName} should not be null or empty.");
            }
            return argument;
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> argument, string argumentName)
        {
            if (argument.IsNullOrEmpty())
            {
                throw new ArgumentException(argumentName + " can not be null or empty!", argumentName);
            }

            return argument;
        }

        public static string NotNullOrWhiteSpace(this string argument, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException(argumentName, $"{argumentName} should not be null or empty.");
            }
            return argument;
        }

        public static int Positive(this int number, string argumentName)
        {
            if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, $"{argumentName} should be positive.");
            }
            return number;
        }

        public static long Positive(this long number, string argumentName)
        {
            if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, $"{argumentName} should be positive.");
            }
            return number;
        }

        public static decimal Positive(this decimal number, string argumentName)
        {
            if (number <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, $"{argumentName} should be positive.");
            }
            return number;
        }

        public static int Nonnegative(this int number, string argumentName)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be non negative.");
            }
            return number;
        }

        public static long Nonnegative(this long number, string argumentName)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be non negative.");
            }
            return number;
        }

        public static decimal Nonnegative(this decimal number, string argumentName)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be non negative.");
            }
            return number;
        }

        public static Guid NotEmptyGuid(this Guid guid, string argumentName)
        {
            if (Guid.Empty == guid)
            {
                throw new ArgumentException($"{argumentName} shoud be non-empty GUID.", argumentName);
            }
            return guid;
        }

        public static int Equal(this int value, int expected, string argumentName)
        {
            if (value != expected)
            {
                throw new ArgumentException($"{argumentName} expected value: {expected}, actual value: {value}", argumentName);
            }
            return value;
        }

        public static long Equal(this long value, long expected, string argumentName)
        {
            if (value != expected)
            {
                throw new ArgumentException($"{argumentName} expected value: {expected}, actual value: {value}", argumentName);
            }
            return value;
        }

        public static bool Equal(this bool value, bool expected, string argumentName)
        {
            if (value != expected)
            {
                throw new ArgumentException($"{argumentName} expected value: {expected}, actual value: {value}", argumentName);
            }
            return value;
        }

        public static string LengthOf(this string value, string argumentName, int maxLength, int minLength = 0)
        {
            if (minLength > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException($"{argumentName} can not be null or empty!", argumentName);
                }

                if (value.Length < minLength)
                {
                    throw new ArgumentOutOfRangeException($"{argumentName} length must be equal to or bigger than {minLength}!", argumentName);
                }
            }

            if (value != null && value.Length > maxLength)
            {
                throw new ArgumentOutOfRangeException($"{argumentName} length must be equal to or lower than {maxLength}!", argumentName);
            }

            return value!;
        }
    }
}
