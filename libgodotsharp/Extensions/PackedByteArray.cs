using System.Collections;
using System.Runtime.CompilerServices;

namespace GDExtension;

public unsafe partial class PackedByteArray
{

    public byte this[int index]
    {
        get => this[(long)index];
        set => this[(long)index] = value;
    }

    public byte this[long index]
    {
        get
        {
            return Unsafe.Read<byte>(GDExtensionMain.extensionInterface.packed_byte_array_operator_index(_internal_pointer, index));
        }
        set
        {
            Unsafe.Write(GDExtensionMain.extensionInterface.packed_byte_array_operator_index(_internal_pointer, index), value);
        }
    }

    public static implicit operator PackedByteArray(List<byte> self)
    {
        var data = new PackedByteArray();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedByteArray(byte[] self)
    {
        var data = new PackedByteArray();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedByteArray(Span<byte> self)
    {
        var data = new PackedByteArray();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator List<byte>(PackedByteArray self)
    {
        var data = new List<byte>((int)self.Size());
        for (int i = 0; i < data.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator byte[](PackedByteArray self)
    {
        var data = new byte[self.Size()];
        for (int i = 0; i < self.Size(); i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator Span<byte>(PackedByteArray self)
    {
        var data = new byte[self.Size()];
        for (int i = 0; i < self.Size(); i++)
        {
            data[i] = self[i];
        }
        return data;
    }

}
