FROM node as frontend-env
WORKDIR /src
COPY . .
WORKDIR /src/CDN
RUN npm install
RUN npm run bundle

FROM kyma/docker-nginx
COPY --from=frontend-env /src/CDN /var/www
ENTRYPOINT ["nginx"]
