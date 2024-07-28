import { LineChart } from "@mui/x-charts";
import { useEffect, useState } from "react";
import {API_PATH} from "./Constants"

export function PerformanceChart() {
  const [xData , setXData] = useState([])
  const [yData , setYData] = useState([])
  useEffect(()=>{
    fetch(`${API_PATH}/getProgressChartData`)
    .then(res=>res.json())
    .then(res=>{
      let _x = [];
      let _y = [];
      for(let i =0;i<res.length;i++){
        _x.push(res[i][1]+1)
        _y.push(res[i][0])
      }
      setXData(_x);
      setYData(_y);
    })
  }, []);
  return (
    <LineChart
      xAxis={[{ scaleType: "band", data: xData, label:"Session"}]}
      series={[
        {
          label: "Percentage completion %",
          data: yData,
        }
      ]}
      height={500}
    />
  );
}
