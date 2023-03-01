namespace GDExtension;

public unsafe partial class ArrayMesh
{
    public T SurfaceGetMaterial<T>(long surf_idx = 0) where T: Material
    {
        return (T)SurfaceGetMaterial(surf_idx);
    }
}

