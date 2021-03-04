[![Maintainability](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/maintainability)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/test_coverage)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/test_coverage)

[![img](https://img.shields.io/badge/Lifecycle-Experimental-339999)](https://github.com/bcgov/repomountie/blob/master/doc/lifecycle-badges.md)  ![Cucumber Tests](https://github.com/bcgov/jag-traffic-courts-online/workflows/Cucumber%20Tests/badge.svg)

# jag-traffic-courts-online

Welcome to Traffic Courts Online

## Project Structure

    ├── .github                                 # Contains GitHub Related sources
    ├── infrastructure                          # openshift templates and pipeline
    ├── docs                                    # docs and images
    ├── src                                     # application source files
    |   ├── backend                             # backend apis
    |   |   └── DisputeApi.Web                  # mock dispute api
    │   └── frontend                            # frontend applications
    │       └── citizen-portal                  # citizen poral
    ├── COMPLIANCE.yaml                         #
    ├── CONTRIBUTING.md                         #
    ├── LICENSE                                 # Apache License
    └── README.md                               # This file.

## Apps

| Name                | Description                                  | Doc                             |
| ------------------- | -------------------------------------------- | --------------------------------|
| backend             | all server side services                     |                                 |
| disute api          | base dispute api                             | [README](src/backend/README.md)|
| frontend            | all client side applications                 | [README](src/frontend/README.md)|


# Splunk Docker Examples

https://splunk.github.io/docker-splunk/EXAMPLES.html#create-standalone-with-hec

# Run docker-compose

Copy the `.env.template` to `.env` and then run docker-compose up.
Add the configuration for token and password for splunk.
Default user is `admin`. Password is what is configured in `.env`

```
docker-compose up
```

To remove services run (all services and networking)
```
docker-compose down
```