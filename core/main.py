import logging as log

from terminal_input import InputReader
from webui.server import UIServer

log.basicConfig(level=0)


def main() -> None:
    ir: InputReader = InputReader()
    ir.start()

    try:
        ui: UIServer = UIServer(port=5000, load_dotenv=True)
        ui.start()
    except EnvironmentError as ex:
        log.exception(ex)
        print('Could not launch Web UI server, resuming without UI.')

    ir.join()
    print('Exit successful.')


if __name__ == '__main__':
    main()
