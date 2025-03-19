#!/usr/bin/env bash
if [ $# -eq 0 ]; then
  PARAMS="up -d"
else
    PARAMS=$@
fi

docker compose \
 -f docker-compose.yml \
 -f ./.docker/docker-compose.splunk.yml \
 -f ./.docker/docker-compose.jaeger.yml \
 $PARAMS
