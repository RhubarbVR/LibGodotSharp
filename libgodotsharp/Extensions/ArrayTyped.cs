using System.Collections;
using System.Reflection;

namespace GDExtension;

public unsafe partial class Array<T> : Array, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
    public Array() : base()
    {
        var (type, name) = GetVariantData;
        GDExtensionMain.extensionInterface.array_set_typed(_internal_pointer, (uint)type, name._internal_pointer, new Variant()._internal_pointer);
    }

    public Array(Array array) : base(array, (long)GetVariantData.Item1, GetVariantData.Item2, new Variant())
    {
    }

    public static (Variant.Type, StringName) GetVariantData
    {
        get
        {
            if (typeof(T) == typeof(string))
            {
                return (Variant.Type.String, new StringName());
            }
            if ((typeof(T) == typeof(float)) || typeof(T) == typeof(double))
            {
                return (Variant.Type.Float, new StringName());
            }
            if ((typeof(T) == typeof(byte)) || typeof(T) == typeof(sbyte) ||
                (typeof(T) == typeof(short)) || typeof(T) == typeof(ushort) ||
                (typeof(T) == typeof(int)) || typeof(T) == typeof(uint) ||
                (typeof(T) == typeof(long)) || typeof(T) == typeof(ulong))
            {
                return (Variant.Type.Int, new StringName());
            }
            if (Enum.TryParse(typeof(T).Name, true, out Variant.Type type))
            {
                return (type, new StringName());
            }
            return (Variant.Type.Object, new StringName(typeof(T).Name));
        }
    }

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
    public static implicit operator Array<T>(List<T> self)
    {
        var data = new Array<T>();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator Array<T>(T[] self)
    {
        var data = new Array<T>();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator Array<T>(Span<T> self)
    {
        var data = new Array<T>();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator List<T>(Array<T> self)
    {
        var data = new List<T>((int)self.Size());
        for (int i = 0; i < data.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator T[](Array<T> self)
    {
        var data = new T[self.Length];
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator Span<T>(Array<T> self)
    {
        var data = new T[self.Length];
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
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
