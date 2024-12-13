using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	private Rigidbody _rb;
	private Mesh _mesh;
	private MaterialPropertyBlock _properties;

	public Mesh Mesh => _mesh;
	public Matrix4x4 Matrix => transform.localToWorldMatrix;
	public MaterialPropertyBlock Properties => _properties;

	private void OnEnable()
	{
		_rb = GetComponent<Rigidbody>();
		_mesh = GetComponent<MeshFilter>().sharedMesh;
		_properties = new();
	}

	private void Update()
	{
		Map.Instance.QueueForRender(this);
		Color colour = new(_rb.velocity.x, _rb.velocity.z, 0, 0);
		_properties.SetColor("_Color", colour);
	}
}
