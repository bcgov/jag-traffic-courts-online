#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM maven:3.8.1-openjdk-8

WORKDIR /opt/ticket-gen
COPY . /opt/ticket-gen

RUN mvn clean package -Dmaven.test.skip=true 

RUN chown -R 1001:0 /opt/ticket-gen && \
    chown -R 1001:0 $HOME && \
    chmod -R ug+rwX /opt/ticket-gen

USER 1001

CMD ["java", "-jar", "/opt/ticket-gen/target/ticket-generator-0.0.1-SNAPSHOT.jar"]
