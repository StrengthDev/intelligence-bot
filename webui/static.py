"""
Package dedicated to defining the getters for static files, which can be imported through the `bp` :class:`Blueprint`.
"""

from os import path
from flask import Blueprint, Response, send_from_directory

bp = Blueprint('Static files', __name__, url_prefix='/')


@bp.route('/favicon.ico')
def favicon() -> Response:
    """
    Getter for the favicon.ico file.

    :return: :class:`Response` object containing the file data.
    """
    return send_from_directory(path.join(bp.root_path, 'static'),
                               'favicon.ico', mimetype='image/vnd.microsoft.icon')
