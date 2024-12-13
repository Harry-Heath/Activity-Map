using UnityEngine;

public static class Quad
{
	private static Mesh _quadMesh;

	public static Mesh GetMesh()
	{
		if (_quadMesh != null)
			return _quadMesh;

		_quadMesh = new Mesh { name = "Quad" };
		_quadMesh.SetVertices(new Vector3[]
		{
			new(0, 0, 0),
			new(0, 0, 1),
			new(1, 0, 0),
			new(1, 0, 1),
		});
		_quadMesh.SetUVs(0, new Vector2[]
		{
			new(0, 0),
			new(0, 1),
			new(1, 0),
			new(1, 1),
		});
		_quadMesh.SetNormals(new Vector3[]
		{
			new(0, 1, 0),
			new(0, 1, 0),
			new(0, 1, 0),
			new(0, 1, 0),
		});
		_quadMesh.SetIndices(new[] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, false);

		return _quadMesh;
	}
}
