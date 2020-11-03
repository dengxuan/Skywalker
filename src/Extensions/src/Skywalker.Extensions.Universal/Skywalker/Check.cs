using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker
{
    public static class Check
    {

        public static Type AssignableTo<TBaseType>(Type type, [NotNull] string parameterName)
        {
            if (!type.NotNull(parameterName).IsAssignableTo<TBaseType>())
            {
                throw new ArgumentException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
            }

            return type;
        }

        public static T NotNull<T>(this T argument, [NotNull] string parameterName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, $"{parameterName} should not be null.");
            }
            return argument;
        }

        public static T NotNull<T>(this T argument, [NotNull] string parameterName, string message) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }
            return argument;
        }

        public static string NotNullOrEmpty(this string argument, [NotNull] string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(argumentName, $"{argumentName} should not be null or empty.");
            }
            return argument;
        }

        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> argument, [NotNull] string argumentName)
        {
            if (argument.IsNullOrEmpty())
            {
                throw new ArgumentException(argumentName + " can not be null or empty!", argumentName);
            }

            return argument;
        }

        public static string NotNullOrWhiteSpace(this string argument, [NotNull] string argumentName)
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

        public static long Nonnegative(this long number, [NotNull] string argumentName)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName, argumentName + " should be non negative.");
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

        public static Guid NotEmptyGuid(this Guid guid, [NotNull] string argumentName)
        {
            if (Guid.Empty == guid)
            {
                throw new ArgumentException($"{argumentName} shoud be non-empty GUID.", argumentName);
            }
            return guid;
        }

        public static int Equal(this int value, int expected, [NotNull] string argumentName)
        {
            if (value != expected)
            {
                throw new ArgumentException($"{argumentName} expected value: {expected}, actual value: {value}", argumentName);
            }
            return value;
        }

        public static long Equal(this long value, long expected, [NotNull] string argumentName)
        {
            if (value != expected)
            {
                throw new ArgumentException($"{argumentName} expected value: {expected}, actual value: {value}", argumentName);
            }
            return value;
        }

        public static bool Equal(this bool value, bool expected, [NotNull] string argumentName)
        {
            if (value != expected)
            {
                throw new ArgumentException($"{argumentName} expected value: {expected}, actual value: {value}", argumentName);
            }
            return value;
        }

        public static string Length([MaybeNull] string value, [NotNull] string argumentName, int maxLength, int minLength = 0)
        {
            if (minLength > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"{argumentName} can not be null or empty!", argumentName);
                }

                if (value.Length < minLength)
                {
                    throw new ArgumentException($"{argumentName} length must be equal to or bigger than {minLength}!", argumentName);
                }
            }

            if (value != null && value.Length > maxLength)
            {
                throw new ArgumentException($"{argumentName} length must be equal to or lower than {maxLength}!", argumentName);
            }

            return value;
        }
    }
}
