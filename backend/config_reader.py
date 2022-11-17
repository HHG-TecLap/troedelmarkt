__all__ = ("CONFIG",)

from configparser import ConfigParser as _ConfigParser
from os import path as _path

CONFIG = _ConfigParser()
# Some magic to always read the config.cfg in this file's directory
CONFIG.read(_path.join(_path.dirname(__file__),"config.cfg"))