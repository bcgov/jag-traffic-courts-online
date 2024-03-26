# Jaeger All-In-One

All-in-one is an executable designed for quick local testing, launches the Jaeger UI, collector, query, and agent, with an in memory storage component.

This chart is based on https://github.com/bcgov/fams3/blob/master/openshift/templates/jaeger-aio.dc.yaml

## TL;DR

```
helm install dev jaeger-aio
```

Will create resources 

```
NAME                                READY   STATUS    RESTARTS   AGE
pod/dev-jaeger-aio-f596c57b-qntk5   1/1     Running   0          96s

NAME                               TYPE        CLUSTER-IP     EXTERNAL-IP   PORT(S)                                  AGE
service/dev-jaeger-aio-agent       ClusterIP   None           <none>        5775/UDP,6831/UDP,6832/UDP,5778/TCP      96s
service/dev-jaeger-aio-collector   ClusterIP   10.217.5.136   <none>        14250/TCP,14267/TCP,14268/TCP,9411/TCP   96s
service/dev-jaeger-aio-query       ClusterIP   10.217.4.250   <none>        80/TCP                                   96s
service/dev-jaeger-aio-zipkin      ClusterIP   10.217.4.253   <none>        9411/TCP                                 96s

NAME                             READY   UP-TO-DATE   AVAILABLE   AGE
deployment.apps/dev-jaeger-aio   1/1     1            1           96s

NAME                                      DESIRED   CURRENT   READY   AGE
replicaset.apps/dev-jaeger-aio-f596c57b   1         1         1       96s
```