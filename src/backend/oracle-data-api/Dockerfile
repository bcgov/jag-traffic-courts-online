ARG BUILD_IMAGE=docker.io/maven
ARG RUNTIME_IMAGE=eclipse-temurin

#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM $BUILD_IMAGE:3.8.6-jdk-11 as build

ARG SKIP_TESTS=true

WORKDIR /

# Cache .m2 artifacts for subsequent builds.
COPY pom.xml .
RUN mvn verify clean --fail-never

# Compile codebase to a single jar
COPY . .
RUN mvn -B clean package \
		-Dtarget.fileName=oracle-data-api \
        -Dmaven.test.skip=${SKIP_TESTS} 

#############################################################################################

#############################################################################################
### Stage where Docker is running a java process to run a service built in previous stage ###
#############################################################################################
FROM $RUNTIME_IMAGE:11
ARG PROMETHEUS_AGENT_VERSION=0.17.2

# Optional Git Version arguments
ARG GIT_COMMIT_DATE=not-set
ARG GIT_SHA=not-set
ARG GIT_SHORT_SHA=not-set
ARG GIT_VERSION=not-set

LABEL git.commit.date="$GIT_COMMIT_DATE" \
      git.sha="$GIT_SHA" \
      git.sha.short="$GIT_SHORT_SHA" \
      git.version="$GIT_VERSION"

ENV APP_VERSION=$GIT_VERSION
ENV GIT_SHA=$GIT_SHA

WORKDIR /app
EXPOSE 8080

ADD https://repo1.maven.org/maven2/io/prometheus/jmx/jmx_prometheus_javaagent/${PROMETHEUS_AGENT_VERSION}/jmx_prometheus_javaagent-${PROMETHEUS_AGENT_VERSION}.jar /app/jmx_prometheus_javaagent.jar
RUN printf "Generate jmx_prometheus_javaagent_config.yaml\n" & \
 echo "startDelaySeconds: 0 \n\
ssl: false \n\
lowercaseOutputName: false \n\
lowercaseOutputLabelNames: false \n\
" > /app/jmx_prometheus_javaagent_config.yaml

# permissions - are these correct?
# a+rw - all users rw
# o-rw - other users remove rw
RUN chgrp 0 /app/jmx_prometheus_javaagent.jar         && chmod a+rw,o-rw /app/jmx_prometheus_javaagent.jar
RUN chgrp 0 /app/jmx_prometheus_javaagent_config.yaml && chmod a+rw,o-rw /app/jmx_prometheus_javaagent_config.yaml

ENV TZ=America/Vancouver

COPY --from=build ./target/oracle-data-api.jar .
COPY --from=build ./entrypoint.sh .
RUN chgrp 0 entrypoint.sh && chmod a+rwx,o-rwx entrypoint.sh
# use CMD "shell form" so we can expand env vars
CMD ./entrypoint.sh java -javaagent:/app/jmx_prometheus_javaagent.jar=9090:/app/jmx_prometheus_javaagent_config.yaml $JAVA_OPTS -jar /app/oracle-data-api.jar
#############################################################################################
