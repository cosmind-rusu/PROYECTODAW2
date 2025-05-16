# Etapa de build con Node
FROM node:18-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Etapa final con Caddy
FROM caddy:alpine
COPY --from=builder /app/dist /usr/share/caddy
# Copiar configuración de Caddy
COPY Caddyfile /etc/caddy/Caddyfile