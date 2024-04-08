
# Go to maintenance mode

## Scale maintenance-site up to 1 instance

```
oc project 0198bb-dev
oc scale deployment maintenance-site --replicas=1
```

## Change Route
```
oc apply -f citizen-web-for-maintenance.yaml
```

## Verify Site

[dev.tickets.gov.bc.ca](https://dev.tickets.gov.bc.ca/)


# Leave maintenance mode

Verify new site is up and running.

# Change Route

```
oc project 0198bb-dev
oc apply -f citizen-web-for-justice-proxy.yaml
```

## Scale maintenance-site down to 0 instances to save resources

```
oc scale deployment maintenance-site --replicas=0
```
