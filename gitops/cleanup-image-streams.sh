#!/usr/bin/env bash
# loop over each image stream
for IS in arc-dispute-api citizen-api citizen-web oracle-data-api staff-api staff-web workflow-service; do
  # get all the tags for the current image stream
  TAGS=`oc get is ${IS} --template='{{range .status.tags}}{{" "}}{{.tag}}{{end}}{{"\n"}}'`

  for tag in $TAGS; do     # do no delete current production tags, or the test tags and their alias
    if [[ "$tag" != "2.6.7" && ! "$tag" =~ ^513c8cd && "$tag" != "tcvp-3144-alpha.1" && ! "$tag" =~ ^f8eaeaa ]]; then
      echo "oc tag ${IS}:$tag -d"
    fi
  done
done
