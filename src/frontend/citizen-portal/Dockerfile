# base image
FROM node:10.16 as build-deps

# set working directory
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

FROM nginx

FROM nginx:1.19-alpine

RUN rm -rf /usr/share/nginx/html/
COPY --from=build-deps /usr/src/app/dist/citizen-portal /usr/share/nginx/html

RUN rm -rf /etc/nginx/conf.d/default.conf
COPY --from=build-deps /usr/src/app/nginx.conf /etc/nginx/conf.d/default.conf


# CMD ["nginx", "-g", "daemon off;"]
RUN chmod g+rwx /var/cache/nginx /var/run /var/log/nginx
RUN chmod g+rwx /usr/share/nginx/html/


EXPOSE 8080

RUN printenv
CMD ["sh", "-c", "nginx -g 'daemon off;'"]
