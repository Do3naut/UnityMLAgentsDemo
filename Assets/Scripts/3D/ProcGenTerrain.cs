using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ProcGenTerrain : MonoBehaviour
{
    Mesh mesh;
    public int xSize = 20;
    public int zSize = 20;
    Vector3[] vertices;
    int[] triangles;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];


        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }


        int vert = 0;
        int triIndex = 0;
        triangles = new int[xSize * zSize * 6];
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                triangles[triIndex] = vert + 0;
                triangles[triIndex + 1] = vert + xSize + 1;
                triangles[triIndex + 2] = vert + 1;
                triangles[triIndex + 3] = vert + 1;
                triangles[triIndex + 4] = vert + xSize + 1;
                triangles[triIndex + 5] = vert + xSize + 2;

                vert++;
                triIndex += 6;
            }
            vert++;
        }

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null)
    //        return;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], .1f);
    //    }
    //}
}