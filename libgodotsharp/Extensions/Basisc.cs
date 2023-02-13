namespace GDExtension;

public unsafe partial struct Basis {

	public Basis(
		double xAxisX, double xAxisY, double xAxisZ,
		double yAxisX, double yAxisY, double yAxisZ,
		double zAxisX, double zAxisY, double zAxisZ
	) : this(
		new Vector3(xAxisX, xAxisY, xAxisZ),
		new Vector3(yAxisX, yAxisY, yAxisZ),
		new Vector3(zAxisX, zAxisY, zAxisZ)
	) { }
}
