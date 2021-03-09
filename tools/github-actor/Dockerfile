FROM debian:buster-slim

ARG GITHUB_RUNNER_VERSION="2.165.2"
ARG GITHUB_PAT "${GITHUB_PAT}"
ARG GITHUB_OWNER "${GITHUB_OWNER}"
ARG GITHUB_REPOSITORY "${GITHUB_REPOSITORY}"
ARG RUNNER_NAME "${RUNNER_NAME}"

ENV RUNNER_NAME "${RUNNER_NAME}"
ENV GITHUB_PAT "${GITHUB_PAT}"
ENV GITHUB_OWNER "${GITHUB_OWNER}"
ENV GITHUB_REPOSITORY "${GITHUB_REPOSITORY}"
ENV RUNNER_WORKDIR "_work"
RUN printenv
RUN apt-get update \
    && apt-get install -y \
        curl \
        sudo \
        git \
        jq \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && useradd -u 1001 -m github \
    && usermod -aG sudo github \
    && echo "%sudo ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers
RUN touch /home/github/.env && \
    touch /home/github/.path && \
    chown -R 1001:1001 /home/github && \
    chmod -R 777 /home/github

USER github
WORKDIR /home/github

RUN curl -Ls https://github.com/actions/runner/releases/download/v${GITHUB_RUNNER_VERSION}/actions-runner-linux-x64-${GITHUB_RUNNER_VERSION}.tar.gz | tar xz \
    && sudo ./bin/installdependencies.sh

COPY --chown=github:github entrypoint.sh ./entrypoint.sh
RUN sudo chmod u+x ./entrypoint.sh

ENTRYPOINT ["/home/github/entrypoint.sh"]