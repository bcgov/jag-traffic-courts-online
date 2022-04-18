#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM maven:3.8.3-jdk-11 as build

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

EXPOSE 8080

ARG SPLUNK_URL=${SPLUNK_URL}
ARG SPLUNK_TOKEN=${SPLUNK_TOKEN}
ARG REDIS_HOST=${REDIS_HOST}
ARG REDIS_PORT=${REDIS_PORT}
ARG REDIS_SENTINAL_MASTER=${REDIS_SENTINAL_MASTER}
ARG REDIS_SENTINAL_NODES=${REDIS_SENTINAL_NODES}
ARG REDIS_PASSWORD=${REDIS_PASSWORD}
ARG CODETABLE_REFRESH_ENABLED=${CODETABLE_REFRESH_ENABLED}
ARG CODETABLE_REFRESH_CRON=${CODETABLE_REFRESH_CRON}
ARG JAVA_OPTS=${JAVA_OPTS}

ENV SPLUNK_URL=${SPLUNK_URL}
ENV SPLUNK_TOKEN=${SPLUNK_TOKEN}
ENV REDIS_HOST=${REDIS_HOST}
ENV REDIS_PORT=${REDIS_PORT}
ENV REDIS_SENTINAL_MASTER=${REDIS_SENTINAL_MASTER}
ENV REDIS_SENTINAL_NODES=${REDIS_SENTINAL_NODES}
ENV REDIS_PASSWORD=${REDIS_PASSWORD}
ENV CODETABLE_REFRESH_ENABLED=${CODETABLE_REFRESH_ENABLED}
ENV CODETABLE_REFRESH_CRON=${CODETABLE_REFRESH_CRON}
ENV JAVA_OPTS=${JAVA_OPTS}

COPY --from=build ./target/oracle-data-api.jar /app/service.jar

CMD java $JAVA_OPTS -jar /app/service.jar
#############################################################################################
