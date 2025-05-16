# Etapa de build con Node
FROM node:18 AS builder
WORKDIR /app

# Instalar dependencias
COPY package*.json ./
RUN npm install

# Instalar versiones exactas conocidas por ser compatibles
RUN npm install vue@3.3.4 @vitejs/plugin-vue@4.3.0 vite@4.4.9 sass@1.32.13 --save-exact

# Copiar código fuente
COPY . .

# Construir la aplicación sin type-checking
RUN node_modules/.bin/vite build --mode production

# Etapa final con Caddy
FROM caddy:2-alpine
COPY --from=builder /app/dist /usr/share/caddy
COPY Caddyfile /etc/caddy/Caddyfile