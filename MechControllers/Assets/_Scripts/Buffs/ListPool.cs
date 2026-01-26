using System.Collections.Generic;

public static class ListPool<T>
{
    private static readonly Stack<List<T>> _pool = new();

    public static List<T> Get()
    {
        return _pool.Count > 0 ? _pool.Pop() : new List<T>(16);
    }

    public static void Release(List<T> list)
    {
        list.Clear();
        _pool.Push(list);
    }
}
