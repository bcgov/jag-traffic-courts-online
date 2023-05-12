#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM maven:3.8.6-jdk-11 as build

WORKDIR /opt/keycloak-init
COPY . /opt/keycloak-init

RUN mvn clean package -Dmaven.test.skip=true 

RUN chown -R 1001:0 /opt/keycloak-init && \
    chown -R 1001:0 $HOME && \
    chmod -R ug+rwX /opt/keycloak-init

USER 1001

CMD ["java", "-jar", "/opt/keycloak-init/target/keycloak-user-initializer-0.0.1-SNAPSHOT.jar"]