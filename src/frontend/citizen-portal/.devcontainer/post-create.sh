#!/bin/bash

#
# This script is after the devcontainer is created
#
npm install -g npm

# install angular/cli globally so 'ng' commands work
# TODO: figure out how to pull the version from package.json
NG_CLI_ANALYTICS=ci npm install -g @angular/cli@11.0.6
NG_CLI_ANALYTICS=ci npm install
