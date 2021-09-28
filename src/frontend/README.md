# Front end source code - citizens and partner portal

This directory contains application source code for the citizens and partner portal.

## Project Structure

```bash
├── citizen-portal                        # Contains citizen portal source code.
```

## Apps

Name           | Description    | Doc
-------------- | -------------- | ----------------------------------
citizen-portal | citizen portal | [README](citizen-portal/README.md)

1.) Set environment variables(.env)file using template shown in `.env.template`

2.) Running application locally enter following command in Bash:

Pre-requisites:

- Make sure you have [Yarn](https://classic.yarnpkg.com/en/docs/getting-started/) and [NPM](https://docs.npmjs.com/getting-started) CLI working

After fullfiling all the pre-requisites enter the command:

```bash
yarn install
```

then

```bash
yarn start
```

3.) Running application services via docker:

Pre-requisites:

- Install Docker Hub on Desktop
- Install Docker CLI
- Verify Docker Daemon is up and running in the backend

After fullfiling all the pre-requisites enter the command:

```bash
docker-compose up --build
```

4.) Upon successful completion trying launching each of the services in the browser. There might be some problems opening the front end service with ZScaler. Try Enabling/Disabling. Also recommended browser to test is Microsoft Edge. There may be difficulties in Google Chrome.

5.) If (there is error ) An unhandled exception occurred: Call retries were exceeded See `/tmp/ng-5vKGPg/angular-errors.log` for further details. error Command failed with exit code 127.

Increase Docker memory from Docker Hub
