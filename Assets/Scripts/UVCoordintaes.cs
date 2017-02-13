using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UVCoordintaes : MonoBehaviour {

	// Use this for initialization

	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uvs = new Vector2[vertices.Length];

		for (int i=0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
		}
		mesh.uv = uvs;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
