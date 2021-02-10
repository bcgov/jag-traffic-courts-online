# base image
FROM node:10.16 as build-deps

# set working directory
ENV NODE_ROOT /usr/src/app
RUN mkdir -p /usr/src/app
WORKDIR /usr/src/app

COPY package.json package-lock.json ./

RUN npm install @angular/cli -g --silent
RUN npm install yarn

COPY . .

RUN yarn 

FROM nginx

FROM nginx:1.19-alpine
COPY --from=build-deps /usr/src/app/dist/citizen-portal /usr/share/nginx/html
COPY --from=build-deps /usr/src/app/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80

RUN printenv
CMD ["sh", "-c", "envsubst < /usr/share/nginx/html/assets/env.template.js > /usr/share/nginx/html/assets/env.js && exec nginx -g 'daemon off;'"]
