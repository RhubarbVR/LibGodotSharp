namespace GDExtension;

public unsafe partial struct Transform2D {

	public Transform2D(
		double xAxisX, double xAxisY,
		double yAxisX, double yAxisY,
		double originX, double originY
	) : this(
		new Vector2(xAxisX, xAxisY),
		new Vector2(yAxisX, yAxisY),
		new Vector2(originX, originY)
	) { }
}
