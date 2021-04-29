# base image
FROM node:10.16 as build-deps
RUN printenv

# set working directory
ARG USE_MOCK_SERVICES
ARG USE_KEYCLOAK
ARG API_URL
ARG KEYCLOAK_URL
ARG KEYCLOAK_REALM
ARG KEYCLOAK_CLIENT_ID
RUN printenv

ENV NODE_ROOT /usr/src/app

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY package.json package-lock.json yarn.lock ./

RUN npm install
RUN npm audit fix

COPY . .

RUN rm -rf dist
RUN yarn cache clean
RUN yarn build-dev

FROM registry.redhat.io/ubi8/nginx-118

COPY --from=build-deps /usr/src/app/dist/citizen-portal /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/nginx.conf /opt/app-root/etc/nginx.default.d/default.conf

EXPOSE 8080

RUN printenv
CMD ["sh", "-c", "nginx -g 'daemon off;'"]
