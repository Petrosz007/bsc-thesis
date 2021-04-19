FROM node:15.5.1-alpine3.12 as builder


RUN mkdir /app
WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH

# Required for node-sass
RUN apk add --no-cache python2 make g++

COPY package*.json yarn.lock ./
RUN yarn install

COPY . /app

RUN yarn build

 
FROM nginx:1.19.6-alpine

EXPOSE 80
ENV API_URL="undefined"

RUN apk add --no-cache jq

COPY --from=builder /app/build /usr/share/nginx/html

COPY docker/nginx.conf /etc/nginx/conf.d/default.conf

COPY docker/docker-entrypoint.sh docker/generate_config_js.sh /
RUN chmod +x docker-entrypoint.sh generate_config_js.sh
 
ENTRYPOINT ["/docker-entrypoint.sh"]