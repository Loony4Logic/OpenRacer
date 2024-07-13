import os
import sqlite3

class Recorder:
    createInputTable = """
    CREATE TABLE step(
        all_wheels_on_track BOOLEAN CHECK(all_wheels_on_track IN(0, 1)),
        x float,
        y float,
        closest_waypoint1 int,
        closest_waypoint2 int,
        distance_from_center float,
        is_crashed BOOLEAN CHECK(is_crashed IN(0, 1)),
        is_left_of_center BOOLEAN CHECK(is_left_of_center IN(0, 1)),
        is_reversed BOOLEAN CHECK(is_reversed IN(0, 1)),
        progress float,
        speed float,
        steering_angle float,
        steps int,
        track_length float,
        track_width float,
        actionX float, 
        actionY float,
        reward float, 
        agentId int,
        session int);
        """
    def __init__(self):
        os.remove("session.db")
        self.con = sqlite3.connect("session.db")
        print("Initialized a recorder")
        self.cur = self.con.cursor()
        print("Creating table")
        self.cur.execute(self.createInputTable)

        
    def record(self, formatedInput, processedInput, action, reward, session):
        data = []
        for agentId in range(len(formatedInput)):
            values = []
            for i in formatedInput[agentId].values():
                d = i
                if type(d) == bool: 
                    d = 1 if d else 0
                elif type(d) == list:
                    values.append(d[0])
                    d = d[1]
                values.append(d)
            values.append(action[agentId][0])
            values.append(action[agentId][1])
            values.append(reward[agentId])
            values.append(agentId)
            values.append(session)
            data.append(values) 
        self.cur.executemany("INSERT INTO step VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)", data)
        self.con.commit()
    # TODO: record each steps (input, action, reward, loss)
    # TODO: Seperate tables based on data to make it more space efficient