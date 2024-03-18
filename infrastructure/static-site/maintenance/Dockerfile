FROM docker.io/nginx:stable-alpine

COPY ./index.html /usr/share/nginx/html/index.html
COPY ./nginx.conf /etc/nginx/nginx.conf

USER root

# Create necessary directories and set permissions
RUN mkdir -p /usr/share/nginx/tmp/client_temp /usr/share/nginx/tmp/proxy_temp /usr/share/nginx/tmp/fastcgi_temp /usr/share/nginx/tmp/uwsgi_temp /usr/share/nginx/tmp/scgi_temp && \
    chown -R nginx:nginx /usr/share/nginx/tmp && \
    chmod -R 777 /usr/share/nginx/tmp

EXPOSE 8080

USER nginx

CMD ["sh", "-c", "nginx -g 'daemon off;'"]