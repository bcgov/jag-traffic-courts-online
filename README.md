[![Maintainability](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/maintainability)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/test_coverage)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/test_coverage)

# jag-traffic-courts-virtualization

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
