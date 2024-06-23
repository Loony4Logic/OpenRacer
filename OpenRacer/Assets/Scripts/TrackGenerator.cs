using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField]
    [Range(1f, 50f)]
    float _scale = 15f;

    public List<Vector3> centerLine;
    public List<Vector3> endLineRight, endLineLeft;

    RoadBuilder builder;

    public void generate(List<Vector3> verts)
    {
        builder = GetComponent<RoadBuilder>();
        builder.setPath(verts, _scale);
        Mesh trackMesh = GetComponent<MeshFilter>().mesh;
        Vector3 localScale = gameObject.transform.localScale;
        for (int i = 0; i < trackMesh.vertices.Length; i+=4)
        {
            endLineRight.Add(Vector3.Scale(localScale, trackMesh.vertices[i]));
            endLineLeft.Add(Vector3.Scale(localScale, trackMesh.vertices[i+1]));
            centerLine.Add((endLineLeft.Last()+ endLineRight.Last())*0.5f);
        }
        // TODO: use edge line for creating curbs   
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<CarControl>().all_wheels_on_track == false)
        {
            other.GetComponentInParent<CarControl>().all_wheels_on_track = true;
            return;
        }
        other.GetComponentInParent<CarControl>().all_wheels_on_track = false;
        other.GetComponentInParent<CarControl>().resetPosition();
    }
}