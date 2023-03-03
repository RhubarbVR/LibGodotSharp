namespace GDExtension;

public unsafe partial class PackedStringArray
{

    public string this[int index]
    {
        get => this[(long)index];
        set => this[(long)index] = value;
    }

    public string this[long index]
    {
        get
        {
            var res = (void**)GDExtensionMain.extensionInterface.packed_string_array_operator_index(_internal_pointer, index);
            return StringMarshall.ToManaged(*res);
        }
        set
        {
            var res = (void**)GDExtensionMain.extensionInterface.packed_string_array_operator_index(_internal_pointer, index);
            res[0] = StringMarshall.ToNative(value);
        }
    }

    public static implicit operator PackedStringArray(List<string> self)
    {
        var data = new PackedStringArray();
        data.Resize(self.Count);
        for (int i = 0; i < self.Count; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedStringArray(string[] self)
    {
        var data = new PackedStringArray();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }

    public static implicit operator PackedStringArray(Span<string> self)
    {
        var data = new PackedStringArray();
        data.Resize(self.Length);
        for (int i = 0; i < self.Length; i++)
        {
            data[i] = self[i];
        }
        return data;
    }
}
