#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM docker.io/maven:3.8.3-jdk-11 as build

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
FROM eclipse-temurin:11
WORKDIR /app
EXPOSE 8080

ENV TZ=America/Vancouver

COPY --from=build ./target/oracle-data-api.jar .
COPY --from=build ./entrypoint.sh .
RUN chgrp 0 entrypoint.sh && chmod a+rwx,o-rwx entrypoint.sh
# use CMD "shell form" so we can expand env vars
CMD ./entrypoint.sh java $JAVA_OPTS -jar /app/oracle-data-api.jar
#############################################################################################
