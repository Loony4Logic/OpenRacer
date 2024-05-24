from fastapi import FastAPI, WebSocket
import numpy as np

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
        print(len(res))
        await websocket.send_text(str(res))

def checkCommand(signal):
    if "track" in signal:
        track_name = signal.split(":")[1].trim()
        track = np.load(f"{track_name}.npy")
        return  [(point[0], 0, point[1]) for point in track]
    elif "eval" in signal:
        command = signal.split(':')[1].trim()
        print(command)
        return {"x": 0.1, "y":0.1}
