
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct Action
{
    public float x;
    public float y;

    public Action(float x, float y)
    {
        this.y = y;
        this.x = x; 
    }
    public override string ToString()
    {
        return $"{y}, {x}";
    }
}

public struct Actions 
{
    public List<Action> actions;

}

public struct RawState
{
    public bool all_wheels_on_track;            // flag to indicate if the agent is on the track
    public float x;                             // agent's x-coordinate in meters
    public float y;                             // agent's y-coordinate in meters
    public bool is_reversed;                    // flag to indicate if the agent is driving clockwise (True) or counter clockwise (False).
    public float speed;                         // agent's speed in meters per second (m/s)
    public float steering_angle;                // agent's steering angle in degrees
    public int[] closest_waypoints;             // indices of the two nearest waypoints.
    public float progress;                      // percentage of track completed
}

public struct ProcessedState
{
    public bool all_wheels_on_track;            // flag to indicate if the agent is on the track
    public float x;                             // agent's x-coordinate in meters
    public float y;                             // agent's y-coordinate in meters
    public int[] closest_waypoints;             // indices of the two nearest waypoints.
    public float distance_from_center;          // distance in meters from the track center 
    public bool is_crashed;                     // bool flag to indicate whether the agent has crashed.
    public bool is_left_of_center;              // Flag to indicate if the agent is on the left side to the track center or not. 
    public bool is_reversed;                    // flag to indicate if the agent is driving clockwise (True) or counter clockwise (False).
    public float progress;                      // percentage of track completed
    public float speed;                         // agent's speed in meters per second (m/s)
    public float steering_angle;                // agent's steering angle in degrees
    public int steps;                           // number steps completed
    public float track_length;                  // track length in meters.
    public float track_width;                   // width of the track

    public void setFromRawState(RawState state)
    {
        this.x = state.x;
        this.y = state.y;
        this.all_wheels_on_track = state.all_wheels_on_track;
        this.is_reversed = state.is_reversed;
        this.speed = state.speed;
        this.steering_angle = state.steering_angle;
        this.closest_waypoints = state.closest_waypoints;
        this.progress = state.progress;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }

}

public struct Track
{
    public List<Vector3> track;
}

public struct Details
{
    public string trackName;
    public int batchSize;
    public int epoch;
    public int sessionTime;

    public void fillDetails(string trackName, int batchSize, int epoch, int sessionTime)
    {
        this.trackName = trackName;
        this.epoch = epoch;
        this.batchSize = batchSize;
        this.sessionTime = sessionTime;
    }
}

public class StateProcessor
{
    public string getState(RawState rawState)
    {
        ProcessedState processedState = processState(rawState);
        return processedState.ToString();
    }

    public ProcessedState processState(RawState rawState)
    {
        ProcessedState ps = new ProcessedState();
        ps.setFromRawState(rawState);
        ps.is_crashed = !ps.all_wheels_on_track;
        ps.track_length = 300;

        return ps;
    }
}

public class ActionProcessor 
{
    public Action processAction(string command)
    {
        Action action = JsonUtility.FromJson<Action>(command);
        return action;
    }

    public List<Action> processActions(string command)
    {
        List<Action> actions = JsonUtility.FromJson<Actions>(command).actions;
        return actions;
    }
}

public class InteractionManager
{
    ServerConnector serverConnector;
    StateProcessor stateProcessor;
    ActionProcessor actionProcessor;

    public InteractionManager(ServerConnector serverConnector)
    {
        this.serverConnector = serverConnector;
        stateProcessor = new StateProcessor();
        actionProcessor = new ActionProcessor();
    }

    public WebSocketState status()
    {
        return this.serverConnector.status();
    }

    public async Task<Track> GetTrackVerts(string trackName) 
    {
        string messageToSend = "track~" + trackName;
        string trackJsonString = await serverConnector.sendToWebsocket(messageToSend);
        Track track = JsonUtility.FromJson<Track>(trackJsonString);
        return track;
    }

    public async Task<string> sendTrackVerts(Vector3 trackVerts)
    {
        string messageToSend = "trackAck~" + JsonUtility.ToJson(trackVerts);
        string ackMessage = await serverConnector.sendToWebsocket(messageToSend);
        Debug.Log(ackMessage);
        return ackMessage;
    }

    public async Task<Action> interact(RawState rawState)
    {
        string messageToSend = stateProcessor.getState(rawState);
        string command = await serverConnector.sendToWebsocket(messageToSend);
        Action action = actionProcessor.processAction(command);
        return action;
    }

    public async Task<List<Action>> sendBatch(List<RawState> rawState)
    {
        string messageToSend = "eval~";
        List<string> states = new List<string>();
        for(int i = 0; i < rawState.Count; i++)
        {
            states.Add(stateProcessor.getState(rawState[i]));
        }
        messageToSend += states.ToCommaSeparatedString();
        string command = await serverConnector.sendToWebsocket(messageToSend);
        List<Action> actions = actionProcessor.processActions(command);
        return actions;
    }

    public async Task<string> sendEpochEnd(int epochNum)
    {
        string messageToSend = "epoch~"+epochNum;
        string ackMessage = await serverConnector.sendToWebsocket(messageToSend);
        Debug.Log(ackMessage);
        return ackMessage;
    }

    public async Task<string> sendDetails(string trackName, int batchSize, int epoch, int sessionTime)
    {
        Details details = new Details();
        details.fillDetails(trackName, batchSize, epoch, sessionTime);
        string messageToSend = "details~" + JsonUtility.ToJson(details);
        string ackMessage = await serverConnector.sendToWebsocket(messageToSend);
        return ackMessage;
    }

}
