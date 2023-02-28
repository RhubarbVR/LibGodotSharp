using LibGodotSharp;
using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace GDExtension;

public unsafe partial class Callable
{
    protected Callable(Delegate @delegate,Object @object)
    {
        if (@delegate is null)
        {
            throw new NullReferenceException();
        }
        _delegate = @delegate;
        _object = @object;

        int fake;
        lock (callables)
        {
            fake = 0;
            foreach (var call in callables)
            {
                if (call.Key == fake)
                {
                    fake++;
                }
                else
                {
                    break;
                }
            }
            callables.Add(fake, this);
        }
        _internal_pointer = LibGodotCustomCallable.Libgodot_create_callable((void*)fake);
    }
    internal static Dictionary<int, Callable> callables = new();

    internal readonly Delegate _delegate;
    internal readonly Object _object;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Callable From<T>(T target, Object @object = null) where T : Delegate
    {
        if(target is null)
        {
            throw new NullReferenceException();
        }
        if(target.Target is Object delobject)
        {
            @object = delobject;
        }
        if(@object is null)
        {
            throw new NullReferenceException();
        }
        return new Callable(target, @object);
    }
}