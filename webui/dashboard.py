"""
Package dedicated to defining the main page of the Web UI, which can be imported through the `bp` :class:`Blueprint`.
"""

from flask import Blueprint, Response

bp = Blueprint('Dashboard', __name__, url_prefix='/')


@bp.route('/')
def main() -> Response:
    """
    This functions sets up the main page of the Web UI.

    :return: :class:`Response` object containing the main page HTML.
    """
    return Response('Hey')
