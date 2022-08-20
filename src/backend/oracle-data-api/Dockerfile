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

COPY --from=build ./target/oracle-data-api.jar .
COPY --from=build ./entrypoint.sh .
RUN chown 1001:0 ./entrypoint.sh && chmod 550 ./entrypoint.sh
CMD ["./entrypoint.sh", "java", "-jar", "/app/oracle-data-api.jar"]
#############################################################################################
