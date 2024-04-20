using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Utils
{
    public static class Guard
    {
        public static void NotDefault<T>(T parameter, string parameterName, string message = default)
        {
            if (Equals(default(T), parameter))
                throw new InvalidOperationException($"Parameter : {parameterName} has default value : { (string.IsNullOrWhiteSpace(message) ? string.Empty : message) }");
        }

        public static void NotNull(object parameter, string parameterName, string message = default)
        {
            if (Equals(null, parameter)) throw new ArgumentNullException(parameterName, message ?? string.Empty);
        }
        public static void CollectionNotNullAndNotEmpty<T>(ICollection<T> collection, string argumentName, string message = default)
        {
            NotNull(collection, nameof(collection));

            if (collection.Count == 0) throw new ArgumentNullException($"Collection {argumentName} is empty", message ?? string.Empty);
        }

        public static void CollectionMustBeNullAndEmpty<T>(ICollection<T> collection, string argumentName, string message = default)
        {
            if (Equals(null, collection) || collection.Count == 0) return;

            throw new ArgumentNullException($"Collection {argumentName} is not empty", message ?? string.Empty);
        }

        public static void CollectionHasExactlyNumberElements<T>(ICollection<T> collection, int numElements, string argumentName, string message = default)
        {
            CollectionNotNullAndNotEmpty(collection, nameof(collection));

            if (collection.Count != numElements) throw new ArgumentNullException($"Collection {argumentName} has not exactly {numElements} elements", message ?? string.Empty);
        }

        public static void CollectionHasOneElement<T>(ICollection<T> collection, string argumentName, string message = default)
        {
            CollectionNotNullAndNotEmpty(collection, argumentName, message);

            if (collection.Count != 1) throw new ArgumentNullException($"Collection {argumentName} does not have one element", message ?? string.Empty);
        }
                
        public static void StringNotNullOrEmpty(string s, string parameterName, string message = default)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(parameterName, message ?? string.Empty);
        }

        public static void StringNotNullOrWhiteSpace(string s, string parameterName, string message = default)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException(parameterName, message ?? string.Empty);
        }

        public static void NumberInRange<T>(T number, T min, T max, string argumentName) where T : IComparable<T>
        {

            if (number.CompareTo(min) < 0 || number.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(argumentName);
        }

        public static void NumberMin<T>(T number, T min, string argumentName) where T : IComparable<T>
        {
            if (number.CompareTo(min) < 0) throw new ArgumentOutOfRangeException(argumentName);
        }
        public static void NumberMax<T>(T number, T max, string argumentName) where T : IComparable<T>
        {
            if (number.CompareTo(max) > 0) throw new ArgumentOutOfRangeException(argumentName);
        }
    }

}
