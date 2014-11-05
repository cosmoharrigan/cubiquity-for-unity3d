from distribute_setup import use_setuptools
use_setuptools() 

import sys
from setuptools import setup, find_packages
from requirements_utils import parse_dependency_links, parse_requirements
