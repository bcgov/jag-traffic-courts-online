# Deploy Routes

There are OpenShift route definitions in each of the [dev](./dev), [test](./test) and [prod](./prod) folders.
They can be applied individually with `oc apply -f <filename>`

# Deploy Image Streams

A copy of all image used to build or run the solution are available in the tools namespace.  An image stream manifest file
exists for each image. Each image may have one or more tags that are made available. Each tag will reside in the same file.
The [dotnet](tools/image-streams/dotnet.yaml) has the sdk and runtime images in the same file.

To update the image streams, you can apply the images streams using the `oc` command.

```
oc login ...
oc apply -f tools/image-streams/
```

# Deploy Sysdig Team

The [sysdigteam.yaml](sysdigteam.yaml) needs to created as per [Set up a team in Sysdig Monitor](https://docs.developer.gov.bc.ca/sysdig-monitor-setup-team/).

# Deploy Applications

See the [gitops chart](../../.gitops/charts/) folder.
