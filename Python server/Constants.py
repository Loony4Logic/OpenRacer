from enum import Enum


class COMMAND(str, Enum):
    Track = "track"
    TrackAck = "trackAck"
    Details = "details"
    Epoch = "epoch"
    Eval = "eval"
    Test = "test"
    Lap = "lap" 
    End = "end"

ACK = "ack"