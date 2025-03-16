import { LineChart } from "@mui/x-charts";
import { API_PATH } from "./Constants";
import { useEffect, useState } from "react";
import { DataControl } from "./DataControl";
import { Typography } from "@mui/joy";
import { RaceDataControl } from "./DataControlRace";

export function RaceDetailsChart({agentCount, sessionCount}) {
  const [xData, setXData] = useState([]);
  const [yData, setYData] = useState([]);
  const [progress, setProgress] = useState([]);
  const [reward, setReward] = useState([]);
  const [speed, setSpeed] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const [step, setStep] = useState(10);
  const [session1, setSession1] = useState(0);
  const [agent1, setAgent1] = useState(0);
  const [session2, setSession2] = useState(0);
  const [agent2, setAgent2] = useState(0);
  const [parameter, setParameter] = useState("speed");

  const parmMap = {"speed":speed, "progress":progress, "reward":reward}
  const parameterMapping = (parameter)=>{return parmMap[parameter]}

  useEffect(() => {
    setIsLoading(true);
    Promise.all([
      fetch(`${API_PATH}/getRaceDetails`)
    ])
      .then((responses) => Promise.all(responses.map((r) => r.json())))
      .then((responses) => {
        responses = responses[0]
        let _x = [];
        let _progress = [];
        let _reward = [];
        let _speed = [];
        let _lap = [];

        for (let i = 0; i < responses.length; i += step) {
          _x.push(new Date(responses[i][4]));
          _progress.push(responses[i][0]);
          _reward.push(responses[i][1]);
          _speed.push(responses[i][2]);
          _lap.push(responses[i][3]);
        }
        setXData(_x);
        setProgress(_progress);
        setReward(_reward);
        setSpeed(_speed);
        // setLap(_lap);
        switch (parameter) {
            case "speed":
                setYData(_speed)    
                break;
            case "reward":
                setYData(_reward)    
                break;
            case "progress":
                setYData(_progress)    
                break;
            default:
                break;
        }
        setIsLoading(false);
      });
  }, [step]);

  const updateParam = (param)=>{
    setParameter(param)
    setYData(parmMap[param])
  }

  return (
    <>
      <RaceDataControl setParameter={updateParam}
      {...{step, parameter, setStep, isLoading}}/>
      <LineChart
        xAxis={[{ scaleType: "time", data: xData }]}
        series={[
          {
            id: "Car1",
            data: yData,
            label: "Car1",
            legend: { hidden: true },
          },
        ]}
        height={500}
      />
    </>
  );
}
