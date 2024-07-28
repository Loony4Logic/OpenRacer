import { Typography } from "@mui/joy";
import { useEffect, useState } from "react";
import { API_PATH } from "./Constants";

export function DetailsCard({details}){
    
    return(
        <>
        <Typography level="h3">Details: </Typography>
            Track: {details.track} <br />
            Batch size: {details.batchSize} <br />
            session Time: {details.sessionTime} Sec <br />
            Sessions: {details.sessions} <br />
        </>
    );
}