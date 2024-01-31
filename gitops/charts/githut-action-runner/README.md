# OpenShift GitHub Actions Runners

Runner: https://github.com/redhat-actions/openshift-actions-runners

Helm Chart: https://github.com/redhat-actions/openshift-actions-runner-chart

Packaged Charts - https://github.com/redhat-actions/openshift-actions-runner-chart/tree/release-chart/packages
Is it possible to `helm pull <chart>`? Nothing obvious



```
oc project 0198bb-tools
helm install actions-runner actions-runner-2.1.tgz `
  --values values.yaml `
  --set-string githubOwner=bcgov `
  --set-string githubRepository=jag-traffic-courts-online `
  --set-string githubPat=<user-pat>
```
