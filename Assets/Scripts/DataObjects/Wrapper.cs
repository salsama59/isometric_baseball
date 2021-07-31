using System;

[Serializable]
public class Wrapper<T>
{
    public T[] items = null;

    public Wrapper() { }

    public Wrapper(T[] items)
    {
        this.items = items;
    }
}