import threading
from ipaddress import ip_address
from socket import gethostname, gethostbyname
from typing import Mapping
from flask import Flask

from . import dashboard
from . import static


class UIServer(threading.Thread):
    """
    This class represents a Web UI server thread running in the background.
    """

    def __init__(self,
                 port: int | None = None,
                 load_dotenv: bool = True,
                 options: Mapping[str, any] | None = None):
        """
        Creates a new instance of the Web UI server.

        :param port: The network port which the server will be listening to. Defaults to `5000`.
        :param load_dotenv: If `True` (default), load the nearest .env and .flaskenv files to set environment variables.
            Will also change the working directory to the directory containing the first file found.
        :param options: The options to be forwarded to the underlying Werkzeug server. See werkzeug.serving.run_simple
            for more information.

        :raise EnvironmentError: If the local machine is not connected to a private network, as the Web UI
            server is not meant to be accessible to anyone directly through the internet.
        """
        # There's no way to make Flask stop, so must launch daemonic thread, which gets killed on exit
        super(UIServer, self).__init__(name='UIServer thread', daemon=True)
        self.app: Flask = Flask(__name__)
        self.port: int = port if port is not None else 5000
        self.debug: bool = False    # only works in main thread
        self.load_dotenv: bool = load_dotenv
        self.options: Mapping[str, any] = options if options is not None else {}
        self.ip: str = gethostbyname(gethostname())
        if not ip_address(self.ip).is_private:
            raise EnvironmentError('Local machine is not in private network, the Web UI is not meant to be '
                                   'accessible through the internet.')

        self.app.register_blueprint(static.bp)
        self.app.register_blueprint(dashboard.bp)

    def run(self) -> None:
        """
        Main UIServer thread function, runs a loop to serve the Web UI.
        """
        self.app.run(self.ip, self.port, self.debug, self.load_dotenv, **self.options)
        print('UIServer successfully exited')
