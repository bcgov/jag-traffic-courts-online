
# Go to maintenance mode

## Scale maintenance-site up to 1 instance

```
oc project 0198bb-test
oc scale deployment maintenance-site --replicas=1
```

## Change Route
```
oc apply -f citizen-web-for-maintenance.yaml
```

## Verify Site

[test.tickets.gov.bc.ca](https://test.tickets.gov.bc.ca/)


# Leave maintenance mode

Verify new site is up and running.

# Change Route

```
oc project 0198bb-test
oc apply -f citizen-web-for-justice-proxy.yaml
```

## Scale maintenance-site down to 0 instances to save resources

```
oc scale deployment maintenance-site --replicas=0
```
