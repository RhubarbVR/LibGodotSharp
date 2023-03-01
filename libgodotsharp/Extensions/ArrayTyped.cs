using System.Collections;
using System.Reflection;

namespace GDExtension;

public unsafe partial class Array<T> : Array, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
    public new T this[int index]
    {
        get
        {
            return (T)base[index];
        }
        set
        {
            base[index] = value;
        }
    }

    public new T this[long index]
    {
        get
        {
            return (T)base[index];
        }
        set
        {
            base[index] = value;
        }
    }

    int ICollection<T>.Count => (int)Size();

    bool ICollection<T>.IsReadOnly => false;

    public void Add(T item)
    {
        Append(Variant.ObjectToVariant(item));
    }

    public bool Contains(T item)
    {
        return Has(Variant.ObjectToVariant(item));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        var amount = System.Math.Max(Length - arrayIndex, array.Length);
        for (int i = 0; i < amount; i++)
        {
            array[i] = this[i + arrayIndex];
        }
    }

    public int IndexOf(T item)
    {
        return (int)Find(Variant.ObjectToVariant(item));
    }

    public void Insert(int index, T item)
    {
        Insert((long)index, Variant.ObjectToVariant(item));
    }

    public bool Remove(T item)
    {
        Erase(Variant.ObjectToVariant(item));
        return true;
    }

    public new IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
        {
            yield return this[i];
        }
    }
}
