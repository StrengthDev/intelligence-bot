"""
This script is meant to be run once after the assistant chatbot repository is first cloned, to generate default
configuration files for the bot.
"""

from os import path, makedirs
from typing import TextIO
from argparse import ArgumentParser, Namespace


default_token: str = 'thisisadefaulttokenstringpleasereplacemewithanactualtoken'

default_config: str = '''[Links]
source=https://github.com/StrengthDev/intelligence-bot

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

default_cmd_config: str = '''[General]

[Power]
tail=»
body=-
head=>
mod=10
'''

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


def run() -> None:
    """
    Creates a `config` folder inside the project directory if one doesn't already exist, and inside the folder
    generates default configuration files necessary for the execution of the assistant chatbot.
    """
    project_dir: str = path.join(path.curdir, path.pardir)
    config_dir: str = path.join(project_dir, 'config')

    if not path.exists(config_dir):
        makedirs(config_dir)
        print('Created config directory.')

    overwrite: bool = False

    print('Writing default configuration files..')

    write_config(config_dir, 'TOKEN', default_token, overwrite)
    write_config(config_dir, 'config.ini', default_config, overwrite)
    write_config(config_dir, 'commands.ini', default_cmd_config, overwrite)
    write_config(config_dir, 'ask.txt', default_ask, overwrite)

    print(f'Files written to: "{path.abspath(config_dir)}"\n')
    print('Please do not forget to replace the contents of the TOKEN file with your own '
          'Discord bot token, if you haven\'t already.')


def main() -> None:
    """
    Entry point fot the default config generator, parses arguments from the terminal and starts the generation.
    """
    run()


if __name__ == '__main__':
    main()
