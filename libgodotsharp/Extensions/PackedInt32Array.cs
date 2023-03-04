using System.Runtime.CompilerServices;

namespace GDExtension;

public unsafe partial class PackedInt32Array
{

    public int this[int index]
    {
        get => this[(long)index];
        set => this[(long)index] = value;
    }

    public int this[long index]
    {
        get
        {
            return Unsafe.Read<int>(GDExtensionMain.extensionInterface.packed_int32_array_operator_index(_internal_pointer, index));
        }
        set
        {
            Set(index, value);
        }
    }

    public static implicit operator PackedInt32Array(List<int> self)
    {
        var data = new PackedInt32Array();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedInt32Array(int[] self)
    {
        var data = new PackedInt32Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedInt32Array(Span<int> self)
    {
        var data = new PackedInt32Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }
}
