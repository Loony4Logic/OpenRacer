from abc import abstractmethod
import json
import os
import pickle
import numpy as np
from typing import List

from OpenRacer.Recorder import Recorder
from OpenRacer.datatypes import Params

class ModelInterface:
    def __init__(self):
        """Initializing BaseClass for AI Model
        """
        print("Model started")
        self.session = 0
        self.models = dict()
        self.currentModel = None
        self.recorder: Recorder = None
        self.currentPath: str = None
        
    def addModel(self, model):
        """add ML model with a name atteibute. model.name should be a non empty string.

        Args:
            model (class): a class of ML that will have eval, train, backPropagate, loss fn
        """
        if not hasattr(model, "name"):
            print(f"class {model} doesn't contain attribute name.")
            exit(0)
            return
        recorder = Recorder(model.name)
        self.models[model.name] = {"model": model, "recorder": recorder}
    
    def setModel(self, modelName):
        """set model to be used for eval and train

        Args:
            modelName (str): name of model to be used. it should be present in added models. 
        """
        if self.models.get(modelName):
            self.currentModel = self.models[modelName]["model"]
            self.recorder = self.models[modelName]["recorder"]
            self.currentPath = os.path.join(os.getcwd(), self.currentModel.name, "model")
            self.currentFile = os.path.join(self.currentPath, f"{self.currentModel.name}.pkl")
            os.makedirs(self.currentPath, exist_ok=True)
    
    def getModel(self):
        """ returns model that will be used for training and testing """
        return self.currentModel

    def setTrack(self, track:List[List[float]]):
        """Setting track coordniates for the session 

        Args:
            track (List[Tuple[float]]): List of Tuple of coordinates. (x, y, z). Coordinates are according to Unity. x, z should be used for 2Dcase. (x,z) => (x,y)
        """
        self.track = track
        for model in self.models:
            self.models[model]["model"].track = track

    def sessionEnd(self, sessionNum:int):
        """called after every epoc end to save model. for more control you can add save function to model class and it will be called and interface will not save it.  

        Args:
            sessionNum (int): number of epoch ended 
        """
        print(f"Session {sessionNum} ended. Saving model checkpoint")
        if getattr(self.currentModel, "save"):
            self.currentModel.save(sessionNum, self.currentPath)
        else:
            self.currentFile = os.path.join(self.currentPath, f"{self.currentModel.name}-{self.session}.pkl")
            pickle.dump(self.currentModel, open(self.currentFile, 'wb'))
        self.session = sessionNum+1
        self.action = None
        
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
            if getattr(self, "action", None) != None:
                reward = self.rewardFn(self.action, inputData)
                self.backprop(self.action, inputData)
            else:
                reward = [0 for i in range(self.agentCount)]
            self.action = self.trainEval(inputData)
            self.recorder.recordStep(formattedInputData, self.action, reward, self.session)
        else:
            if getattr(self, "action", None) != None:
                reward = self.rewardFn(self.action, inputData)
            else:
                reward = [0 for i in range(self.agentCount)]
            self.action = self.testEval(inputData)
            self.recorder.recordRaceStep(formattedInputData, self.action, reward, self.session)
        return self.formatAction(self.action)
          
    def trainEval(self, inputData) -> List[List[float]]:
        """
        This will be called for each step in training. it will call trainEval function of current model

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
        return self.currentModel.trainEval(inputData)
    
    def testEval(self, inputData):
        """
        This will be called for each step in testing/Race. It will call testEval of current model

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
        return self.currentModel.testEval(inputData)
    
    def rewardFn(self, action:List[List[float]], inputData) -> List[float]:
        """It will be called on each step. you can define how to reward your Agent based on its input and action. it will call reward function on current model

        Args:
            action (List[List[float]]): It will be a 2D List. [[0.1,0.2], [0.3,-0.1]] 
            inputData (Params|Object): It would be same input data as provided in trainEval/TestEval.

        Returns:
            List[float]: Rewards for each Agent.
        """
        return self.currentModel.rewardFn(action, inputData)
    
    def backprop(self, action, inputData):
        """This will be called while training after every step this can be used to evaluate your step and adjust the model.

        Args:
            action (List[List[float]]): It will be a 2D List. [[0.1,0.2], [0.3,-0.1]] 
            inputData (Params|Object): It would be same input data as provided in trainEval/TestEval.
        """
        return self.currentModel.backprop(action, inputData)
    
    def preProcess(self, inputData:Params):
        """Incase you want to preprocess your inputs.

        Args:
            inputData (Params): This is data received from Unity

        Returns:
            Processed Data. Default is Params.
        """
        if hasattr(self.currentModel, "preProcess"):
            return self.currentModel.preProcess(inputData)
        return inputData
    
    def formatInput(self, unprocessedInput:str) -> Params:
        """This converts data from string received from untiy to Prams 

        Args:
            unprocessedInput (str): Str message received from unity over Websocket.

        Returns:
            Params: Processed input for taking next step.
        """
        inputData = json.loads(f"[{unprocessedInput}]")
        self.agentCount = len(inputData)
        return inputData
    
    def formatAction(self, action:np.ndarray):
        return {"actions": list(map(lambda x: {"x":x[0], "y":x[1]}, action))}


class ModelBase:
    def __init__(self):
        self.name = "unknown"
    
    def preProcess(self, inputData:Params):
        """Incase you want to preprocess your inputs.

        Args:
            inputData (Params): This is data received from Unity

        Returns:
            Processed Data. Default is Params.
        """
        return inputData
        
    def trainEval(self, inputData) -> List[List[float]]:
        """
        This will be called for each step in training. it will call trainEval function of current model
            Note:
                output range should be [-1,1] in both axis. 
                x: [-1, 1] -> [Backward, Forward]
                y: [-1, 1] -> [Left, Right]

            Example output:
                For eg: For 2 agents, 
                [
                    [0.5, 0.2], #Agent 1: 0.5 forward and 0.2 towards Right 
                    [0.6, -0.1] #Agent 2: 0.6 forward and 0.1 towards Left 
                ]

        Args:
            inputData (Returned from PreProcess): This will contain same object returned from preProcess. If not set by default it will get Params. 
            
        Returns:
            List[List[float]]: It should return a list of actions need to be taken by Agent. 
            
        """
        Exception(f"Train Eval (trainEval) function not delcared for {self.name}")
    
    def testEval(self, inputData):
        """
        This will be called for each step in testing/Race. It will call testEval of current model
            Note:
                output range should be [-1,1] in both axis. 
                x: [-1, 1] -> [Backward, Forward]
                y: [-1, 1] -> [Left, Right]

            Example output:
                For eg: For 2 agents, 
                [
                    [0.5, 0.2], #Agent 1: 0.5 forward and 0.2 towards Right 
                    [0.6, -0.1] #Agent 2: 0.6 forward and 0.1 towards Left 
                ]

        Args:
            inputData (Returned from PreProcess): This will contain same object returned from preProcess. If not set by default it will get Params. 
            
        Returns:
            List[List]: It should return a list of actions need to be taken by Agent. 
                
        """
        Exception(f"Test Eval (testEval) function not delcared for {self.name}")

    def rewardFn(self, action:List[List[float]], inputData) -> List[float]:
        """It will be called on each step. you can define how to reward your Agent based on its input and action. it will call reward function on current model

        Args:
            action (List[List[float]]): It will be a 2D List. [[0.1,0.2], [0.3,-0.1]] 
            inputData (Params|Object): It would be same input data as provided in trainEval/TestEval.

        Returns:
            List[float]: Rewards for each Agent.
        """
        Exception(f"reward function not delcared for {self.name}")
    
    def backprop(self, action, inputData):
        """This will be called while training after every step this can be used to evaluate your step and adjust the model.

        Args:
            action (List[List[float]]): It will be a 2D List. [[0.1,0.2], [0.3,-0.1]] 
            inputData (Params|Object): It would be same input data as provided in trainEval/TestEval.
        """
        Exception(f"backprop function not delcared for {self.name}")

    def save(self, sessionNum, dirPath):
        """this function will be called at the end of every session to save the model. user needs to implement save functionality.
        it can be saved on the directory path provided in the input. 

        Args:
            sessionNum (int): session number of currently ended session. 
            dirPath (str): path to the directory created by OpenRacer module.
        """
        print(f"Save not implemented. Not saving model for {self.name}")

    