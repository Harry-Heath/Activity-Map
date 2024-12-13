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
	private readonly List<Entity> _entities = new();

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
	}

	public void QueueForRender(Entity entity)
	{
		_entities.Add(entity);
	}

	public void Render(CommandBuffer cmd)
	{
		if (_entities.Count == 0) return;

		cmd.SetRenderTarget(_renderTexture);
		cmd.ClearRenderTarget(true, false, Color.clear);
		cmd.SetViewMatrix(_view);
		cmd.SetProjectionMatrix(_proj);

		foreach (Entity entity in _entities)
		{
			cmd.DrawMesh(entity.Mesh, entity.Matrix, _map.EntityMaterial, 0, 0, entity.Properties);
		}

		_entities.Clear();
	}

	private void OnDestroy()
	{
		_renderTexture.Release();
	}
}
