#############################################################################################
###              Stage where Docker is building spring boot app using maven               ###
#############################################################################################
FROM maven:3.8.1-openjdk-8

WORKDIR /

COPY . .

RUN mvn clean package -Dmaven.test.skip=true 

CMD ["java", "-jar", "./target/ticket-generator-0.0.1-SNAPSHOT.jar"]
