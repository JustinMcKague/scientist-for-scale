using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Util
{
    public static T Next<T> (this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] array = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(array, src) + 1;
        return (array.Length < j ? array[0] : array[j]);
    }

    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] array = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(array, src) - 1;
        return (j < 0 ? array[array.Length] : array[j]);
    }
}

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}

