from enum import Enum


class COMMAND(str, Enum):
    Track = "track"
    TrackAck = "trackAck"
    Details = "details"
    Epoch = "epoch"
    Eval = "eval"
    End = "end"

ACK = "ack"