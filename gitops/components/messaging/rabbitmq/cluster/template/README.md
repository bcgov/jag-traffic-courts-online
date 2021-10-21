## Templates to create/update all required components to set-up RabbitMq Cluster in Opennshift


### Command to execute template to create/update Service account, role & role bindings for RabbitMq cluster peer discovery & to pull image from tools namespace.
#### Contact Openshift project admin if you don't have permissions to create/update roles & role bindings.
1) Login to OC using login command
2) Run below command in each env. namespace dev/test/prod/tools
   ``oc process -f rabbitmq_cluster_rbac.yaml --param-file=rabbitmq_cluster_rbac.env | oc apply -f -``

### Command to execute template to create/update RabbitMq cluster components.
1) Login to OC using login command
2) Run below command in each env. namespace dev/test/prod/tools
   ``oc process -f rabbitmq_cluster.yml --param-file=rabbitmq_cluster.env | oc apply -f -``
