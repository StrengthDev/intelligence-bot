"""
This script is meant to be run once after the assistant chatbot repository is first cloned, to generate default
configuration files for the bot.
"""

from os import path, makedirs
from typing import TextIO, List
from argparse import ArgumentParser, Namespace
from importlib.metadata import metadata, version


token_file: str = '.token'
default_token: str = 'thisisadefaulttokenstringpleasereplacemewithanactualtoken'

config_file: str = 'config.ini'
default_config: str = f'''[Links]
source={metadata('intelligence-bot')['project-url'].partition('Repository, ')[2]}

[Startup]
status=
status_type=4
; 0 => Playing
; 1 => Streaming
; 2 => Listening to
; 3 => Watching
; 4 => 

[Options]
'''

cmd_config_file: str = 'commands.ini'
default_cmd_config: str = '''[General]

[Power]
tail=D
body=-
head=>
mod=10
'''

ask_file: str = 'ask.txt'
default_ask: str = '''50:Yes
50:No
1:Maybe
'''


def write_config(directory: str, filename: str, contents: str, overwrite: bool) -> None:
    """
    Writes `contents` to the file, referenced by `filename`, inside `directory`. If the file does not exist,
    a new one is created. If file already exists and `overwrite` is set to `True`, the file is overwritten,
    otherwise nothing happens.

    :param directory: The directory in which the file resides.
    :param filename: The name of the file to be written to.
    :param contents: The contents which will be written to the file.
    :param overwrite: Whether to overwrite the file, if it already exists.
    """
    full_path: str = path.join(directory, filename)

    def write_file(filepath: str, data: str) -> None:
        """
        Small helper function to write to a file.

        :param filepath: The full path to the file.
        :param data: The data to be written to the file.
        """
        f: TextIO = open(filepath, 'w')
        f.write(data)
        f.close()

    if path.isfile(full_path):
        if overwrite:
            print(f'Overwriting {filename} with default values..')
            write_file(full_path, contents)

        else:
            print(f'{filename} already exists, skipping..')

    else:
        print(f'Generating new {filename} file..')
        write_file(full_path, contents)


def run(token: str, overwrite: bool, skip: List[str]) -> None:
    """
    Creates a `config` folder inside the project directory if one doesn't already exist, and inside the folder
    generates default configuration files necessary for the execution of the assistant chatbot.

    :param token: The contents of the token file.
    :param overwrite: If `True`, existing files are overwritten, otherwise they are skipped.
    :param skip: List of files to be skipped.
    """
    project_dir: str = path.join(path.curdir)
    config_dir: str = path.join(project_dir, 'config')

    if not path.exists(config_dir):
        makedirs(config_dir)
        print('Created config directory.')

    print('Writing default configuration files..')

    if token_file not in skip:
        write_config(config_dir, token_file, token, overwrite)
    else:
        print(f'Skipping {token_file}..')

    if config_file not in skip:
        write_config(config_dir, config_file, default_config, overwrite)
    else:
        print(f'Skipping {config_file}..')

    if cmd_config_file not in skip:
        write_config(config_dir, cmd_config_file, default_cmd_config, overwrite)
    else:
        print(f'Skipping {cmd_config_file}..')

    if ask_file not in skip:
        write_config(config_dir, ask_file, default_ask, overwrite)
    else:
        print(f'Skipping {ask_file}..')

    print(f'Files written to: "{path.abspath(config_dir)}"\n')
    print(f'Please do not forget to replace the contents of the {token_file} file with your own '
          'Discord bot token, if you haven\'t already.')


def main() -> None:
    """
    Entry point fot the default config generator, parses arguments from the terminal and starts the generation.
    """
    parser: ArgumentParser = ArgumentParser('generate_config',
                                            description='Generate configuration files with default values',
                                            epilog='run using poetry: poetry run %(prog)s [args ...]')
    parser.add_argument('-t', '--token', action='store', type=str, default=default_token, dest='token',
                        help='contents of the token file')
    parser.add_argument('-ow', '--overwrite', action='store_true', dest='overwrite',
                        help='overwrite existing files')
    parser.add_argument('-s', '--skip', action='extend', nargs='+', type=str, dest='skip',
                        choices=[token_file, config_file, cmd_config_file, ask_file],
                        default=[],
                        metavar='file', help='files to skip/ignore')
    parser.add_argument('--version', action='version', version=f'%(prog)s {version("intelligence-bot")}')
    args: Namespace = parser.parse_args()
    run(args.token, args.overwrite, args.skip)
