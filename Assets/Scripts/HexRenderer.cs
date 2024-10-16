using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Face{
    public List<Vector3> vertices { get; private set; }
    public List<int> triangles { get; private set; }
    public List<Vector2> uvs { get; private set; } 

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs){
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<Face> faces;

    public Material material;

    public float innerSize = 0;
    public float outerSize = 1;
    public float height = 0.25f;
    public bool isFlatTopped;

    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "Hex";

        meshFilter.mesh = mesh;
        meshRenderer.material = material;
    }

    private void OnValidate() {
        if (Application.isPlaying && mesh != null)
        {   
            DrawMesh();
        }
    }

    private void OnEnable() {
        DrawMesh();
    }

    public void SetMaterial(Material newMaterial){
        meshRenderer.material = newMaterial;
    }

    public void DrawMesh(){
        DrawFaces();
        CombineFaces();
    }

    public void DrawFaces(){
        faces = new List<Face>();

        //Top faces
        for(int point = 0; point < 6; point++){
            faces.Add(CreateFace(innerSize, outerSize, height / 2f, height / 2f, point));
        }

        //Bottom faces
        for(int point = 0; point < 6; point++){
            faces.Add(CreateFace(innerSize, outerSize, -height / 2f, -height / 2f, point, true));
        }

        //Outer faces
        for(int point = 0; point < 6; point++){
            faces.Add(CreateFace(outerSize, outerSize, height / 2f, -height / 2f, point, true));
        }

        //Inner faces
        for(int point = 0; point < 6; point++){
            faces.Add(CreateFace(innerSize, innerSize, height / 2f, -height / 2f, point, false));
        }
    }

    public void CombineFaces(){
        List<Vector3> vertices = new();
        List<int> tris = new();
        List<Vector2> uvs = new();

        for(int i = 0; i < faces.Count; i++){
            vertices.AddRange(faces[i].vertices);
            uvs.AddRange(faces[i].uvs);

            int offset = 4 * i;

            foreach (int triangle in faces[i].triangles)
            {
                tris.Add(triangle + offset);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    private Face CreateFace(float innerRadius, float outerRadius, float heighA, float heightB, int point, bool reverse = false){
        Vector3 pointA = GetPoint(innerRadius, heightB, point);
        Vector3 pointB = GetPoint(innerRadius, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRadius, heighA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRadius, heighA, point);

        List<Vector3> vertices = new List<Vector3> { pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int> { 0, 1, 2, 2, 3, 0};
        List<Vector2> uvs = new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};
        
        if(reverse){
            vertices.Reverse();
        }

        return new Face(vertices, triangles, uvs);
    }

    protected Vector3 GetPoint(float size, float height, int index){
        float angleDeg = isFlatTopped ? 60 * index : 60 * index - 30;
        float angleRad = Mathf.PI / 180f * angleDeg;
        return new Vector3(size * Mathf.Cos(angleRad), height, size * MathF.Sin(angleRad));
    }
}
