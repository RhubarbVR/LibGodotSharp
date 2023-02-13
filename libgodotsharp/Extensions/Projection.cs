namespace GDExtension;

public unsafe partial struct Projection {

	public Projection(
		double xAxisX, double xAxisY, double xAxisZ, double xAxisW,
		double yAxisX, double yAxisY, double yAxisZ, double yAxisW,
		double zAxisX, double zAxisY, double zAxisZ, double zAxisW,
		double wAxisX, double wAxisY, double wAxisZ, double wAxisW
	) : this(
		new Vector4(xAxisX, xAxisY, xAxisZ, xAxisW),
		new Vector4(yAxisX, yAxisY, yAxisZ, yAxisW),
		new Vector4(zAxisX, zAxisY, zAxisZ, zAxisW),
		new Vector4(wAxisX, wAxisY, wAxisZ, wAxisW)
	) { }
}
