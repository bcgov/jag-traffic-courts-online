# Infrastructure
Generating kubernetes yaml file using docker-compose.yaml to launch and deploy the project inside the OC4
- Using Kompose kubernetes tool
 - Install Kompose and add your directory location into your environment  settings PATH variable.
 - Follow this link on how to install Kompose tool https://kompose.io/
 - Inside the terminal go to your docker-compose.yaml location of the project
 - Run command "kompose convert --provider=openshift --build=build-config --build-repo=https://github.com/bcgov/jag-traffic-courts-online.git" this will convert all the service inside the docker-compose.yaml to kubernetes yaml format and will generate build configs
 - After that Run command "kompose convert --provider=openshift --build=deploy-config --build-repo=https://github.com/bcgov/jag-traffic-courts-online.git" this will convert all the service inside the docker-compose.yaml to kubernetes yaml format and will generate deployment configs
 - Yaml Files will be created at the location from where the command is ran. 
 

1.) Deploying Dispute-api in the OC4 Environment using GUI of OC4
    - dispute-api-deploymentconfig.yaml
     This is used to generate the deployment configs for the dispute api inside the OC4 environment.
     Copy the content of insfrastructure/openshift/dispute-api-deploymentconfig.yaml into deployment config of the dev namespace of traffic courts project in OC4
    - dispute-api-imagestream.yaml
     This file is used to generate and save the Dotnet core image inside the OC4 repository
     Copy the content of insfrastructure/openshift/dispute-api-imagestream.yaml   yaml into the imagestream of the tools namespace of the traffics courts online in OC4
    -dispute-api-service.yaml
     This file is to generate the service which can define the ports for the backend api
     Copy the content of insfrastructure/openshift/dispute-api-service.yaml into the service of dev namespace of the traffic courts online projects in OC4

2.) Deploying Dispute-api in the OC4 Environment using CLI tool OC
    - For installing OC in your system follow this link https://docs.openshift.com/container-platform/4.2/cli_reference/openshift_cli/getting-started-cli.html#cli-installing-cli_cli-developer-commands

    - Login in the OC4 using your login token "oc login --token=${LOGIN_TOKEN} --server=https://api.silver.devops.gov.bc.ca:6443"

    - Select the appropriate project and its namespace using comman "oc project ${LICENSE_PLATE-NAMESPACE}" i.e. oc project 0198bb-dev

    - After selecting correct project and namespace run command "oc process -f ${YAML_FILE_NAME} | oc create -f -