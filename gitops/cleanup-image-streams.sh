#!/usr/bin/env bash
# loop over each image stream
for IS in arc-dispute-api citizen-api citizen-web oracle-data-api staff-api staff-web workflow-service; do
  # get all the tags for the current image stream
  TAGS=`oc get is ${IS} --template='{{range .status.tags}}{{" "}}{{.tag}}{{end}}{{"\n"}}'`

  for tag in $TAGS; do
    # do no delete current production tags, or the test tags and their alias
    if [[ "$tag" != "v1.0.0" && "$tag" != "v1.0.1" && "$tag" != "1.66.1" && "$tag" != "93b7e94*" ]]; then
      echo "oc tag ${IS}:$tag -d"
    fi
  done
done
