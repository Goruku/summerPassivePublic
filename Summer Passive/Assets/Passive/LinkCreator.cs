using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LinkCreator : MonoBehaviour {

    public Transform p1;
    public Transform p2;
    public float r = 20;
    public float linkWidth = 10;
    [Range(3,20)]
    public int resolution = 10;

    public bool invertDirection;

    public bool autoUpdate;

    public void UpdateLink() {
        GetComponent<MeshFilter>().mesh = CreateCircularMesh(p1.position, p2.position);
    }
    
    
    private Mesh CreateCircularMesh(Vector2 p1, Vector2 p2) {
        var d = p2 - p1;
        var m = (p1 + p2) * 0.5f;
        var p3Direction = invertDirection ? new Vector2(d.y, -d.x).normalized : new Vector2(-d.y, d.x).normalized;
        var p3 = m + p3Direction * (float) Math.Sqrt(r * r - (d.magnitude * d.magnitude) / 4);
        float angleStart = (float) Math.Atan2(p1.y - p3.y, p1.x - p3.x);
        float angleEnd = (float) Math.Atan2(p2.y - p3.y, p2.x - p3.x);

        float rClose = r - linkWidth * 0.5f;
        float rFar = r + linkWidth * 0.5f;
        
        Vector3[] verts = new Vector3[resolution*2];
        Vector2[] uvs = new Vector2[verts.Length];
        int[] tris = new int[2 * (resolution - 1) * 3];
        int vertIndex = 0;
        int triIndex = 0;
        
        for (int i = 0; i < resolution; i++) {
            float anglePercent = 1 / ((float) resolution-1) * i;
            uvs[vertIndex] = new Vector2(anglePercent, 1);
            uvs[vertIndex + 1] = new Vector2(anglePercent, 0);
            if (i != resolution - 1) {
                float nextAngle = Mathf.LerpAngle(angleStart, angleEnd, anglePercent);
                verts[vertIndex] = p3 + new Vector2((float) (rClose*Math.Cos(nextAngle)), (float) (rClose*Math.Sin(nextAngle)));
                verts[vertIndex + 1] = p3 + new Vector2((float) (rFar*Math.Cos(nextAngle)), (float) (rFar*Math.Sin(nextAngle)));
            }
            else {
                verts[vertIndex] = p3 + new Vector2((float) (rClose*Math.Cos(angleEnd)), (float) (rClose*Math.Sin(angleEnd)));
                verts[vertIndex + 1] = p3 + new Vector2((float) (rFar*Math.Cos(angleEnd)), (float) (rFar*Math.Sin(angleEnd)));
            }

            if (i < resolution - 1) {
                tris[triIndex] = vertIndex;
                tris[triIndex + 1] = vertIndex + 2;
                tris[triIndex + 2] = vertIndex + 1;
                
                tris[triIndex + 3] = vertIndex + 1;
                tris[triIndex + 4] = vertIndex + 2;
                tris[triIndex + 5] = vertIndex + 3;
            }

            vertIndex += 2;
            triIndex += 6;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        return mesh;
    }
}
