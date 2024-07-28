import { LineChart } from "@mui/x-charts";
import { API_PATH } from "./Constants";
import { useEffect, useState } from "react";
import { DataControl } from "./DataControl";

export function ComparisionChart({agentCount, sessionCount}) {
  const [xData, setXData] = useState([]);
  const [yData1, setYData1] = useState([]);
  const [yData2, setYData2] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  const [step, setStep] = useState(10);
  const [session1, setSession1] = useState(0);
  const [agent1, setAgent1] = useState(0);
  const [session2, setSession2] = useState(0);
  const [agent2, setAgent2] = useState(0);
  const [parameter, setParameter] = useState("speed");

  useEffect(() => {
    setIsLoading(true);
    Promise.all([
      fetch(`${API_PATH}/getChartData/${parameter}/${agent1}/${session1}`),
      fetch(`${API_PATH}/getChartData/${parameter}/${agent2}/${session2}`),
    ])
      .then((responses) => Promise.all(responses.map((r) => r.json())))
      .then((responses) => {
        let res1 = responses[0];
        let res2 = responses[1];
        let _x = [];
        let _y1 = [];
        let _y2 = [];

        for (let i = 0; i < res1.length; i += step) {
          _x.push(new Date(res1[i][1]));
          _y1.push(res1[i][0]);
          _y2.push(res2[i][0]);
        }
        setXData(_x);
        setYData1(_y1);
        setYData2(_y2);
        setIsLoading(false);
      });
  }, [step, agent1, agent2, session1, session2, parameter]);
  return (
    <>
      <DataControl
        {...{
          step,
          agent1,
          agent2,
          session1,
          session2,
          setAgent1,
          setAgent2,
          setSession1,
          setSession2,
          setStep,
          isLoading,
          parameter,
          setParameter,
          sessionCount,
          agentCount
        }}
      />
      Compare Performace with others
      <LineChart
        xAxis={[{ scaleType: "time", data: xData }]}
        series={[
          {
            id: "Car1",
            data: yData1,
            label: "Car1",
            legend: { hidden: true },
          },
          {
            id: "Car2",
            data: yData2,
            label: "Car2",
          },
        ]}
        height={500}
      />
    </>
  );
}
