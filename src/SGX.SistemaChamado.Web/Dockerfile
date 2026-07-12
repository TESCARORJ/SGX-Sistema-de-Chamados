FROM node:22-alpine AS build
WORKDIR /app

COPY src/SGX.SistemaChamado.Web/package*.json ./
RUN npm ci

COPY src/SGX.SistemaChamado.Web ./

ARG VITE_API_BASE_URL=
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
RUN npm run build

FROM nginx:1.28-alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY src/SGX.SistemaChamado.Web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist ./

EXPOSE 80
