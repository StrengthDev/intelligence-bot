import os
from flask import Blueprint, Response, send_from_directory

bp = Blueprint('Static files', __name__, url_prefix='/')


@bp.route('/favicon.ico')
def favicon() -> Response:
    """
    Getter for the favicon.ico file.

    :return: :class:`Response` object containing the file data.
    """
    return send_from_directory(os.path.join(bp.root_path, 'static'),
                               'favicon.ico', mimetype='image/vnd.microsoft.icon')
