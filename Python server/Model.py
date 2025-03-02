from abc import abstractmethod
import json
import math
import numpy as np
from typing import List
from Recorder import Recorder
from datatypes import Params
import time

class ModelBase:
    def __init__(self):
        """Initializing BaseClass for AI Model
        """
        print("Model started")
        self.session = 0
    
    @abstractmethod
    def getModel(self):
        """ returns model that will be used for training and testing """
        pass

    def setRecorder(self, recorder:Recorder):
        """ Sets the recorder. This will be used for Recording all the steps and detials in database. """
        self.recorder = recorder

    def setTrack(self, track:List[List[float]]):
        """Setting track coordniates for the session 

        Args:
            track (List[Tuple[float]]): List of Tuple of coordinates. (x, y, z) 
            Note: Coordinates are according to Unity. x, z should be used for 2Dcase.  
            (x,z) => (x,y)
        """
        self.track = track

    def eval(self, inputDataFromUnity:str, isTraining:bool=False) -> dict:
        """This is the function that will be called on each step to evaluate what to do. 

        Args:
            inputDataFromUnity (str): This is unformatted data from Unity.
            isTraining (bool, optional): Is this a step in training. If true then It will call backpropogate. Defaults to False.

        Returns:
            dict: This is action dict that contains x, y input for Car AI in Unity.
        """
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
    def trainEval(self, inputData) -> List[List[float]]:
        """
        This will be called for each step in training.

        Args:
            inputData (Returned from PreProcess): This will contain same object returned from preProcess. If not set by default it will get Params. 
            
        Returns:
            List[List[float]]: It should return a list of actions need to be taken by Agent. 
            
        Note:
            Range should be [-1,1] in both axis. 
            x: [-1, 1] -> [Backward, Forward]
            y: [-1, 1] -> [Left, Right]

        Example:
            For eg: For 2 agents, 
            [
                [0.5, 0.2], #Agent 1: 0.5 forward and 0.2 towards Right 
                [0.6, -0.1] #Agent 2: 0.6 forward and 0.1 towards Left 
            ]
        """
        pass
    
    @abstractmethod
    def testEval(self, inputData):
        """
        This will be called for each step in testing/Race.

        Args:
            inputData (Returned from PreProcess): This will contain same object returned from preProcess. If not set by default it will get Params. 
            
        Returns:
            List[List]: It should return a list of actions need to be taken by Agent. 
            Range should be [-1,1] in both axis. 
            x: [-1, 1] -> [Backward, Forward]
            y: [-1, 1] -> [Left, Right]
            For eg: For 2 agents, 
            [
                [0.5, 0.2], #Agent 1: 0.5 forward and 0.2 towards Right 
                [0.6, -0.1] #Agent 2: 0.6 forward and 0.1 towards Left 
            ]
        """
        pass
    
    @abstractmethod
    def rewardFn(self, action:List[List[float]], inputData) -> List[float]:
        """It will be called on each step. you can define how to reward your Agent based on its input and action.

        Args:
            action (List[List[float]]): It will be a 2D List. [[0.1,0.2], [0.3,-0.1]] 
            inputData (Params|Object): It would be same input data as provided in trainEval/TestEval.

        Returns:
            List[float]: Rewards for each Agent.
        """
        return [0 for i in inputData]  
    
    @abstractmethod
    def backprop(self, action, inputData):
        """This will be called while training after every step this can be used to evaluate your step and adjust the model.

        Args:
            action (List[List[float]]): It will be a 2D List. [[0.1,0.2], [0.3,-0.1]] 
            inputData (Params|Object): It would be same input data as provided in trainEval/TestEval.
        """
        pass
    
    
    def preProcess(self, inputData:Params):
        """Incase you want to preprocess your inputs.

        Args:
            inputData (Params): This is data received from Unity

        Returns:
            Processed Data. Default is Params.
        """
        return inputData
    
    def formatInput(self, unprocessedInput:str) -> Params:
        """This converts data from string received from untiy to Prams 

        Args:
            unprocessedInput (str): Str message received from unity over Websocket.

        Returns:
            Params: Processed input for taking next step.
        """
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
            
            magnitude = self.clamp(math.sqrt(dx **2 + dy** 2), -5, 5)
            res.append([angleScaled, magnitude])
        return res

# TODO: make a NN model 