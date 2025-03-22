import numpy as np
from OpenRacer.Interface import Interface
from OpenRacer.Model import ModelInterface, ModelBase
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
            nextpointId = (carInputData["closest_waypoints"][0] + 0) % len(self.track)
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
    
    def save(self, epocNum:int):
        print(f"Saved model for epoch {epocNum}")
        return 

randModel = RandomModel()

modelInterface = ModelInterface()
modelInterface.addModel(randModel)
modelInterface.setModel(randModel.name)

Interface(model=modelInterface).start()

