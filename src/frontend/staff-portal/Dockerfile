ARG BUILD_IMAGE=docker.io/node
ARG RUNTIME_IMAGE=registry.access.redhat.com/ubi8/nginx-120

# Build image
FROM $BUILD_IMAGE:14.18 as build-deps

# set working directory
ENV NODE_ROOT /usr/src/app

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY package.json yarn.lock ./

RUN yarn config set network-timeout 300000
RUN yarn install

COPY . .

RUN NODE_OPTIONS=--max_old_space_size=4096 yarn run build-prod

# Runtime image
FROM $RUNTIME_IMAGE

# Optional Git Version arguments
ARG GIT_COMMIT_DATE=not-set
ARG GIT_SHA=not-set
ARG GIT_SHORT_SHA=not-set
ARG GIT_VERSION=not-set

LABEL git.commit.date="$GIT_COMMIT_DATE" \
      git.sha="$GIT_SHA" \
      git.sha.short="$GIT_SHORT_SHA" \
      git.version="$GIT_VERSION"

ENV APP_VERSION=$GIT_VERSION
ENV GIT_SHA=$GIT_SHA

COPY --from=build-deps /usr/src/app/src/favicon.ico /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/dist/staff-portal /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/nginx.conf /opt/app-root/etc/nginx.default.d/default.conf
COPY --from=build-deps /usr/src/app/nginx-api-proxy-pass.conf /opt/app-root/etc/nginx.default.d/nginx-api-proxy-pass.conf

EXPOSE 8080

CMD ["sh", "-c", "nginx -g 'daemon off;'"]
