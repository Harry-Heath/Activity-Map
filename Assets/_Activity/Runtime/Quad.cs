using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Quad
{
	private static Dictionary<int2, Mesh> _meshes = new();

	public static Mesh GetMesh(int2 resolution)
	{
		if (_meshes.TryGetValue(resolution, out Mesh mesh)) return mesh;

		mesh = new Mesh() { name = $"Quad ({resolution.x}, {resolution.y})" };

		int vertexCount = (resolution.x + 1) * (resolution.y + 1);

		Vector3[] vertices = new Vector3[vertexCount];
		Vector3[] normals = new Vector3[vertexCount];
		Vector2[] uvs = new Vector2[vertexCount];

		// Offset by half the texture res
		float2 offset = 0.5f / (float2)(resolution + 1);

		for (int y = 0; y <= resolution.y; y++)
		{
			for (int x = 0; x <= resolution.x; x++)
			{
				int index = (resolution.x + 1) * y + x;

				normals[index] = Vector3.up;

				vertices[index] = new Vector3(
					x / (float)resolution.x, 0,
					y / (float)resolution.y
				);

				uvs[index] = new Vector2(
					x / (float)(resolution.x + 1) + offset.x, 
					y / (float)(resolution.y + 1) + offset.y
				);
			}
		}

		int[] triangles = new int[6 * resolution.x * resolution.y];

		for (int y = 0; y < resolution.y; y++)
		{
			for (int x = 0; x < resolution.x; x++)
			{
				int vertex = (resolution.x + 1) * y + x;

				int bottomLeft  = vertex;
				int bottomRight = bottomLeft + 1;
				int topLeft     = bottomRight + resolution.x;
				int topRight    = topLeft + 1;

				int quad = 6 * ((resolution.x * y) + x);

				triangles[quad + 0] = bottomLeft;
				triangles[quad + 1] = topLeft;
				triangles[quad + 2] = bottomRight;

				triangles[quad + 3] = topLeft;
				triangles[quad + 4] = topRight;
				triangles[quad + 5] = bottomRight;
			}
		}

		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uvs);
		mesh.SetNormals(normals);
		mesh.SetTriangles(triangles, 0);

		_meshes[resolution] = mesh;
		return mesh;
	}
}
