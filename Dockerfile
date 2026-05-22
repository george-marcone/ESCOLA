# build stage
FROM node:22 AS build
WORKDIR /app
ARG NUXT_PUBLIC_API_BASE=http://localhost:5000/api
ENV NUXT_PUBLIC_API_BASE=$NUXT_PUBLIC_API_BASE
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run generate

# production stage
FROM nginx:alpine
COPY --from=build /app/.output/public /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["/usr/sbin/nginx","-g","daemon off;"]
