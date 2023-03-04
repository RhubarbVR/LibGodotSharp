using System.Runtime.CompilerServices;

namespace GDExtension;

public unsafe partial class PackedFloat32Array
{

    public float this[int index]
    {
        get => this[(long)index];
        set => this[(long)index] = value;
    }

    public float this[long index]
    {
        get
        {
            return Unsafe.Read<float>(GDExtensionMain.extensionInterface.packed_float32_array_operator_index(_internal_pointer, index));
        }
        set
        {
            Set(index, value);
        }
    }

    public static implicit operator PackedFloat32Array(List<float> self)
    {
        var data = new PackedFloat32Array();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedFloat32Array(float[] self)
    {
        var data = new PackedFloat32Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedFloat32Array(Span<float> self)
    {
        var data = new PackedFloat32Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }
}
