import {
  Select,
  Slider,
  Option,
  Stack,
  Card,
  Typography,
  Grid,
} from "@mui/joy";

export function RaceDataControl({
  step,
  parameter,
  setStep,
  setParameter,
  isLoading,
}) {
  return (
    <div>
      <Typography level="h3" marginBlockEnd="15px">
        Race Details
      </Typography>
      <Grid
        container
        spacing={2}
        sx={{ flexGrow: 1 }}
        justifyContent="flex-start"
        alignItems="center"
      >
        <Grid xs>
          <Card variant="soft" width="50vw">
            <Typography fontWeight="md" textColor="primary.700">
            Settings
            </Typography>
            <Grid container spacing={2}>
              <Grid xs>
                Show data for every {step} steps
                <Slider
                  style={{ width: "100%", display: "block" }}
                  valueLabelDisplay="auto"
                  variant="solid"
                  defaultValue={10}
                  max={25}
                  min={1}
                  value={step}
                  onChange={(_, val) => setStep(val)}
                  disabled={isLoading}
                  marks={[{value:1, label: 1}, {value:10, label:10}, {value:25, label:25}]}
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
