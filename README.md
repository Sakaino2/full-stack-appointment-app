# ============ TODOS LOS SERVICIOS ============

make dev # Desarrollo completo (con logs)
make dev-d # Desarrollo completo (background)
make prod # Producción completa (con logs)
make prod-d # Producción completa (background)

# ============ CONSTRUCCIÓN ============

make build-all-dev # Construir todo en desarrollo
make build-all-prod # Construir todo en producción
make build-backend-dev # Solo backend en desarrollo
make build-frontend-dev # Solo frontend en desarrollo
make build-backend-prod # Solo backend en producción
make build-frontend-prod # Solo frontend en producción

# ============ EJECUCIÓN INDIVIDUAL ============

make run-backend-dev # Solo backend en desarrollo
make run-frontend-dev # Solo frontend en desarrollo
make run-backend-prod # Solo backend en producción
make run-frontend-prod # Solo frontend en producción

# ============ LOGS ============

make logs-all-dev # Logs de todos los servicios
make logs-backend-dev # Logs solo del backend
make logs-frontend-dev # Logs solo del frontend

# ============ UTILIDADES ============

make status # Ver estado de contenedores
make health-all-dev # Health check de todos los servicios
make migrate-dev # Ejecutar migraciones
make shell-backend-dev # Shell en el backend
make stop # Detener todos los servicios
make clean # Limpiar todo

# ============ DEPLOY ============

make deploy # Despliegue completo a producción
make quick-dev # Inicio rápido en desarrollo
make quick-prod # Inicio rápido en producción
