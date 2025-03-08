import datetime
import os
import sqlite3
import uuid

class Recorder:
    createInputTable = """
    CREATE TABLE if not exists step(
        timestamp DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL,
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
        session int);"""
    createDetailsTable="""
    CREATE TABLE if not exists details(
        sessionCount int,
        agentCount int,
        trackName string,
        sessionTime int
    );
        """
    def __init__(self, modelName:str):
        """setup recorder for a mode and start connection for same. 

        Args:
            modelName (str): Name of model, this will be used for directory name and filename  
        """
        debug = True
        USE_TEST_RECODER = os.getenv("USE_TEST_RECORDER")
        if not USE_TEST_RECODER or USE_TEST_RECODER == "False":
            debug = False
        
        if debug:
            dbName = "test.db"
        else:
            dbName = f"{modelName}.db"
        
        dirPath = os.path.join(os.getcwd(), "db", modelName)
        dbPath = os.path.join(dirPath, dbName)
        os.makedirs(dirPath, exist_ok=True)
        
        print(f"DB path for model: {modelName} is {dbPath}")
        self.con = sqlite3.connect(dbPath, check_same_thread=False, isolation_level=None)
        self.con.execute('pragma journal_mode=wal')
        print(f"Initialized a recorder: {dbName}")
        self.cur = self.con.cursor()
        self.ReadCur = self.con.cursor()
        
        if debug or self.con:
            return
        print("Creating table")
        self.cur.execute(self.createInputTable)
        self.cur.execute(self.createDetailsTable)

        
    def record(self, formatedInput, action, reward, session):
        data = []
        for agentId in range(len(formatedInput)):
            values = [datetime.datetime.now()]
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
        self.cur.executemany("INSERT INTO step VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)", data)
        self.con.commit()
        
    def details(self, sessionCount, agentCount, trackName, sessionTime):
        self.cur.execute("INSERT INTO details VALUES(?,?,?,?)", [sessionCount, agentCount, trackName, sessionTime])
        self.con.commit()
    
    def getRecords(self, agentId:int, session:int):
        res = self.ReadCur.execute("SELECT * FROM step WHERE agentId=? and session=?", [agentId, session])
        return res.fetchall()
         
    def getSessionRun(self, session:int):
        res = self.ReadCur.execute("SELECT * FROM step WHERE session=?", [session])
        return res.fetchall()
    
    def getAgentRun(self, agentId:int):
        res = self.ReadCur("SELECT * FROM step WHERE agentId=?", [agentId])
        return res.fetchall()

    def getProgress(self):
        res = self.ReadCur.execute("SELECT max(progress), session FROM step GROUP BY session")
        return res.fetchall()
        
    def getDetailsOf(self,attribute:str, agent:int, session:int):
        readCur = self.con.cursor()
        if attribute == "progress":
            res = readCur.execute("SELECT progress, timestamp from step where session=? and agentId=?", [session, agent])
        elif attribute == "reward":
            res = readCur.execute("SELECT reward, timestamp from step where session=? and agentId=?", [session, agent])
        elif attribute == "speed":
            res = readCur.execute("SELECT speed, timestamp from step where session=? and agentId=?", [session, agent])
        return res.fetchall()
    
    def runDetails(self):
        readCur = self.con.cursor()
        res = readCur.execute("SELECT * FROM details")
        return res.fetchone()
    # TODO: Seperate tables based on data to make it more space efficient