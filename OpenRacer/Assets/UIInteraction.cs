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
    //TODO: get track from server and generate track
    public async void SendUpdate()
    {
        string trackName = trackInput.text;
        Debug.Log($"Lets Race on: {trackName}");
        if (interactionManager == null) return;
        Track track = await interactionManager.GetTrackVerts( trackName );
        trackGenerator.generate(track.track);
    }

}
