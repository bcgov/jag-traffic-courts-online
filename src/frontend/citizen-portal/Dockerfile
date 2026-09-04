ARG BUILD_IMAGE=docker.io/node
ARG RUNTIME_IMAGE=registry.access.redhat.com/ubi8/nginx-120
ARG BUILD_MODE=Production

# Build image
FROM $BUILD_IMAGE:18.12 AS build-deps

# Expose the build mode to the build stage
ARG BUILD_MODE
ENV BUILD_MODE=${BUILD_MODE}

# set working directory
ENV NODE_ROOT /usr/src/app

RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY package.json yarn.lock ./

RUN yarn config set network-timeout 300000
RUN yarn install

COPY . .

RUN if [ "$BUILD_MODE" = "Debug" ] || [ "$BUILD_MODE" = "debug" ]; then \
      echo "Building citizen-portal in DEV mode"; \
      yarn run build-dev; \
    else \
      echo "Building citizen-portal in PROD mode"; \
      yarn run build-prod; \
    fi

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
COPY --from=build-deps /usr/src/app/dist/citizen-portal /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/nginx.conf /opt/app-root/etc/nginx.default.d/default.conf
COPY --from=build-deps /usr/src/app/nginx-api-proxy-pass.conf /opt/app-root/etc/nginx.default.d/nginx-api-proxy-pass.conf

EXPOSE 8080

CMD ["sh", "-c", "nginx -g 'daemon off;'"]
