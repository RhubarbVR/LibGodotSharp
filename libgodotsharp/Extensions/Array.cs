using System;
using System.Collections;
using System.Reflection;

namespace GDExtension;

public unsafe partial class Array : IList, ICollection
{

    public object this[int index]
    {
        get => this[(long)index];
        set
        {
            this[(long)index] = value;
        }
    }

    public object this[long index]
    {
        get
        {
            return new Variant(GDExtensionMain.extensionInterface.array_operator_index(_internal_pointer, index));
        }
        set
        {
            var varentData = Variant.ObjectToVariant(value);
            if (varentData is null) { return; }
            Variant.SaveIntoPointer(varentData, GDExtensionMain.extensionInterface.array_operator_index(_internal_pointer, index));
        }
    }

    public bool IsFixedSize => false;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    bool IList.IsReadOnly => false;

    int ICollection.Count => (int)Size();

    public long Length => Size();

    public int Add(object value)
    {
        Append(Variant.ObjectToVariant(value));
        return (int)Size() - 1;
    }

    public bool Contains(object value)
    {
        return Has(Variant.ObjectToVariant(value));
    }

    public void CopyTo(System.Array array, int index)
    {
        var amount = System.Math.Max(Length - index, array.Length);
        for (int i = 0; i < amount; i++)
        {
            array.SetValue(this[i + index], i);
        }
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
        {
            yield return this[i];
        }
    }

    public int IndexOf(object value)
    {
        return (int)Find(Variant.ObjectToVariant(value));
    }

    public void Insert(int index, object value)
    {
        Insert((long)index, Variant.ObjectToVariant(value));
    }

    public void Remove(object value)
    {
        Erase(Variant.ObjectToVariant(value));
    }

    public void RemoveAt(int index)
    {
        RemoveAt((long)index);
    }
}
