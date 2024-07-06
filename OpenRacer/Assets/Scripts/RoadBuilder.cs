using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode()]
public class RoadBuilder : MonoBehaviour
{
    [SerializeField]
    private SplineContainer _splineContainer;

    [SerializeField]
    private int _splineIndex;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float _width= 0.1f;
    [SerializeField]
    [Range (1f, 500f)]
    private float resolution = 300;

    [SerializeField]
    private Material _material;

    float3 position;
    float3 forward;
    float3 upVector;

    float3 p1;
    float3 p2;


    List<Vector3> _vertsP1;
    List<Vector3> _vertsP2;

    void Start()
    {
        _splineContainer = gameObject.GetComponent<SplineContainer>();
        if (GetComponent<MeshCollider>() == null) gameObject.AddComponent<MeshCollider>();
        if (GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();
        if (GetComponent<Rigidbody>() == null) gameObject.AddComponent<Rigidbody>();

        // set Material 
        GetComponent<MeshRenderer>().sharedMaterial = _material;

        //set physics for Road
        GetComponent<Rigidbody>().isKinematic= true;

    }

    public void setPath(List<Vector3> points, float scale= 1f)
    {
        _splineContainer.Spline.Clear();    
        for (int i = 0; i < points.Count; i++)
            _splineContainer.Spline.Add(new BezierKnot(points[i]));
        _splineContainer.Spline.Closed = true;
        //_splineContainer.Spline.SetTangentMode(new SplineRange(0, _splineContainer.Spline.Count), TangentMode.AutoSmooth);
        getVerts();
        buildMesh();
        GetComponent<Transform>().localScale = new Vector3(scale, 1, scale);
    }

    private void getVerts()
    {
        _vertsP1 = new List<Vector3>(); 
        _vertsP2 = new List<Vector3>();

        float steps = 1f / (float)resolution;

        for(int  i = 0; i <= resolution; i++) {
            float t = steps * i+0.0000001f;
            _splineContainer.Evaluate(_splineIndex, t, out position, out forward, out upVector);

            float3 right = Vector3.Cross(forward, upVector).normalized;
            p1 = position + (right * _width);
            p2 = position + (-right * _width);

            _vertsP1 .Add(p1);
            _vertsP2 .Add(p2);
        }
    }

    public List<Vector3> getCenterLine()
    {
        List<Vector3> centerLine = new List<Vector3>();
        for (int i = 0; i < _splineContainer.Spline.Count; i++)
            centerLine.Add(_splineContainer.Spline[i].Position);
        return centerLine;
    }

    private void buildMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> UVs = new List<Vector2>();
        int offset = 0;
        float uvOffset = 0, uvDistance = 0; 

        int length = _vertsP2.Count;

        for(int i = 1;i < length;i++)
        {
            Vector3 p1 = _vertsP1[i-1];
            Vector3 p2 = _vertsP2[i-1];
            Vector3 p3, p4;

            if(i  == length)
            {
                p3 = _vertsP1[0];
                p4 = _vertsP2[0];
            }
            else
            {
                p3 = _vertsP1[i];
                p4 = _vertsP2[i];
            }
            offset = 4 * (i - 1);

            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;

            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            vertices.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> {  t1, t2, t3, t4, t5, t6 });

            float distance = Vector3.Distance(p1, p3)/ 4f;
            uvDistance = uvOffset + distance;
            UVs.AddRange(new List<Vector2> { new Vector2(uvOffset,0), new Vector2(uvOffset,1), new Vector2(uvDistance,0), new Vector2(uvDistance,1)});
            uvOffset += distance; 
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, UVs);

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

    }

}
