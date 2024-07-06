from fastapi import FastAPI, WebSocket
import numpy as np
import json
import random
from enum import Enum 
import os

ACK = "ack"

class COMMAND(str, Enum):
    Track = "track"
    TrackAck = "trackAck"
    Epoch = "epoch"
    Eval = "eval"
    End = "end"

Track = []

def getCommand(signal) -> list:
    assert(len(signal.split("~")) == 2)
    return signal.split("~")

def EvalBatch(inputData):
    a = 0
    for i in range(int(1e6)):
        a+=10
    # Placeholder for ML computation step about 2 sec per step
    return {"actions": [{"x": random.randrange(-100, 100)/100, "y":random.randrange(-2, 5)/2} for i in range(len(inputData))]}               

# TODO: Create an interface
# TODO: make a ping pong point
# TODO: Add pretty print and fancy console 
app = FastAPI()

@app.get("/")
def Hello():
    return "hello"

@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    while True:
        signal = await websocket.receive_text()
        res = await checkCommand(signal)
        await websocket.send_text(json.dumps(res))

async def checkCommand(signal):
    global Track 
    command, value = getCommand(signal)
    if command == COMMAND.Track:
        track_name = value
        if not os.path.isfile(f"{track_name}.npy"):
            print("file not find")
            return np.load("albert.npy")
        track = np.load(f"{track_name}.npy")
        trackVert = [{"x":point[0], "y": 0, "z":point[1]} for point in track[:-1]]
        Track = trackVert
        return  {"track": trackVert}
    elif command == COMMAND.TrackAck:
        track_coords_string = value
        Track = json.loads(track_coords_string)
        return ACK
    elif command == COMMAND.Epoch:
        print(f"Completed Epoch{value}")
        return ACK
    elif command == COMMAND.Eval:
        inputData = json.loads(f"[{value}]")
        output = EvalBatch(inputData)
        return output
    elif command == COMMAND.End:
        print("Training Ended")
        return ACK