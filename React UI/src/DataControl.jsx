import {
  Select,
  Slider,
  Option,
  Stack,
  Card,
  Typography,
  Grid,
} from "@mui/joy";

export function DataControl({
  step,
  agent1,
  agent2,
  session1,
  session2,
  parameter,
  setStep,
  setAgent1,
  setAgent2,
  setSession1,
  setSession2,
  setParameter,
  isLoading,
  sessionCount,
  agentCount
}) {
  return (
    <div>
      <Typography level="h3" marginBlockEnd="15px">
        Select parameter for Agent to be compared
      </Typography>
      <Grid
        container
        spacing={2}
        sx={{ flexGrow: 1 }}
        justifyContent="flex-start"
        alignItems="center"
      >
        <Grid>
          <Card variant="soft">
            <Typography fontWeight="md" textColor="primary.700">
              Agent 1
            </Typography>
            <Stack
              direction="row"
              justifyContent="flex-start"
              alignItems="center"
              spacing={2}
            >
              <Select
                placeholder="Choose Agent..."
                onChange={(e, val) => setAgent1(val)}
                value={agent1}
                disabled={isLoading}
              >
                {
                  Array.from(Array(agentCount)).map((val, idx)=><Option value={idx} key={idx}>Agent{idx+1}</Option>)
                }
              </Select>

              <Select
                placeholder="Choose session..."
                onChange={(e, val) => setSession1(val)}
                value={session1}
              >
                {
                  Array.from(Array(sessionCount)).map((val, idx)=><Option value={idx} key={idx}>session {idx+1}</Option>)
                }
              </Select>
            </Stack>
          </Card>
        </Grid>
        <Grid>
          <Typography level="body-lg" fontWeight={700}>
            VS
          </Typography>
        </Grid>
        <Grid>
          <Card variant="soft">
            <Typography fontWeight="md" textColor="primary.700">
              Agent 2
            </Typography>
            <Stack
              direction="row"
              justifyContent="flex-start"
              alignItems="center"
              spacing={2}
            >
              <Select
                placeholder="Choose Agent..."
                onChange={(e, val) => setAgent2(val)}
                value={agent2}
                disabled={isLoading}
              >
                {
                  Array.from(Array(agentCount)).map((val, idx)=><Option value={idx} key={idx}>Agent{idx+1}</Option>)
                }
              </Select>

              <Select
                placeholder="Choose session..."
                onChange={(e, val) => setSession2(val)}
                value={session2}
                disabled={isLoading}
              >
                {
                  Array.from(Array(sessionCount)).map((val, idx)=><Option value={idx} key={idx}>Session {idx+1}</Option>)
                }
              </Select>
            </Stack>
          </Card>
        </Grid>
        <Grid xs>
          <Card variant="soft" width="50vw">
            <Typography fontWeight="md" textColor="primary.700">
              Comman settings
            </Typography>
            <Grid container spacing={2}>
              <Grid xs>
                Step:
                <Slider
                  style={{ width: "100%", display: "block" }}
                  marks={false}
                  valueLabelDisplay="auto"
                  variant="solid"
                  defaultValue={10}
                  max={25}
                  min={1}
                  value={step}
                  onChange={(_, val) => setStep(val)}
                  disabled={isLoading}
                />
              </Grid>
              <Grid xs={2}>
                Parameter
                <Select
                  value={parameter}
                  onChange={(_, val) => setParameter(val)}
                  disabled={isLoading}
                >
                  <Option value={"speed"}>Speed</Option>
                  <Option value={"progress"}>Progress</Option>
                  <Option value={"reward"}>reward</Option>
                </Select>
              </Grid>
            </Grid>
          </Card>
        </Grid>
      </Grid>
    </div>
  );
}
