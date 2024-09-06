using System.Collections;
using System.Reflection;

public class DeepComparer<T> : IEqualityComparer<T>
{
    /// <summary>
    /// Equals method to perform deep comparision
    /// </summary>
    public bool Equals(T? expected, T? actual)
    {
        return Compare(expected, actual);
    }
    /// <summary>
    /// Comparing datatypes of type IEnumerable
    /// </summary>
    private bool CompareIEnumerable(IEnumerable expectedEnumerable, IEnumerable actualEnumerable)
    {
        var expectedValue = expectedEnumerable.GetEnumerator();
        var actualValue = actualEnumerable.GetEnumerator();
        Dictionary<Object, int> map = [];

        int expectedCount = 0;
        int actualCount = 0;
        while (expectedValue.MoveNext())
        {
            if (!map.ContainsKey(expectedValue.Current))
                map[expectedValue.Current] = 1;
            else
            {
                map[expectedValue.Current] = ++map[expectedValue.Current];
            }
            expectedCount++;
        }
        while (actualValue.MoveNext())
        {
            if (!map.ContainsKey(actualValue.Current))
                return false;

            if (map[actualValue.Current] == 0)
                return false;

            map[actualValue.Current] = --map[actualValue.Current];
            actualCount++;
        }
        if (expectedCount != actualCount)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// Comparing primitives,nested objects and IEnumerable datatypes
    /// </summary>
    private bool Compare(Object? expected, Object? actual)
    {
        if (expected == null && actual == null)
        {
            return true;
        }
        if (expected == null || actual == null)
        {
            return false;
        }
        if (!expected.GetType().Equals(actual.GetType()))
        {
            return false;
        }
        Type type = expected.GetType();
        //compares primitive datatypes
        if (type.IsPrimitive || typeof(string).Equals(type) || typeof(DateTime).Equals(type))
        {
            return expected.Equals(actual);
        }
        //compare IEnumerable datatype
        else if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            IEnumerable? expectedEnumerable = expected as IEnumerable;
            IEnumerable? actualEnumerable = actual as IEnumerable;
            if (expectedEnumerable == null && actualEnumerable == null)
            {
                return true;
            }
            if (expectedEnumerable == null || actualEnumerable == null)
            {
                return false;
            }
            return CompareIEnumerable(expectedEnumerable, actualEnumerable);
        }
        //compares 2 Objects
        else
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var prop in props)
            {
                var expectedValue = prop.GetValue(expected, null);
                var actualValue = prop.GetValue(actual, null);
                if (!Compare(expectedValue, actualValue))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetHashCode(T parameterValue)
    {
        return Tuple.Create(parameterValue).GetHashCode();
    }
}