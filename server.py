from fastapi import FastAPI, WebSocket
import numpy as np
import json

app = FastAPI()

@app.get("/")
def Hello():
    return "hello"

@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await websocket.accept()
    while True:
        signal = await websocket.receive_text()
        res = checkCommand(signal)
        await websocket.send_text(json.dumps(res))

def checkCommand(signal):
    if "track~" in signal:
        track_name = signal.split("~")[1].strip()
        track = np.load(f"{track_name}.npy")
        return  [(point[0], 0, point[1]) for point in track]
    elif "eval~" in signal:
        command = signal.split('~')[1].strip()
        return {"x": 1, "y":1}