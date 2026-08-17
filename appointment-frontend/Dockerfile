# ============ ETAPA DE DESARROLLO ============
FROM node:26-alpine AS development

WORKDIR /app

# Instalar Angular CLI globalmente
RUN npm install -g @angular/cli@22

# Copiar archivos de dependencias
COPY package*.json ./

# Instalar dependencias
RUN npm ci

# Copiar el resto del código
COPY . .

# Exponer puerto de desarrollo
EXPOSE 4200

# Comando simple para desarrollo con hot reload
CMD ["ng", "serve", "--host", "0.0.0.0"]

# ============ ETAPA DE CONSTRUCCIÓN ============
FROM node:26-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

# ============ ETAPA DE PRODUCCIÓN ============
FROM nginx:alpine AS production

# Copiar los archivos construidos
COPY --from=build /app/dist/appointment-frontend/browser /usr/share/nginx/html

# Copiar configuración de nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
