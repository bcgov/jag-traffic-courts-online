## Run automation test suite using Cypress 

1. To run the Cypress tests, navigate to citizen portal folder and run one of the following commands:

`./node_modules/.bin/cypress run --project e2e` OR 
`npm run cypress`

The above commands will run the tests using inbuilt electron browser provided by cypress package.

2. To run the tests using chrome instead of electron browser, run following command:

`./node_modules/.bin/cypress run --project e2e --headed --browser chrome`

3. To open the Cypress Test Runner and run the tests using interactive window, run one of the followings commands:

`./node_modules/.bin/cypress open --project e2e` OR
`npm run cypress-open`

4. Alternatively, npx can be installed local machine using `npm install -g npx` and Cypress tests can be run using following command:

`npx cypress run --project e2e --headed`

