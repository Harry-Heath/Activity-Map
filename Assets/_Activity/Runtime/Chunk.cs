using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
	private Map _map;
	private RenderTexture _renderTexture;
	private Matrix4x4 _proj;
	private Matrix4x4 _view;
	private MaterialPropertyBlock _properties;
	private ComputeBuffer _buf;
	private readonly Dictionary<Mesh, Instances> _instances = new();
	private bool _shouldRender = false;

	public void Initialise(Map map, int2 coords, int2 size, int2 resolution)
	{
		_map = map;
		_renderTexture = new(resolution.x + 1, resolution.y + 1, 16);

		// Setup game object
		float2 pos = coords * size;
		transform.position = new(pos.x, 0, pos.y);
		transform.localScale = new(size.x, 1, size.y);
		transform.parent = map.transform;
		gameObject.name = $"Chunk ({coords.x}, {coords.y})";

		// Create matrices
		float2 scale = size * ((resolution + 1) / (float2)resolution); // Transform size from cell to pixels
		float2 offset = pos - (0.5f * (size / (float2)resolution));    // Offset position by half a cell
		_view = Matrix4x4.TRS(new(offset.x, 0, offset.y), Quaternion.Euler(90, 0, 0), Vector3.one).inverse;
		_proj = Matrix4x4.Ortho(0, scale.x, 0, scale.y, -100, 100);

		// Add quad
		var meshFilter = gameObject.AddComponent<MeshFilter>();
		var meshCollider = gameObject.AddComponent<MeshCollider>();
		var meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = Quad.GetMesh(resolution);
		meshCollider.sharedMesh = Quad.GetMesh(resolution);
		meshRenderer.sharedMaterial = map.ChunkMaterial;
		
		// Set texture
		MaterialPropertyBlock block = new();
		block.SetTexture("_BaseMap", _renderTexture);
		meshRenderer.SetPropertyBlock(block);

		// Setup properties
		_buf = new ComputeBuffer(Instances.Max, 2 * sizeof(float));
		_properties = new();
		_properties.SetBuffer("_Colours", _buf);
	}

	public void QueueForRender(Entity entity)
	{
		if (!_instances.TryGetValue(entity.Mesh, out Instances instances))
		{
			instances = new();
			_instances[entity.Mesh] = instances;
		}

		instances.Add(entity.Matrix, entity.Colour);
		_shouldRender = true;
	}

	public void Render(CommandBuffer cmd)
	{
		if (!_shouldRender) return;

		cmd.SetRenderTarget(_renderTexture);
		cmd.ClearRenderTarget(true, false, Color.clear);
		cmd.SetViewMatrix(_view);
		cmd.SetProjectionMatrix(_proj);

		foreach ((Mesh mesh, Instances instances) in _instances)
		{
			if (instances.Count == 0) continue;

			cmd.SetBufferData(_buf, instances.Colours, 0, 0, instances.Count);
			cmd.DrawMeshInstanced(mesh, 0, _map.EntityMaterial, 0, instances.Transforms, instances.Count, _properties);

			instances.Clear();
		}

		_shouldRender = false;
	}

	private void OnDestroy()
	{
		_renderTexture.Release();
		_buf.Release();
	}
}

class Instances
{
	public const int Max = 1023;

	private Matrix4x4[] _transforms = new Matrix4x4[Max];
	private Vector2[] _colours = new Vector2[Max];
	private int _index = 0;

	public int Count => _index;
	public Matrix4x4[] Transforms => _transforms;
	public Vector2[] Colours => _colours;

	public void Add(Matrix4x4 transform, Vector2 colour)
	{
		if (_index == Max) return;

		_transforms[_index] = transform;
		_colours[_index] = colour;
		_index++;
	}

	public void Clear()
	{
		_index = 0;
	}
}