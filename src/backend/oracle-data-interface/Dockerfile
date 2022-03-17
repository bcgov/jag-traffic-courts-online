#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM maven:3.8.3-jdk-11 as build

ARG SKIP_TESTS=true
ARG SERVICE_NAME=oracle-data-interface-application
ARG SPLUNK_URL=https://localhost:8088
ARG SPLUNK_TOKEN=token

WORKDIR /

COPY . .

RUN mvn -B clean package \
		-Dtarget.fileName=${SERVICE_NAME} \
        -Dmaven.test.skip=${SKIP_TESTS} 

#############################################################################################

#############################################################################################
### Stage where Docker is running a java process to run a service built in previous stage ###
#############################################################################################
FROM eclipse-temurin:11

EXPOSE 8080

ARG SERVICE_NAME=oracle-data-interface-application
ARG SPLUNK_URL=${SPLUNK_URL}
ARG SPLUNK_TOKEN=${SPLUNK_TOKEN}

ENV SPLUNK_URL=${SPLUNK_URL}
ENV SPLUNK_TOKEN=${SPLUNK_TOKEN}

COPY --from=build ./target/${SERVICE_NAME}.jar /app/service.jar

CMD ["java", "-jar", "/app/service.jar"]
#############################################################################################
