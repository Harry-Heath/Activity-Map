using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	private Rigidbody _rb;
	private Mesh _mesh;
	private Vector4 _colour;

	public Mesh Mesh => _mesh;
	public Matrix4x4 Matrix => transform.localToWorldMatrix;
	public Vector4 Colour => _colour;

	private void OnEnable()
	{
		_rb = GetComponent<Rigidbody>();
		_mesh = GetComponent<MeshFilter>().sharedMesh;
	}

	private void Update()
	{
		Map.Instance.QueueForRender(this);
		_colour = new(_rb.velocity.x, _rb.velocity.z, 0, 0);
	}
}
