FROM ubuntu:bionic

ARG DEBIAN_FRONTEND=noninteractive
ARG DOCKER_VERSION=17.06.0-ce

RUN apt-get update && \
apt-get install -y libglu1 xvfb libxcursor1

COPY Worker/UnityMmo/build/StandaloneLinux64                  /root/build/
COPY Docker/Worker/entrypoint.sh           /entrypoint.sh

WORKDIR /root/
ENTRYPOINT ["/bin/bash", "/entrypoint.sh"]