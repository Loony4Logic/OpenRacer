using Mono.Cecil.Cil;

struct point
{
    public float x;
    public float y;
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
    public bool is_offtrack;                    // bool flag to indicate whether the agent has gone off track.
    public bool is_reversed;                    // flag to indicate if the agent is driving clockwise (True) or counter clockwise (False).
    public float progress;                      // percentage of track completed
    public float speed;                         // agent's speed in meters per second (m/s)
    public float steering_angle;                // agent's steering angle in degrees
    public int steps;                           // number steps completed
    public float track_length;                  // track length in meters.
    public float track_width;                   // width of the track

    public override string ToString()
    {
        return $"{{'x': {x}, 'y':{y}}}";
    }

}


public class StateProcessor
{
    ServerConnector serverConnector;
    public StateProcessor(ServerConnector serverConnector)
    {
        this.serverConnector = serverConnector;
    }

    public void sendState(RawState rawState)
    {
        ProcessedState processedState = processState(rawState);
        serverConnector.sendToWebsocket($"eval~{processedState.ToString()}");
    }

    public ProcessedState processState(RawState rawState)
    {
        ProcessedState ps = new ProcessedState();
        ps.x = rawState.x;
        ps.y = rawState.y;
        return ps;
    }

}

public class ActionProcessor 
{ 

}
