using TMPro;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField trackInput;

    public InteractionManager interactionManager;
    public TrackGenerator trackGenerator;
    public CarManager carManager;
    public async void SendUpdate()
    {
        string trackName = trackInput.text;
        if (interactionManager == null) return;
        Track track = await interactionManager.GetTrackVerts(trackName);
        trackGenerator.generate(track.track);
        Vector3 startPoint = trackGenerator.centerLine[trackGenerator.centerLine.Count - 2];
        Vector3 nextPoint = trackGenerator.centerLine[trackGenerator.centerLine.Count - 1];
        carManager.Setup(startPoint + new Vector3(0, 2f, 0), nextPoint - startPoint);
        carManager.centerLine = trackGenerator.centerLine;
    }

}
