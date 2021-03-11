FROM ubuntu:20.04
USER root
RUN apt-get update && \
    apt-get install -y \
        curl \
        sudo \
        git \
        nmap \
        nano \
        telnet \
        vim \
        openssl \
        inetutils-ping 
USER 1005460000
CMD ["/usr/bin/tail","-f","/dev/null"]