from abc import abstractmethod
import json
import math
import numpy as np
from typing import List
from Recorder import Recorder

class ModelBase:
    def __init__(self):
        print("Model started")
        self.session = 0
    
    @abstractmethod
    def getModel(self):
        pass

    def setRecorder(self, recorder:Recorder):
        self.recorder = recorder

    def setTrack(self, track:List[List[float]]):
        self.track = track
        print("Track coords Received")
        print(self.track)

    def eval(self, inputDataFromUnity:List[dict], isTraining:bool=False) -> dict:
        formattedInputData = self.formatInput(inputDataFromUnity)
        inputData = self.preProcess(formattedInputData)
        if isTraining:
            action = self.trainEval(inputData)
            reward = self.rewardFn(action, inputData)
            self.backprop(action, inputData)
        else:
            action = self.testEval(inputData)
            reward = self.rewardFn(action, inputData)
        self.recorder.record(formattedInputData, action, reward, self.session)
        return self.formatAction(action)
    
    @abstractmethod        
    def trainEval(self, inputData):
        pass
    
    @abstractmethod
    def testEval(self, inputData):
        pass
    
    @abstractmethod
    def rewardFn(self, action, inputData) -> float:
        return [0 for i in inputData]  
    
    @abstractmethod
    def backprop(self, action, inputData):
        pass
    
    
    def preProcess(self, inputData:dict):
        return inputData
    
    def formatInput(self, unprocessedInput:str) -> dict:
        inputData = json.loads(f"[{unprocessedInput}]")
        self.agnetCount = len(inputData)
        return inputData
    
    def formatAction(self, action:np.ndarray):
        return {"actions": list(map(lambda x: {"x":x[0], "y":x[1]}, action))}

class RandomModel(ModelBase):
    def __init__(self, seed:int=0):
        super().__init__()
        
    def clamp(self, n, smallest, largest): 
        return max(smallest, min(n, largest))
    
    def scale(self, n, smallest, largest, newSmallest, newLargest):
        return n* (newLargest - newSmallest)/( largest - smallest )

    
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
            
            magnitude = self.clamp(math.sqrt(dx **2 + dy** 2), -1, 1)
            print(magnitude, angle, angleScaled)
            res.append([angleScaled, magnitude])
        return res

# TODO: make a NN model 