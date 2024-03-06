#!/bin/bash

# Install the Angular CLI autocompletion to the user's profile
grep -q "Angular" $HOME/.bashrc 2> /dev/null
if [ $? -ne 0 ]
then
  echo "# Load Angular CLI autocompletion." >> $HOME/.bashrc
  echo "source <(ng completion script)" >> $HOME/.bashrc
fi

yarn install