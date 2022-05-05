# Build image
FROM node:14.18 as build-deps

ARG API_BASE_PATH=http://staff-api:8080/api/
ENV API_BASE_PATH=${API_BASE_PATH}

# set working directory
ENV NODE_ROOT /usr/src/app

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY package.json yarn.lock ./

RUN yarn config set network-timeout 300000
RUN yarn install

COPY . .

RUN NODE_OPTIONS=--max_old_space_size=4096 yarn run build-prod

# replace the API_BASE_PATH env var in nginx.conf
RUN apt-get update; \
    apt-get install -y gettext-base
RUN envsubst '${API_BASE_PATH}' < /usr/src/app/nginx-api-proxy-pass.conf.template > /usr/src/app/nginx-api-proxy-pass.conf

# Runtime image
FROM registry.access.redhat.com/ubi8/nginx-120

COPY --from=build-deps /usr/src/app/src/favicon.ico /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/dist/staff-portal /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/nginx.conf /opt/app-root/etc/nginx.default.d/default.conf
COPY --from=build-deps /usr/src/app/nginx-api-proxy-pass.conf /opt/app-root/etc/nginx.default.d/nginx-api-proxy-pass.conf

EXPOSE 8080

CMD ["sh", "-c", "nginx -g 'daemon off;'"]
