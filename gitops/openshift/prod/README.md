
# Go to maintenance mode

## Scale maintenance-site up to 1 instance

```
oc project 0198bb-prod
oc scale deployment maintenance-site --replicas=3
```

Wait for instances to be ready

## Change Route
```
oc apply -f citizen-web-for-maintenance.yaml
```

## Verify Site

[tickets.gov.bc.ca](https://tickets.gov.bc.ca/)


# Leave maintenance mode

Verify new site is up and running.

# Change Route

```
oc project 0198bb-prod
oc apply -f citizen-web-for-justice-proxy.yaml
```

## Scale maintenance-site down to 0 instances to save resources

```
oc scale deployment maintenance-site --replicas=0
```
