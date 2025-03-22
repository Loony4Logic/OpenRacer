# OpenRacer

## Overview

The OpenRacer WebSocket interface facilitates communication between the Unity-based simulator and Python-based machine learning models. The simulator sends input data for each frame, and the ML model processes it before sending a response back.

This package provides a server interface that manages WebSocket connections and ensures seamless real-time data exchange.

## Installation

```sh
pip install OpenRacer
```

## Usage

### Creating a Custom Model

To integrate an ML model, create a class that **inherits from** `ModelBase` in the OpenRacer package and implement following methonds
* `preProcess` (optional) : used for processing input before passing to `trainEval` or `testEval`
* `trainEval` : will be called each frame for training
* `testEval`: will be called each frame for testing/Race/eval
* `rewardFn`: will be called in each eval to monitor model performance
* `backprop` : will be called for each step in training. Use this to update model based on eval.


```python
from OpenRacer.Model import ModelBase

class MyModel(ModelBase):
    # implement the required functions 
```

### Starting the Interface

The Interface is responsible for handling connections between the simulator and the ML model and Dashboard.

```python
from OpenRacer.Interface import Interface

# Create and start the server
Interface(model=modelInterface).start()
```

### API Reference

Will be updated in some time

### Notes
- Ensure that the **WebSocket server is running before launching the simulator.**

### License

This package is open-source under the MIT License.

### Contributing

Contributions are welcome! Feel free to submit issues or pull requests on GitHub.

### Contact

For questions, reach out to the OpenRacer team via GitHub.

