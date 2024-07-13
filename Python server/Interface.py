from typing import List
from rich import print as rPrint
from rich.panel import Panel

from fastapi import FastAPI
import uvicorn

from Model import ModelBase
from Routes import Routes

server = FastAPI(title="OpenRacer API", redoc_url="/docs", docs_url="/docs-old")

class Interface:
    def __init__(self, model:ModelBase, host:str="localhost", port:int=8000, debug:bool = False):
        self.host = host
        self.port = port
        self.model = model
        self.url = f"http://{host}:{port}"
        self.router = Routes(model=model)
        server.debug = debug
        server.include_router(self.router.communicationRoutes)
        
    def start(self):
        """ Start the server. """
        self.printStart(intro=["Welcome to OperRacer", f"[link={self.url}]Home Page: {self.url}[/link]"])
        uvicorn.run("Interface:server", host=self.host, port=self.port)
        
    def printStart(self, intro:List[str]=["Welcome to OperRacer"], padding:int = 5):
        """
        Print startup Box to welcome and show links

        Args:
            intro (List[str], optional): Lines to be shown in startup panel. Defaults to ["Welcome to OperRacer"].
            padding (int, optional): use to decied size of panel. Defaults to 5.
        """
        maxLength = len(max(intro, key = lambda x: len(x))) + padding*2
        rPrint(Panel(intro[1], title=intro[0], width=maxLength))
        

    