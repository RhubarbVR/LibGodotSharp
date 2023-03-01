using System.Runtime.CompilerServices;

namespace GDExtension;

public unsafe partial class PackedFloat64Array
{

    public double this[int index]
    {
        get => this[(long)index];
        set => this[(long)index] = value;
    }

    public double this[long index]
    {
        get
        {
            return Unsafe.Read<float>(GDExtensionMain.extensionInterface.packed_float64_array_operator_index(_internal_pointer, index));
        }
        set
        {
            Unsafe.Write(GDExtensionMain.extensionInterface.packed_float64_array_operator_index(_internal_pointer, index), value);
        }
    }

    public static implicit operator PackedFloat64Array(List<double> self)
    {
        var data = new PackedFloat64Array();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedFloat64Array(double[] self)
    {
        var data = new PackedFloat64Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedFloat64Array(Span<double> self)
    {
        var data = new PackedFloat64Array();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }
}
