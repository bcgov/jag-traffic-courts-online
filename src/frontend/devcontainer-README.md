
# Dev Containers

Each front end project has devcontainer support.  You can use VS Code and open the folder in a container.
The container will have all the build tools required to make work with the angular applications.

## Features
* angular cli
* java
* git

## Extensions
* Angular Language Service (Angular.ng-template)
* Better Comments (aaron-bond.better-comments)
* Error Lens (usernamehw.errorlens)
* ESLint (dbaeumer.vscode-eslint)
* others installed part of the features above

## Reference 

https://code.visualstudio.com/docs/devcontainers/create-dev-container


## Tips

* Checkout the front end code on a native WSL volume. The build speed will be significantly faster.

  1. Clone the repository on the WSL volume
  2. From the appropriate directory, run "code ."
  3. In VS Code, choose "Reopen in container"