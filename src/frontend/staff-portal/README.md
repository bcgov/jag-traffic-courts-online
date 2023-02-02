# Citizen Portal

## Installation and Configuration

The installation and configuration of the Traffic Court Online development environment is sequentially ordered to ensure software dependencies are available when needed during setup.

### Installation

The following list includes the required software needed to run the application, as well as, the suggested IDE with extensions for web client development, and software for source control management and API development/testing.

#### Git and GitKraken

[Download](https://git-scm.com/downloads) and install the Git version control system, and optionally [download](https://www.gitkraken.com) and install the free GitKraken Git GUI client.

Clone the Traffic Court Online repository into a project directory GitKraken or the terminal by typing:

```bash
git clone https://github.com/bcgov/jag-traffic-courts-online
```

#### Node

[Download](https://nodejs.org/en/) and install **Node v14.x**
NOTE: ensure to use v14.x. More recent versions (i.e. v17.x) do not work.
It's useful to install different versions of node (https://docs.microsoft.com/en-us/windows/dev-environment/javascript/nodejs-on-windows)
For this project, before building any code perform a:
```bash
nvm install 14.19.0
nvm use 14.19.0
```

#### Angular cli

Install Angular cli using command:

```bash
yarn add @angular/cli
```

#### VS Code

[Download](https://code.visualstudio.com/) and install VSCode and accept the prompt to install the recommended extensions when the PRIME repository is initially opened in VSCode.

#### PostMan

[Download](https://www.getpostman.com/apps) and install the Postman HTTP client.

## Building and Running

### Dev steps to run locally
1. yarn install 
2. yarn run api:clean (will clear out src/app/api folder)
3. yarn run api:download (will pull the latest swagger.json file from the staff-api)
3. yarn run api:generate (will regenerate the API based off the swagger.json file)
5. yarn start

### Client

To build, run, and open the Angular application in the default browser at <http://localhost:4200> for development go to the Citizen Portal project repository in the terminal and type:

```bash
yarn start
```

To test the production build locally before pushing features to the repository for deployment type:

```bash
ng build --prod
```

#### Angular CLI Reference

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 10.1.x. Refer to the Angular CLI documentation for the available commands, but the most used commands during development will be:

1. `yarn install` to install any packages that the application depends on.
2. `ng serve -o` to serve your application locally in memory during development at `http://localhost:4200` through the default browser, which watches for changes, recompiles, and automatically refresh the application in the browser
3. `ng build` to build the application, which is stored in `/dist` directory. Use the `--prod` flag for a production build, which significantly decreases the size of the application
4. `ng g <blueprint>` to create code scaffolding for a directive, pipe, service, class, guard, interface, enum, and module
5. `ng lint` to lint the application code using TSLint.
6. `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).
7. `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

##### Getting Help

1. To get more help on the Angular CLI use `ng help`
2. `ng doc component` to look up documentation for features
3. `ng serve --help` to look up doc for `ng serve` command

## Coding Styles

Coding styles should adhere to the [Angular Style Guide](https://angular.io/docs/ts/latest/guide/style-guide.html) at all times! The editor config setup for the project will also assist with coding style automatically, as well as VSCode settings.
