using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;

using static Unity.Mathematics.math;

public class Map : MonoBehaviour
{
	private static Map _instance;
	public static Map Instance => _instance;

	[SerializeField] private Material _entityMaterial;
	[SerializeField] private Material _chunkMaterial;
	[SerializeField] private int2 _quantity = new(5, 5);
	[SerializeField] private int2 _chunkSize = new(10, 10);
	[SerializeField] private int2 _chunkResolution = new(1024, 1024);

	private Chunk[,] _chunks;
	private CommandBuffer _cmd;
	private List<Entity> _entities = new();

	public Material EntityMaterial => _entityMaterial;
	public Material ChunkMaterial => _chunkMaterial;

	public void QueueForRender(Entity entity)
	{
		_entities.Add(entity);
	}

	public void WorldToChunk(float2 worldPosition, out int2 chunkCoord, out float2 chunkPosition)
	{
		worldPosition /= _chunkSize;
		chunkCoord = int2(floor(worldPosition));
		chunkPosition = worldPosition - chunkCoord;
	}

	public float2 ChunkToWorld(int2 chunkCoord, float2 chunkPosition)
	{
		return (chunkCoord + chunkPosition) * _chunkSize;
	}

	private void Initialise()
	{
		_instance = this;
		_cmd = new();
		_chunks = new Chunk[_quantity.x, _quantity.y];

		for (int i = 0; i < _quantity.x; i++)
		{
			for (int j = 0; j < _quantity.y; j++)
			{
				GameObject child = new();
				Chunk chunk = child.AddComponent<Chunk>();
				chunk.Initialise(this, int2(i, j), _chunkSize, _chunkResolution);
				_chunks[i, j] = chunk;
			}
		}
	}

	private void Render()
	{
		// Add entities to appropriate chunks
		foreach (Entity entity in _entities)
		{
			float2 worldPosition = float3(entity.transform.position).xz;
			WorldToChunk(worldPosition, out int2 coords, out float2 position);
			float2 mapPosition = coords + position;

			// Add to 3 by 3
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					int2 chunkCoords = coords + int2(i, j);

					// If that chunk doesn't exist, skip
					if (any(chunkCoords < 0) || any(chunkCoords >= _quantity))
						continue;

					// If this object is not close enough to the chunk, skip
					float2 chunkPosition = mapPosition - chunkCoords;
					if (any(chunkPosition < float2(-0.1)) ||
						any(chunkPosition > float2(1.1)))
						continue;

					_chunks[chunkCoords.x, chunkCoords.y].QueueForRender(entity);
				}
			}
		}

		// Render each chunk
		_cmd.Clear();
		foreach (Chunk chunk in _chunks)
		{
			chunk.Render(_cmd);
		}
		Graphics.ExecuteCommandBuffer(_cmd);
		_entities.Clear();
	}

	private void Start()
	{
		Initialise();
	}

	private void LateUpdate()
	{
		Render();
	}
}
