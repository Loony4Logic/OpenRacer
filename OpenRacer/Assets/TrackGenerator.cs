using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    /* REF
    Vector3[] verts = new Vector3[]
{
    new Vector3(0, 0, 0), // 0
    new Vector3(1, 0, 0), // 1
    new Vector3(0, 1, 0), // 2
    new Vector3(0, 0, 1), // 3
    new Vector3(1, 1, 0), // 4
    new Vector3(1, 0, 1), // 5
    new Vector3(0, 1, 1), // 6
    new Vector3(1, 1, 1), // 7
};

    int[] tris = new int[]
    {
     0, 4, 1, 0, 2, 4, // front
     5, 6, 3, 5, 7, 6, // back
     1, 7, 5, 1, 4, 7, // right
     3, 2, 0, 3, 6, 2, // left
     2, 7, 4, 2, 6, 7, // top
     0, 1, 5, 0, 5, 3, // bottom

    };
    */
    /*List<Vector3> verts = new List<Vector3>
    {
        new Vector3(-5.10f, 0, 6.10f),
        new Vector3(-4.95f, 0, 6.10f),
        new Vector3(-4.80f, 0, 6.10f),
        new Vector3(-4.65f, 0, 6.10f),
        new Vector3(-4.50f, 0, 6.10f),
        new Vector3(-4.35f, 0, 6.11f),
        new Vector3(-4.20f, 0, 6.11f),
        new Vector3(-4.05f, 0, 6.12f),
        new Vector3(-3.90f, 0, 6.12f),
        new Vector3(-3.75f, 0, 6.12f),
        new Vector3(-3.60f, 0, 6.12f),
        new Vector3(-3.45f, 0, 6.12f),
        new Vector3(-3.30f, 0, 6.11f),
        new Vector3(-3.15f, 0, 6.09f),
        new Vector3(-3.00f, 0, 6.07f),
        new Vector3(-2.86f, 0, 6.03f),
        new Vector3(-2.72f, 0, 5.97f),
        new Vector3(-2.59f, 0, 5.90f),
        new Vector3(-2.46f, 0, 5.82f),
        new Vector3(-2.35f, 0, 5.72f),
        new Vector3(-2.25f, 0, 5.61f),
        new Vector3(-2.15f, 0, 5.49f),
        new Vector3(-2.07f, 0, 5.37f),
        new Vector3(-1.99f, 0, 5.24f),
        new Vector3(-1.92f, 0, 5.10f),
        new Vector3(-1.86f, 0, 4.97f),
        new Vector3(-1.81f, 0, 4.82f),
        new Vector3(-1.78f, 0, 4.68f),
        new Vector3(-1.75f, 0, 4.53f),
        new Vector3(-1.73f, 0, 4.38f),
        new Vector3(-1.71f, 0, 4.23f),
        new Vector3(-1.69f, 0, 4.08f),
        new Vector3(-1.67f, 0, 3.94f),
        new Vector3(-1.65f, 0, 3.79f),
        new Vector3(-1.62f, 0, 3.64f),
        new Vector3(-1.59f, 0, 3.49f),
        new Vector3(-1.56f, 0, 3.34f),
        new Vector3(-1.53f, 0, 3.20f),
        new Vector3(-1.49f, 0, 3.05f),
        new Vector3(-1.45f, 0, 2.91f),
        new Vector3(-1.41f, 0, 2.76f),
        new Vector3(-1.37f, 0, 2.62f),
        new Vector3(-1.32f, 0, 2.48f),
        new Vector3(-1.27f, 0, 2.34f),
        new Vector3(-1.21f, 0, 2.20f),
        new Vector3(-1.15f, 0, 2.06f),
        new Vector3(-1.09f, 0, 1.92f),
        new Vector3(-1.02f, 0, 1.79f),
        new Vector3(-0.94f, 0, 1.66f),
        new Vector3(-0.85f, 0, 1.54f),
        new Vector3(-0.75f, 0, 1.42f),
        new Vector3(-0.65f, 0, 1.32f),
        new Vector3(-0.53f, 0, 1.22f),
        new Vector3(-0.41f, 0, 1.14f),
        new Vector3(-0.28f, 0, 1.07f),
        new Vector3(-0.14f, 0, 1.01f),
        new Vector3(0.01f, 0, 0.97f),
        new Vector3(0.16f, 0, 0.94f),
        new Vector3(0.30f, 0, 0.92f),
        new Vector3(0.45f, 0, 0.91f),
        new Vector3(0.60f, 0, 0.92f),
        new Vector3(0.75f, 0, 0.93f),
        new Vector3(0.90f, 0, 0.95f),
        new Vector3(1.05f, 0, 0.97f),
        new Vector3(1.20f, 0, 1.00f),
        new Vector3(1.34f, 0, 1.05f),
        new Vector3(1.48f, 0, 1.10f),
        new Vector3(1.62f, 0, 1.15f),
        new Vector3(1.76f, 0, 1.22f),
        new Vector3(1.89f, 0, 1.29f),
        new Vector3(2.02f, 0, 1.37f),
        new Vector3(2.14f, 0, 1.45f),
        new Vector3(2.27f, 0, 1.54f),
        new Vector3(2.39f, 0, 1.63f),
        new Vector3(2.51f, 0, 1.72f),
        new Vector3(2.62f, 0, 1.82f),
        new Vector3(2.74f, 0, 1.91f),
        new Vector3(2.86f, 0, 2.00f),
        new Vector3(2.98f, 0, 2.09f),
        new Vector3(3.10f, 0, 2.18f),
        new Vector3(3.23f, 0, 2.26f),
        new Vector3(3.36f, 0, 2.33f),
        new Vector3(3.50f, 0, 2.40f),
        new Vector3(3.64f, 0, 2.45f),
        new Vector3(3.78f, 0, 2.49f),
        new Vector3(3.93f, 0, 2.53f),
        new Vector3(4.08f, 0, 2.54f),
        new Vector3(4.23f, 0, 2.55f),
        new Vector3(4.38f, 0, 2.56f),
        new Vector3(4.53f, 0, 2.57f),
        new Vector3(4.68f, 0, 2.58f),
        new Vector3(4.82f, 0, 2.60f),
        new Vector3(4.97f, 0, 2.63f),
        new Vector3(5.12f, 0, 2.66f),
        new Vector3(5.26f, 0, 2.70f),
        new Vector3(5.40f, 0, 2.75f),
        new Vector3(5.54f, 0, 2.81f),
        new Vector3(5.68f, 0, 2.87f),
        new Vector3(5.82f, 0, 2.94f),
        new Vector3(5.95f, 0, 3.01f),
        new Vector3(6.08f, 0, 3.08f),
        new Vector3(6.22f, 0, 3.14f),
        new Vector3(6.35f, 0, 3.21f),
        new Vector3(6.49f, 0, 3.26f),
        new Vector3(6.64f, 0, 3.29f),
        new Vector3(6.79f, 0, 3.30f),
        new Vector3(6.94f, 0, 3.29f),
        new Vector3(7.09f, 0, 3.26f),
        new Vector3(7.23f, 0, 3.21f),
        new Vector3(7.37f, 0, 3.15f),
        new Vector3(7.50f, 0, 3.08f),
        new Vector3(7.63f, 0, 3.00f),
        new Vector3(7.75f, 0, 2.92f),
        new Vector3(7.88f, 0, 2.84f),
        new Vector3(8.01f, 0, 2.76f),
        new Vector3(8.14f, 0, 2.68f),
        new Vector3(8.26f, 0, 2.60f),
        new Vector3(8.39f, 0, 2.52f),
        new Vector3(8.52f, 0, 2.45f),
        new Vector3(8.65f, 0, 2.37f),
        new Vector3(8.78f, 0, 2.29f),
        new Vector3(8.90f, 0, 2.21f),
        new Vector3(9.03f, 0, 2.13f),
        new Vector3(9.16f, 0, 2.04f),
        new Vector3(9.28f, 0, 1.96f),
        new Vector3(9.40f, 0, 1.87f),
        new Vector3(9.51f, 0, 1.77f),
        new Vector3(9.62f, 0, 1.66f),
        new Vector3(9.72f, 0, 1.55f),
        new Vector3(9.81f, 0, 1.43f),
        new Vector3(9.88f, 0, 1.30f),
        new Vector3(9.94f, 0, 1.16f),
        new Vector3(10.00f, 0, 1.02f),
        new Vector3(10.03f, 0, 0.88f),
        new Vector3(10.07f, 0, 0.73f),
        new Vector3(10.09f, 0, 0.58f),
        new Vector3(10.10f, 0, 0.43f),
        new Vector3(10.11f, 0, 0.28f),
        new Vector3(10.12f, 0, 0.13f),
        new Vector3(10.12f, 0, -0.02f),
        new Vector3(10.12f, 0, -0.17f),
        new Vector3(10.12f, 0, -0.32f),
        new Vector3(10.11f, 0, -0.47f),
        new Vector3(10.09f, 0, -0.62f),
        new Vector3(10.07f, 0, -0.77f),
        new Vector3(10.04f, 0, -0.92f),
        new Vector3(9.99f, 0, -1.06f),
        new Vector3(9.93f, 0, -1.20f),
        new Vector3(9.85f, 0, -1.33f),
        new Vector3(9.76f, 0, -1.45f),
        new Vector3(9.66f, 0, -1.56f),
        new Vector3(9.55f, 0, -1.66f),
        new Vector3(9.43f, 0, -1.74f),
        new Vector3(9.30f, 0, -1.82f),
        new Vector3(9.16f, 0, -1.88f),
        new Vector3(9.02f, 0, -1.93f),
        new Vector3(8.88f, 0, -1.98f),
        new Vector3(8.73f, 0, -2.01f),
        new Vector3(8.58f, 0, -2.04f),
        new Vector3(8.43f, 0, -2.06f),
        new Vector3(8.28f, 0, -2.07f),
        new Vector3(8.13f, 0, -2.08f),
        new Vector3(7.98f, 0, -2.08f),
        new Vector3(7.83f, 0, -2.08f),
        new Vector3(7.68f, 0, -2.07f),
        new Vector3(7.53f, 0, -2.06f),
        new Vector3(7.38f, 0, -2.05f),
        new Vector3(7.23f, 0, -2.05f),
        new Vector3(7.08f, 0, -2.04f),
        new Vector3(6.93f, 0, -2.04f),
        new Vector3(6.78f, 0, -2.05f),
        new Vector3(6.63f, 0, -2.08f),
        new Vector3(6.49f, 0, -2.11f),
        new Vector3(6.35f, 0, -2.17f),
        new Vector3(6.21f, 0, -2.23f),
        new Vector3(6.08f, 0, -2.31f),
        new Vector3(5.96f, 0, -2.40f),
        new Vector3(5.85f, 0, -2.50f),
        new Vector3(5.74f, 0, -2.61f),
        new Vector3(5.64f, 0, -2.71f),
        new Vector3(5.53f, 0, -2.82f),
        new Vector3(5.42f, 0, -2.92f),
        new Vector3(5.31f, 0, -3.02f),
        new Vector3(5.19f, 0, -3.10f),
        new Vector3(5.05f, 0, -3.18f),
        new Vector3(4.92f, 0, -3.24f),
        new Vector3(4.78f, 0, -3.30f),
        new Vector3(4.64f, 0, -3.35f),
        new Vector3(4.50f, 0, -3.40f),
        new Vector3(4.35f, 0, -3.44f),
        new Vector3(4.21f, 0, -3.49f),
        new Vector3(4.06f, 0, -3.52f),
        new Vector3(3.91f, 0, -3.55f),
        new Vector3(3.77f, 0, -3.57f),
        new Vector3(3.62f, 0, -3.58f),
        new Vector3(3.47f, 0, -3.59f),
        new Vector3(3.32f, 0, -3.59f),
        new Vector3(3.17f, 0, -3.59f),
        new Vector3(3.02f, 0, -3.59f),
        new Vector3(2.87f, 0, -3.58f),
        new Vector3(2.72f, 0, -3.58f),
        new Vector3(2.56f, 0, -3.58f),
        new Vector3(2.41f, 0, -3.57f),
        new Vector3(2.26f, 0, -3.57f),
        new Vector3(2.11f, 0, -3.57f),
        new Vector3(2.11f, 0, -3.57f),
        new Vector3(1.96f, 0, -3.58f),
        new Vector3(1.81f, 0, -3.58f),
        new Vector3(1.66f, 0, -3.58f),
        new Vector3(1.51f, 0, -3.59f),
        new Vector3(1.36f, 0, -3.59f),
        new Vector3(1.21f, 0, -3.59f),
        new Vector3(1.06f, 0, -3.59f),
        new Vector3(0.91f, 0, -3.59f),
        new Vector3(0.76f, 0, -3.58f),
        new Vector3(0.61f, 0, -3.58f),
        new Vector3(0.46f, 0, -3.56f),
        new Vector3(0.31f, 0, -3.54f),
        new Vector3(0.17f, 0, -3.52f),
        new Vector3(0.02f, 0, -3.49f),
        new Vector3(-0.13f, 0, -3.46f),
        new Vector3(-0.27f, 0, -3.42f),
        new Vector3(-0.41f, 0, -3.37f),
        new Vector3(-0.55f, 0, -3.31f),
        new Vector3(-0.69f, 0, -3.25f),
        new Vector3(-0.82f, 0, -3.17f),
        new Vector3(-0.95f, 0, -3.09f),
        new Vector3(-1.07f, 0, -3.00f),
        new Vector3(-1.18f, 0, -2.91f),
        new Vector3(-1.30f, 0, -2.81f),
        new Vector3(-1.42f, 0, -2.72f),
        new Vector3(-1.54f, 0, -2.63f),
        new Vector3(-1.66f, 0, -2.54f),
        new Vector3(-1.79f, 0, -2.47f),
        new Vector3(-1.93f, 0, -2.40f),
        new Vector3(-2.06f, 0, -2.34f),
        new Vector3(-2.21f, 0, -2.29f),
        new Vector3(-2.35f, 0, -2.26f),
        new Vector3(-2.50f, 0, -2.24f),
        new Vector3(-2.65f, 0, -2.23f),
        new Vector3(-2.80f, 0, -2.24f),
        new Vector3(-2.95f, 0, -2.26f),
        new Vector3(-3.10f, 0, -2.29f),
        new Vector3(-3.24f, 0, -2.32f),
        new Vector3(-3.39f, 0, -2.36f),
        new Vector3(-3.53f, 0, -2.41f),
        new Vector3(-3.67f, 0, -2.45f),
        new Vector3(-3.82f, 0, -2.50f),
        new Vector3(-3.96f, 0, -2.55f),
        new Vector3(-4.10f, 0, -2.61f),
        new Vector3(-4.24f, 0, -2.66f),
        new Vector3(-4.38f, 0, -2.71f),
        new Vector3(-4.52f, 0, -2.76f),
        new Vector3(-4.66f, 0, -2.81f),
        new Vector3(-4.81f, 0, -2.85f),
        new Vector3(-4.95f, 0, -2.90f),
        new Vector3(-5.10f, 0, -2.94f),
        new Vector3(-5.24f, 0, -2.97f),
        new Vector3(-5.39f, 0, -3.00f),
        new Vector3(-5.54f, 0, -3.02f),
        new Vector3(-5.69f, 0, -3.02f),
        new Vector3(-5.84f, 0, -3.01f),
        new Vector3(-5.98f, 0, -2.97f),
        new Vector3(-6.12f, 0, -2.91f),
        new Vector3(-6.24f, 0, -2.82f),
        new Vector3(-6.35f, 0, -2.71f),
        new Vector3(-6.44f, 0, -2.59f),
        new Vector3(-6.52f, 0, -2.47f),
        new Vector3(-6.59f, 0, -2.33f),
        new Vector3(-6.65f, 0, -2.20f),
        new Vector3(-6.70f, 0, -2.05f),
        new Vector3(-6.75f, 0, -1.91f),
        new Vector3(-6.80f, 0, -1.77f),
        new Vector3(-6.84f, 0, -1.63f),
        new Vector3(-6.89f, 0, -1.48f),
        new Vector3(-6.93f, 0, -1.34f),
        new Vector3(-6.99f, 0, -1.20f),
        new Vector3(-7.04f, 0, -1.06f),
        new Vector3(-7.10f, 0, -0.92f),
        new Vector3(-7.17f, 0, -0.79f),
        new Vector3(-7.24f, 0, -0.65f),
        new Vector3(-7.32f, 0, -0.53f),
        new Vector3(-7.41f, 0, -0.41f),
        new Vector3(-7.51f, 0, -0.30f),
        new Vector3(-7.63f, 0, -0.20f),
        new Vector3(-7.76f, 0, -0.12f),
        new Vector3(-7.89f, 0, -0.05f),
        new Vector3(-8.02f, 0, 0.03f),
        new Vector3(-8.14f, 0, 0.12f),
        new Vector3(-8.25f, 0, 0.22f),
        new Vector3(-8.35f, 0, 0.33f),
        new Vector3(-8.43f, 0, 0.46f),
        new Vector3(-8.50f, 0, 0.59f),
        new Vector3(-8.57f, 0, 0.72f),
        new Vector3(-8.63f, 0, 0.86f),
        new Vector3(-8.69f, 0, 1.00f),
        new Vector3(-8.74f, 0, 1.14f),
        new Vector3(-8.78f, 0, 1.28f),
        new Vector3(-8.83f, 0, 1.43f),
        new Vector3(-8.87f, 0, 1.57f),
        new Vector3(-8.90f, 0, 1.72f),
        new Vector3(-8.93f, 0, 1.87f),
        new Vector3(-8.95f, 0, 2.02f),
        new Vector3(-8.97f, 0, 2.16f),
        new Vector3(-8.97f, 0, 2.32f),
        new Vector3(-8.97f, 0, 2.47f),
        new Vector3(-8.95f, 0, 2.61f),
        new Vector3(-8.91f, 0, 2.76f),
        new Vector3(-8.87f, 0, 2.90f),
        new Vector3(-8.81f, 0, 3.04f),
        new Vector3(-8.74f, 0, 3.18f),
        new Vector3(-8.66f, 0, 3.30f),
        new Vector3(-8.57f, 0, 3.43f),
        new Vector3(-8.48f, 0, 3.54f),
        new Vector3(-8.38f, 0, 3.66f),
        new Vector3(-8.28f, 0, 3.77f),
        new Vector3(-8.18f, 0, 3.88f),
        new Vector3(-8.08f, 0, 3.99f),
        new Vector3(-7.97f, 0, 4.10f),
        new Vector3(-7.87f, 0, 4.21f),
        new Vector3(-7.77f, 0, 4.32f),
        new Vector3(-7.66f, 0, 4.42f),
        new Vector3(-7.56f, 0, 4.54f),
        new Vector3(-7.47f, 0, 4.65f),
        new Vector3(-7.37f, 0, 4.77f),
        new Vector3(-7.28f, 0, 4.89f),
        new Vector3(-7.20f, 0, 5.01f),
        new Vector3(-7.12f, 0, 5.14f),
        new Vector3(-7.03f, 0, 5.27f),
        new Vector3(-6.95f, 0, 5.39f),
        new Vector3(-6.86f, 0, 5.51f),
        new Vector3(-6.77f, 0, 5.63f),
        new Vector3(-6.67f, 0, 5.74f),
        new Vector3(-6.55f, 0, 5.84f),
        new Vector3(-6.43f, 0, 5.92f),
        new Vector3(-6.29f, 0, 5.98f),
        new Vector3(-6.15f, 0, 6.03f),
        new Vector3(-6.00f, 0, 6.07f),
        new Vector3(-5.85f, 0, 6.09f),
        new Vector3(-5.70f, 0, 6.10f),
        new Vector3(-5.55f, 0, 6.10f),
        new Vector3(-5.40f, 0, 6.10f),
        new Vector3(-5.25f, 0, 6.10f),
        new Vector3(-5.10f, 0, 6.10f),
    };
*/
    
    [SerializeField]
    [Range(1f, 50f)]
    float _scale = 15f;

    [SerializeField]
    GameObject _gameObject;

    public List<Vector3> centerLine;
    public List<Vector3> endLineRight, endLineLeft;

    RoadBuilder builder;

    InteractionManager interactionManager;

    public void setInteractionManager(InteractionManager interactionManager)
    {
        this.interactionManager = interactionManager;
    }

    public void generate(List<Vector3> verts)
    {
        if(interactionManager != null)
        {

            builder = GetComponent<RoadBuilder>();
            builder.setPath(verts, _scale);
            Mesh trackMesh = GetComponent<MeshFilter>().mesh;
            Vector3 localScale = gameObject.transform.localScale;
            for (int i = 0; i < trackMesh.vertices.Length; i++)
            {
                if (i % 2 == 0) endLineRight.Add(Vector3.Scale(localScale, trackMesh.vertices[i]));
                else
                {
                    endLineLeft.Add(Vector3.Scale(localScale, trackMesh.vertices[i]));
                    centerLine.Add((endLineLeft.Last()+ endLineRight.Last())*0.5f);
                }
            }
            // TODO: use edge line for creating curbs
            interactionManager.setWaypoints(centerLine);
        }
        else
        {
            Debug.Log("The hell is here");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponentInParent<CarControl>().all_wheels_on_track = false;
    }
}