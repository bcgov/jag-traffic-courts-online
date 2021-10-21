## Templates to create redis with persistance using single instance (not cluster)

### OC command to import the redis image from external redhat image registry into openshift namespace image registry (This requird only during first time import to namespace)
1) Login to OC using login command and switch to tools namespace

``oc import-image rhel8/redis-6:1-21 --from=registry.redhat.io/rhel8/redis-6:1-21 --confirm``


### Command to execute template
1) Login to OC using login command
2) Change the values in .env file as per required env.
3) Run below command in each env. namespace dev/test/prod
   ``oc process -f tco_redis_template.yaml --param-file=tco_redis.env | oc apply -f -``
   

### Note: The redis password present in .env file (REDIS_PASSWORD) is dummy value. It has to be changed in secret after deployment with correct password.

