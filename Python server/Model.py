from abc import abstractmethod
import json
import numpy as np
from typing import List
from Recorder import Recorder

class ModelBase:
    def __init__(self):
        print("Model started")
        self.recorder = Recorder()
        self.session = 0
    
    @abstractmethod
    def getModel(self):
        pass

    def setTrack(self, track:List[List[float]]):
        self.track = track

    def eval(self, inputDataFromUnity:List[dict], isTraining:bool=False) -> dict:
        formattedInputData = self.formatInput(inputDataFromUnity)
        inputData = self.preProcess(formattedInputData)
        if isTraining:
            action = self.trainEval(inputData)
            reward = self.rewardFn(action, inputData)
            self.backprop(action, inputData)
        else:
            action = self.testEval(inputData)
        self.recorder.record(formattedInputData, inputData, action, reward, self.session)
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
        return json.loads(f"[{unprocessedInput}]")
    
    def formatAction(self, action:np.ndarray):
        return {"actions": list(map(lambda x: {"x":x[0], "y":x[1]}, action))}

class RandomModel(ModelBase):
    def __init__(self, seed:int=0):
        super().__init__()
        
    def trainEval(self, inputData):
        return np.clip(np.random.rand(len(inputData),2) * 5 -2, -1,1)
    
    def testEval(self, inputData):
        return np.clip(np.random.rand(len(inputData),2) * 5 -2, -1,1)

# TODO: make a NN model 