import { Typography } from "@mui/joy";
import { useEffect, useState } from "react";
import { API_PATH } from "./Constants";

export function DetailsCard({details}){
    
    return(
        <>
        <Typography level="h3">Last run details: </Typography>
            Track: {details.track} <br />
            Batch size: {details.batchSize} <br />
            Session Time: {details.sessionTime} Sec <br />
            Sessions\lap: {details.sessions} <br />
        </>
    );
}