# Infrastructure
Generating kubernetes yaml file using docker-compose.yaml to launch and deploy the project inside the OC4
- Using Kompose kubernetes tool
 - Install Kompose and add your directory location into your environment  settings PATH variable.
 - Inside the terminal go to your docker-compose.yaml location of the project
 - Run command "kompose convert --provider=openshift --build=build-config --build-repo=https://github.com/bcgov/jag-traffic-courts-online.git" this will convert all the service inside the docker-compose.yaml to kubernetes yaml format and will generate build configs
 - After that Run command "kompose convert --provider=openshift --build=deploy-config --build-repo=https://github.com/bcgov/jag-traffic-courts-online.git" this will convert all the service inside the docker-compose.yaml to kubernetes yaml format and will generate deployment configs
 

1.) Deploying Dispute-api in the OC4 Environment
    - dispute-api-deploymentconfig.yaml
     This is used to generate the deployment configs for the dispute api inside the OC4 environment.
     Copy the content of insfrastructure/openshift/dispute-api-deploymentconfig.yaml into deployment config of the dev namespace of traffic courts project in OC4
    - dispute-api-imagestream.yaml
     This file is used to generate and save the Dotnet core image inside the OC4 repository
     Copy the content of insfrastructure/openshift/dispute-api-imagestream.yaml   yaml into the imagestream of the tools namespace of the traffics courts online in OC4
    -dispute-api-service.yaml
     This file is to generate the service which can define the ports for the backend api
     Copy the content of insfrastructure/openshift/dispute-api-service.yaml into the service of dev namespace of the traffic courts online projects in OC4