import { useEffect, useState } from "react";
import { API_PATH } from "./Constants";
import Typography from "@mui/joy/Typography";
import Stack from "@mui/joy/Stack";
import Card from "@mui/joy/Card";
import Skeleton from "@mui/joy/Skeleton";
import { LineChart } from "@mui/x-charts/LineChart";
import Navbar from "./NavBar";
import { PerformanceChart } from "./PerformanceChart";
import { ComparisionChart } from "./ComparisionChart";
import { DataTable } from "./DataTable";
import { DetailsCard } from "./DetailsCard";

function App() {
  const [isLoading, setIsLoading] = useState(true);
  const [details, setDetails] = useState({track:"", batchSize:0, sessionTime: 0, sessions: 0});

  useEffect(()=>{
      fetch(`${API_PATH}/getRunDetails`)
      .then(res=>res.json())
      .then(res=>{
          let newDetails = {
              sessions: res[0],
              batchSize: res[1],
              track: res[2],
              sessionTime: res[3]
          }
          setDetails(newDetails);
      })
  },[])
  return (
    <>
      <Navbar />

      <Stack
        direction="column"
        justifyContent="center"
        alignItems="stretch"
        spacing={2}
        useFlexGap
        margin="10px"
      >
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="start"
          spacing={2}
          useFlexGap
        >
          <Card style={{ width: "80vw" }}>
            <PerformanceChart />
          </Card>
          <Card style={{ width: "15vw" }}>
            <DetailsCard details={details}/>
            {/* TODO: make it a track viz afterwards */}
          </Card>
        </Stack>
        <Card>
          <ComparisionChart agentCount={details.batchSize} sessionCount={details.sessions} />
        </Card>
        {/* <Card>
          <DataTable/>
        </Card> */}
      </Stack>
    </>
  );
}

export default App;
