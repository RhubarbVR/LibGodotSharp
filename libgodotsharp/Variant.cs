using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace GDExtension;

public sealed unsafe partial class Variant
{

    struct Constructor
    {
        public Native.GDExtensionVariantFromTypeConstructorFunc fromType;
        public Native.GDExtensionTypeFromVariantConstructorFunc toType;
    }

    static readonly Constructor[] constructors = new Constructor[(int)Type.Max];

    public static void Register()
    {
        for (var i = 1; i < (int)Type.Max; i++)
        {
            constructors[i] = new Constructor()
            {
                fromType = GDExtensionMain.extensionInterface.get_variant_from_type_constructor((Native.GDExtensionVariantType)i),
                toType = GDExtensionMain.extensionInterface.get_variant_to_type_constructor((Native.GDExtensionVariantType)i),
            };
        }
    }

    public static void SaveIntoPointer(object value, void* ptr)
    {
        var valCast = ObjectToVariant(value);
        if (valCast is null)
        {
            return;
        }
        SaveIntoPointer(valCast, ptr);
    }

    public static void SaveIntoPointer(Variant value, void* ptr)
    {
        var srcSpan = new Span<byte>(value._internal_pointer, 24);
        var dstSpan = new Span<byte>(ptr, 24);
        srcSpan.CopyTo(dstSpan);
    }

    public static void SaveIntoPointer(Object value, void* ptr)
    {
        var objectPtr = value != null ? value._internal_pointer : null;
        constructors[(int)Variant.Type.Object].fromType(ptr, &objectPtr);
    }
    public static Object GetObjectFromVariant(Variant _object)
    {
        void* res;
        constructors[(int)Type.Object].toType(&res, _object._internal_pointer);
        return Object.ConstructUnknown(res);
    }

    public static Object GetObjectFromPointer(void* ptr)
    {
        void* res;
        constructors[(int)Type.Object].toType(&res, ptr);
        return Object.ConstructUnknown(res);
    }

    internal void* _internal_pointer;

    public Type NativeType => (Type)GDExtensionMain.extensionInterface.variant_get_type(_internal_pointer);

    internal Variant()
    {
        _internal_pointer = GDExtensionMain.extensionInterface.mem_alloc(24);
        byte* dataPointer = (byte*)_internal_pointer;
        for (int i = 0; i < 24; i++)
        {
            dataPointer[i] = 0;
        }
    }

    internal Variant(void* data)
    {
        _internal_pointer = data;
        GC.SuppressFinalize(this);
    }

    public static Variant Nil
    {
        get { var v = new Variant(); GDExtensionMain.extensionInterface.variant_new_nil(v._internal_pointer); return v; }
    }

    public Variant(int value) : this((long)value) { }
    public Variant(float value) : this((double)value) { }

    ~Variant()
    {
        GDExtensionMain.extensionInterface.variant_destroy(_internal_pointer);
        GDExtensionMain.extensionInterface.mem_free(_internal_pointer);
    }
}
