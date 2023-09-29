"""
Entry point for the assistant chatbot.
"""

import logging as log
from argparse import ArgumentParser, Namespace
from importlib.metadata import metadata, version

from .terminal_input import InputReader
from webui.server import UIServer

log.basicConfig(level=0)


def run(port: int, use_ui: bool) -> None:
    """
    Initializes and runs the assistant chatbot until either the program is interrupted or until the exit command
    is given.

    :param port: Network port from which a Web UI would is served, if there is one.
    :param use_ui: Enables UI if set to `True`.
    """
    ir: InputReader = InputReader()
    ir.start()

    if use_ui:
        try:
            ui: UIServer = UIServer(port=port, load_dotenv=True)
            ui.start()
        except EnvironmentError as ex:
            log.exception(ex)
            print('Could not launch Web UI server, resuming without UI.')

    ir.join()
    print('Exit successful.')


def main() -> None:
    """
    Entry point to the assistant chatbot, parses args from the terminal and starts the bot.
    """
    parser: ArgumentParser = ArgumentParser('intelligence', description=metadata('intelligence-bot')['summary'],
                                            epilog='run using poetry: poetry run %(prog)s [args ...]')
    parser.add_argument('-p', '--port', action='store', type=int, default=5000, dest='port',
                        help='the network port from which a Web UI may be served')
    parser.add_argument('-noui', action='store_false', dest='use_ui',
                        help='launch chatbot without any UI')
    parser.add_argument('--version', action='version', version=f'%(prog)s {version("intelligence-bot")}')
    args: Namespace = parser.parse_args()
    assert args.port > -1, 'Port must be a positive integer value.'
    assert args.port < (1 << 16), 'Port number must be less than 65536 (max 16-bit unsigned integer).'
    run(args.port, args.use_ui)
