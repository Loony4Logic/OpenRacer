using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIInteraction : MonoBehaviour
{
    [SerializeField]
    public TMP_InputField trackInput;

    public InteractionManager interactionManager;
    public TrackGenerator trackGenerator;
    public CarManager carManager;
    //TODO: get track from server and generate track
    public async void SendUpdate()
    {
        string trackName = trackInput.text;
        Debug.Log($"Lets Race on: {trackName}");
        if (interactionManager == null) return;
        Track track = await interactionManager.GetTrackVerts( trackName );
        trackGenerator.generate(track.track);
        carManager.Setup(trackGenerator.centerLine[trackGenerator.centerLine.Count-3] + new Vector3(0, 2f, 0));
    }

}
