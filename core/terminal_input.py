"""
Simple package defining the :class:`InputReader` class.
"""

import threading
import logging as log


class InputReader(threading.Thread):
    """
    Class representing a thread which continually captures and submits input to the command queue.
    When the captured input equals `'exit'`, the thread ends its execution.
    """

    def __init__(self):
        """
        Creates a new input reading thread.
        """
        super(InputReader, self).__init__(name='Input reading thread')

    def run(self) -> None:
        """
        Reads the input in a loop and submits it to bot the command queue, until said input equals `'exit'`.
        """
        while True:
            in_str: str = input()
            log.info(f"TERMINAL: {in_str}")
            if in_str == 'exit':
                break
