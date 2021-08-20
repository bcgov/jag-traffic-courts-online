## RabbitMQ Single instance deploy spec

### Prerequisite

1. Have openshift credentials and have read/write permission
2. Run the following commands under \gitops\components\messaging\rabbitmq folder.

### Steps

1. login openshift

```
oc login --token=yourtoken --server=https://api.silver.devops.gov.bc.ca:6443
```

2. go to the correct project

```
oc project 0198bb-dev
```

3. create rabbitmq-config config map

```
oc create -f config-map.yaml --save-config
```

> Note: in config-map.yaml, we remove rabbitmq_peer_discovery_k8s (which is default enabled) from enabled_plugins, as we only have 1 rabbitMQ instance. For details, please refer to [here](https://github.com/bitnami/charts/issues/2227).
> We also add shovel plugin for error processing. For details, please refer to [here](https://www.rabbitmq.com/shovel.html).

4. create rabbitmq secrets, which include Erlang-cookie, default username and password

```
oc create -f secrets.yaml --save-config
```

5. create secrets for docker access (dockerhub credentials for pull in dockerhub images)

```
oc create secret docker-registry docker-creds ^
--docker-server=docker.io ^
--docker-username=bcgovfams3 ^
--docker-password="@&W9H0e#eLlHv5&Z@pR1t" ^
--docker-email=unused
```

> Why we need this?
> When openshift try to pull rabbitmq image from dockerhub, it quite often get "hitting the docker rate-limit issue".
> The solution is found [here](https://github.com/BCDevOps/OpenShift4-Migration/issues/51).

We will Sign up docker account for otc team.\*

6. Link Secret for Build & Pulls (refer to [here](https://github.com/BCDevOps/OpenShift4-Migration/issues/51))

```
oc secrets link default docker-creds --for=pull
```

> Note: as the user openshift role and gov policy, we cannot link secret successfully. If this step failed, we can skip this step, and we will work around it later.

7. create load balance service

```
oc create -f service.yaml --save-config
```

8. create headless service

```
oc create -f service2.yaml --save-config
```

9. Create StatefulSet

```
oc create -f statefulset.yaml --save-config
```

> Note: in statefulset.yaml, we add
>
> ```
> imagePullSecrets:
> - name: docker-creds
> ```
>
> because step 6 failed. If step 6 succeeds, we do not need to add imagePullSecrets

10. expose the rabbitmq management web console

```
oc expose service rabbitmq-cluster-balancer
```

> reference command:

```
oc delete configmap rabbitmq-config
oc delete secrets rabbitmq-secret
oc delete service rabbitmq-cluster
```
