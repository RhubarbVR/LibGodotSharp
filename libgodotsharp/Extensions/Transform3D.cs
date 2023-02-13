namespace GDExtension;

public unsafe partial struct Transform3D {

	public Transform3D(
		double xAxisX, double xAxisY, double xAxisZ,
		double yAxisX, double yAxisY, double yAxisZ,
		double zAxisX, double zAxisY, double zAxisZ,
		double originX, double originY, double originZ
	) : this(
		new Vector3(xAxisX, xAxisY, xAxisZ),
		new Vector3(yAxisX, yAxisY, yAxisZ),
		new Vector3(zAxisX, zAxisY, zAxisZ),
		new Vector3(originX, originY, originZ)
	) { }
}
