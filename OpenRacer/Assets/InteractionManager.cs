
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
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


public class StateProcessor
{
    public List<Vector3> waypoints = new List<Vector3>();
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
        ps.track_width = 7.5f;
        int closestPoint = getClosetWaypoint(ps.x, ps.y);
        ps.closest_waypoints = new int[] { closestPoint, closestPoint + 1};

        return ps;
    }

    // TODO: optimize this with checking only chunck
    public int getClosetWaypoint(float x, float y)
    {
        int closestPoint = 0;
        float closestDist = Mathf.Infinity;
        for (int i = 0;i<waypoints.Count;i++)
        {
            float currentDist = Vector3.Distance(waypoints[i], new Vector3(x, 0, y));
            if (currentDist < closestDist )
            {
                closestDist = currentDist;
                closestPoint = i;
            }
        }
        return closestPoint;
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

    public void setWaypoints(List<Vector3> waypoints)
    {
        stateProcessor.waypoints = waypoints;
    }

    public async Task<Track> GetTrackVerts(string trackName) 
    {
        string messageToSend = "track~" + trackName;
        string trackJsonString = await serverConnector.sendToWebsocket(messageToSend);
        Track track = JsonUtility.FromJson<Track>(trackJsonString);
        return track;
    }

    public async Task<Action> interact(RawState rawState)
    {
        string messageForSocket = stateProcessor.getState(rawState);
        string command = await serverConnector.sendToWebsocket(messageForSocket);
        Action action = actionProcessor.processAction(command);
        return action;
    }

    public async Task<List<Action>> sendBatch(List<RawState> rawState)
    {
        string messageForSocket = "eval~";
        List<string> states = new List<string>();
        for(int i = 0; i < rawState.Count; i++)
        {
            states.Add(stateProcessor.getState(rawState[i]));
        }
        messageForSocket += states.ToCommaSeparatedString();
        string command = await serverConnector.sendToWebsocket(messageForSocket);
        List<Action> actions = actionProcessor.processActions(command);
        return actions;
    }

}
