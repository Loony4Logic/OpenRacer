import json
from typing import List
from fastapi import APIRouter, WebSocket
from Constants import COMMAND, ACK
import os
import numpy as np
from Model import ModelBase

class Routes:
    def __init__(self, model:ModelBase):
        self.model = model
        self.communicationRoutes = APIRouter(prefix="/api/v1", tags=["Communication"])
        self.communicationRoutes.add_api_route("/", self.hello, methods=["GET"])
        self.communicationRoutes.add_api_websocket_route("/ws", self.websocket_endpoint)
        
    def hello(self):
        """Respond with hello. could be use for testing"""
        return {"data": "hello"}

    async def websocket_endpoint(self, websocket: WebSocket):
        """This is used for communicating with Unity APP using websockets. 

        Args:
            websocket (WebSocket): Websocket client to recieve and send messages
        """
        await websocket.accept()
        while True:
            signal = await websocket.receive_text()
            res = await self.checkCommand(signal)
            await websocket.send_text(json.dumps(res))
    
    def getCommand(self, signal:str) -> List[str]:
        """Seperate the signal into command and value part

        Args:
            signal (str): It should always be in format of command~value. 

        Returns:
            [command:str, value:ste]: returns the command and value.
        """
        assert(len(signal.split("~")) == 2)
        return signal.split("~")

    async def checkCommand(self, signal:str):
        """Check message recieved from unity and process to send response

        Args:
            signal (str): it will be string receivied from unity. Structure will be "command~value".

        Returns:
            str|dict|list: based on request different types of responses are generated
        """
        command, value = self.getCommand(signal)
        if command == COMMAND.Track:
            track_name = value
            if not os.path.isfile(f"{track_name}.npy"):
                print("file not find")
                return np.load("albert.npy")
            _track = np.load(f"{track_name}.npy")
            trackVert = [{"x":point[0], "y": 0, "z":point[1]} for point in _track[:-1]]
            track = trackVert
            return  {"track": trackVert}
        
        elif command == COMMAND.TrackAck:
            track_coords_string = value
            track = json.loads(track_coords_string)
            return ACK
        
        elif command == COMMAND.Epoch:
            print(f"Completed Epoch{value}")
            self.model.session = int(value)+1
            return ACK
        
        elif command == COMMAND.Eval:
            output = self.model.eval(value, isTraining=True)
            return output
        
        elif command == COMMAND.End:
            print("Training Ended")
            return ACK
