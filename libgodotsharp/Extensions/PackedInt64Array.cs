using System.Runtime.CompilerServices;

namespace GDExtension;

public unsafe partial class PackedInt64Array
{

    public long this[int index]
    {
        get => this[(long)index];
        set => this[(long)index] = value;
    }

    public long this[long index]
    {
        get
        {
            return Unsafe.Read<long>(GDExtensionMain.extensionInterface.packed_int64_array_operator_index(_internal_pointer, index));
        }
        set
        {
            Unsafe.Write(GDExtensionMain.extensionInterface.packed_int64_array_operator_index(_internal_pointer, index), value);
        }
    }

    public static implicit operator PackedInt64Array(List<long> self)
    {
        var data = new PackedInt64Array();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedInt64Array(long[] self)
    {
        var data = new PackedInt64Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedInt64Array(Span<long> self)
    {
        var data = new PackedInt64Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }
}
