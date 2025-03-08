import numpy as np
from Interface import Interface
from Model import ModelInterface, ModelBase
import math

class RandomModel(ModelBase):
    def __init__(self, seed:int=0):
        super().__init__()
        self.name = "rand"
        
    def clamp(self, n, smallest, largest): 
        return max(smallest, min(n, largest))
    
    def scale(self, n, smallest, largest, newSmallest, newLargest):
        return n* (newLargest - newSmallest)/( largest - smallest )

    def preProcess(self, inputData):
        return inputData
    
    def trainEval(self, inputData):
        return np.clip(np.random.rand(len(inputData),2) * 5 -2, -1,1)
    
    def testEval(self, inputData):
        res = []
        for carInputData in inputData:
            x = carInputData["x"]
            y = carInputData["y"]
            nextpointId = (carInputData["closest_waypoints"][0] + 2) % len(self.track)
            temp = self.track[nextpointId]
            nextpoint = [temp[0], temp[2]]
            
            dy = nextpoint[1]-y
            dx = nextpoint[0]-x
            
            angle = self.clamp(math.degrees(math.atan(-dy/dx)), -30, 30)
            angleScaled = self.scale(angle, -30, 30, -1, 1)
            
            magnitude = self.clamp(math.sqrt(dx **2 + dy** 2), -5, 5)
            res.append([angleScaled, magnitude])
        return res
    
    def rewardFn(self, action, inputData):
        return [0 for i in range(len(action))]

# TODO: track details not received. 
# TODO: network error not working in testing
randModel = RandomModel()

modelInterface = ModelInterface()
modelInterface.addModel(randModel)
modelInterface.setModel(randModel.name)

# TODO: make a ping pong point
Interface(model=modelInterface).start()


"""app = FastAPI(title="OpenRacer API")

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
    
"""