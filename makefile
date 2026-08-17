# ============================================
# VARIABLES
# ============================================
COMPOSE = docker-compose
COMPOSE_DEV = $(COMPOSE) -f docker-compose.yml -f docker-compose.dev.yml
COMPOSE_PROD = $(COMPOSE) -f docker-compose.yml -f docker-compose.prod.yml

# Colores
RED = \033[0;31m
GREEN = \033[0;32m
YELLOW = \033[0;33m
BLUE = \033[0;34m
MAGENTA = \033[0;35m
CYAN = \033[0;36m
NC = \033[0m

# ============================================
# HELP
# ============================================
help: ## 📚 Muestra todos los comandos disponibles
	@echo "$(BLUE)╔══════════════════════════════════════════════════════════╗$(NC)"
	@echo "$(BLUE)║     📋 COMANDOS DISPONIBLES PARA EL PROYECTO           ║$(NC)"
	@echo "$(BLUE)╚══════════════════════════════════════════════════════════╝$(NC)"
	@echo ""
	@echo "$(CYAN)🔧 CONSTRUCCIÓN:$(NC)"
	@grep -E '^build-.*:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)  %-35s$(NC) %s\n", $$1, $$2}'
	@echo ""
	@echo "$(CYAN)🚀 EJECUCIÓN:$(NC)"
	@grep -E '^run-.*:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)  %-35s$(NC) %s\n", $$1, $$2}'
	@echo ""
	@echo "$(CYAN)📋 LOGS:$(NC)"
	@grep -E '^logs-.*:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)  %-35s$(NC) %s\n", $$1, $$2}'
	@echo ""
	@echo "$(CYAN)🐚 SHELL:$(NC)"
	@grep -E '^shell-.*:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)  %-35s$(NC) %s\n", $$1, $$2}'
	@echo ""
	@echo "$(CYAN)🧹 UTILIDADES:$(NC)"
	@grep -E '^(down|clean|migrate-|health-|status|ps):.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "$(GREEN)  %-35s$(NC) %s\n", $$1, $$2}'
	@echo ""

# ============================================
# CONSTRUCCIÓN - TODOS
# ============================================
build-all-dev: ## 🔨 Construye TODOS los servicios en desarrollo
	@echo "$(BLUE)🔨 Construyendo TODOS los servicios (desarrollo)...$(NC)"
	$(COMPOSE_DEV) build --no-cache
	@echo "$(GREEN)✅ Todos los servicios construidos exitosamente$(NC)"

build-all-prod: ## 🔨 Construye TODOS los servicios en producción
	@echo "$(BLUE)🔨 Construyendo TODOS los servicios (producción)...$(NC)"
	$(COMPOSE_PROD) build --no-cache
	@echo "$(GREEN)✅ Todos los servicios construidos exitosamente$(NC)"

# ============================================
# CONSTRUCCIÓN - INDIVIDUAL (Desarrollo)
# ============================================
build-backend-dev: ## 🔨 Construye solo BACKEND en desarrollo
	@echo "$(BLUE)🔨 Construyendo backend (desarrollo)...$(NC)"
	$(COMPOSE_DEV) build --no-cache backend_api
	@echo "$(GREEN)✅ Backend construido exitosamente$(NC)"

build-frontend-dev: ## 🔨 Construye solo FRONTEND en desarrollo
	@echo "$(BLUE)🔨 Construyendo frontend (desarrollo)...$(NC)"
	$(COMPOSE_DEV) build --no-cache frontend_app
	@echo "$(GREEN)✅ Frontend construido exitosamente$(NC)"

build-postgres-dev: ## 🔨 Construye solo POSTGRES en desarrollo
	@echo "$(BLUE)🔨 Construyendo postgres (desarrollo)...$(NC)"
	$(COMPOSE_DEV) build postgres_db
	@echo "$(GREEN)✅ Postgres construido exitosamente$(NC)"

# ============================================
# CONSTRUCCIÓN - INDIVIDUAL (Producción)
# ============================================
build-backend-prod: ## 🔨 Construye solo BACKEND en producción
	@echo "$(BLUE)🔨 Construyendo backend (producción)...$(NC)"
	$(COMPOSE_PROD) build --no-cache backend_api
	@echo "$(GREEN)✅ Backend construido exitosamente$(NC)"

build-frontend-prod: ## 🔨 Construye solo FRONTEND en producción
	@echo "$(BLUE)🔨 Construyendo frontend (producción)...$(NC)"
	$(COMPOSE_PROD) build --no-cache frontend_app
	@echo "$(GREEN)✅ Frontend construido exitosamente$(NC)"

build-postgres-prod: ## 🔨 Construye solo POSTGRES en producción
	@echo "$(BLUE)🔨 Construyendo postgres (producción)...$(NC)"
	$(COMPOSE_PROD) build postgres_db
	@echo "$(GREEN)✅ Postgres construido exitosamente$(NC)"

# ============================================
# EJECUCIÓN - TODOS
# ============================================
dev: ## 🚀 Ejecuta TODOS los servicios en desarrollo (con logs)
	@echo "$(BLUE)🚀 Iniciando TODOS los servicios (desarrollo)...$(NC)"
	$(COMPOSE_DEV) up

dev-d: ## 🚀 Ejecuta TODOS los servicios en desarrollo (background)
	@echo "$(BLUE)🚀 Iniciando TODOS los servicios en background (desarrollo)...$(NC)"
	$(COMPOSE_DEV) up -d
	@echo "$(GREEN)✅ Servicios iniciados en background$(NC)"
	@echo "$(YELLOW)📝 Para ver logs: make logs-all-dev$(NC)"

prod: ## 🏭 Ejecuta TODOS los servicios en producción (con logs)
	@echo "$(BLUE)🏭 Iniciando TODOS los servicios (producción)...$(NC)"
	$(COMPOSE_PROD) up

prod-d: ## 🏭 Ejecuta TODOS los servicios en producción (background)
	@echo "$(BLUE)🏭 Iniciando TODOS los servicios en background (producción)...$(NC)"
	$(COMPOSE_PROD) up -d
	@echo "$(GREEN)✅ Servicios iniciados en background$(NC)"
	@echo "$(YELLOW)📝 Para ver logs: make logs-all-prod$(NC)"

# ============================================
# EJECUCIÓN - INDIVIDUAL (Desarrollo)
# ============================================
run-backend-dev: ## 🚀 Ejecuta solo BACKEND en desarrollo
	@echo "$(BLUE)🚀 Ejecutando backend (desarrollo)...$(NC)"
	$(COMPOSE_DEV) up backend_api postgres_db

run-frontend-dev: ## 🚀 Ejecuta solo FRONTEND en desarrollo
	@echo "$(BLUE)🚀 Ejecutando frontend (desarrollo)...$(NC)"
	$(COMPOSE_DEV) up frontend_app

run-postgres-dev: ## 🚀 Ejecuta solo POSTGRES en desarrollo
	@echo "$(BLUE)🚀 Ejecutando postgres (desarrollo)...$(NC)"
	$(COMPOSE_DEV) up postgres_db

run-backend-only-dev: ## 🚀 Ejecuta solo BACKEND sin dependencias (desarrollo)
	@echo "$(BLUE)🚀 Ejecutando backend sin dependencias (desarrollo)...$(NC)"
	$(COMPOSE_DEV) run --service-ports backend_api

# ============================================
# EJECUCIÓN - INDIVIDUAL (Producción)
# ============================================
run-backend-prod: ## 🚀 Ejecuta solo BACKEND en producción
	@echo "$(BLUE)🚀 Ejecutando backend (producción)...$(NC)"
	$(COMPOSE_PROD) up backend_api postgres_db

run-frontend-prod: ## 🚀 Ejecuta solo FRONTEND en producción
	@echo "$(BLUE)🚀 Ejecutando frontend (producción)...$(NC)"
	$(COMPOSE_PROD) up frontend_app

run-postgres-prod: ## 🚀 Ejecuta solo POSTGRES en producción
	@echo "$(BLUE)🚀 Ejecutando postgres (producción)...$(NC)"
	$(COMPOSE_PROD) up postgres_db

# ============================================
# LOGS
# ============================================
logs-all-dev: ## 📋 Logs de TODOS los servicios en desarrollo
	$(COMPOSE_DEV) logs -f

logs-all-prod: ## 📋 Logs de TODOS los servicios en producción
	$(COMPOSE_PROD) logs -f

logs-backend-dev: ## 📋 Logs solo del BACKEND en desarrollo
	$(COMPOSE_DEV) logs -f backend_api

logs-frontend-dev: ## 📋 Logs solo del FRONTEND en desarrollo
	$(COMPOSE_DEV) logs -f frontend_app

logs-backend-prod: ## 📋 Logs solo del BACKEND en producción
	$(COMPOSE_PROD) logs -f backend_api

logs-frontend-prod: ## 📋 Logs solo del FRONTEND en producción
	$(COMPOSE_PROD) logs -f frontend_app

# ============================================
# SHELL
# ============================================
shell-backend-dev: ## 🐚 Abre SHELL en el backend (desarrollo)
	docker exec -it appointment_api_dev /bin/bash

shell-frontend-dev: ## 🐚 Abre SHELL en el frontend (desarrollo)
	docker exec -it appointment_ui_dev /bin/sh

shell-backend-prod: ## 🐚 Abre SHELL en el backend (producción)
	docker exec -it appointment_api_prod /bin/bash

shell-frontend-prod: ## 🐚 Abre SHELL en el frontend (producción)
	docker exec -it appointment_ui_prod /bin/sh

# ============================================
# MIGRACIONES
# ============================================
migrate-dev: ## 🗄️ Ejecuta migraciones en desarrollo
	@echo "$(BLUE)🗄️ Ejecutando migraciones (desarrollo)...$(NC)"
	docker exec -it appointment_api_dev dotnet ef database update

migrate-prod: ## 🗄️ Ejecuta migraciones en producción
	@echo "$(BLUE)🗄️ Ejecutando migraciones (producción)...$(NC)"
	docker exec -it appointment_api_prod dotnet ef database update

migrate-create-dev: ## 🗄️ Crea una nueva migración en desarrollo
	@echo "$(BLUE)🗄️ Creando nueva migración (desarrollo)...$(NC)"
	@read -p "Nombre de la migración: " name; \
	docker exec -it appointment_api_dev dotnet ef migrations add $$name

# ============================================
# HEALTH CHECKS
# ============================================
health-all-dev: ## 💚 Verifica TODOS los servicios en desarrollo
	@echo "$(BLUE)💚 Verificando todos los servicios (desarrollo)...$(NC)"
	$(COMPOSE_DEV) ps
	@echo ""
	@echo "$(BLUE)🔍 Backend:$(NC)"
	@curl -s http://localhost:8081/health && echo " ✅" || echo " ❌"
	@echo "$(BLUE)🔍 Frontend:$(NC)"
	@curl -s -o /dev/null -w "%{http_code}" http://localhost:4200 | grep -q "200" && echo " ✅" || echo " ❌"
	@echo "$(BLUE)🔍 Postgres:$(NC)"
	@docker exec appointment_db_dev pg_isready -U admin -d appointment_db && echo " ✅" || echo " ❌"

health-all-prod: ## 💚 Verifica TODOS los servicios en producción
	@echo "$(BLUE)💚 Verificando todos los servicios (producción)...$(NC)"
	$(COMPOSE_PROD) ps
	@echo ""
	@echo "$(BLUE)🔍 Backend:$(NC)"
	@curl -s http://localhost:8082/health && echo " ✅" || echo " ❌"
	@echo "$(BLUE)🔍 Frontend:$(NC)"
	@curl -s -o /dev/null -w "%{http_code}" http://localhost:4201 | grep -q "200" && echo " ✅" || echo " ❌"
	@echo "$(BLUE)🔍 Postgres:$(NC)"
	@docker exec appointment_db_prod pg_isready -U admin -d appointment_db && echo " ✅" || echo " ❌"

# ============================================
# ESTADO
# ============================================
status: ## 📊 Muestra el estado de todos los contenedores
	@echo "$(BLUE)📊 Estado de contenedores:$(NC)"
	@docker ps -a --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}" | grep -E "appointment|NAMES"

ps: ## 📊 Alias para status
	make status

# ============================================
# UTILIDADES
# ============================================
stop: ## ⏹️ Detiene TODOS los servicios
	@echo "$(YELLOW)⏹️ Deteniendo todos los servicios...$(NC)"
	$(COMPOSE) down
	@echo "$(GREEN)✅ Servicios detenidos$(NC)"

stop-dev: ## ⏹️ Detiene solo servicios de desarrollo
	@echo "$(YELLOW)⏹️ Deteniendo servicios de desarrollo...$(NC)"
	$(COMPOSE_DEV) down
	@echo "$(GREEN)✅ Servicios de desarrollo detenidos$(NC)"

stop-prod: ## ⏹️ Detiene solo servicios de producción
	@echo "$(YELLOW)⏹️ Deteniendo servicios de producción...$(NC)"
	$(COMPOSE_PROD) down
	@echo "$(GREEN)✅ Servicios de producción detenidos$(NC)"

clean: ## 🧹 Limpia TODOS los contenedores, imágenes y volúmenes
	@echo "$(RED)⚠️  Esto eliminará todos los contenedores, imágenes y volúmenes$(NC)"
	@read -p "¿Estás seguro? (s/N): " confirm; \
	if [ "$$confirm" = "s" ] || [ "$$confirm" = "S" ]; then \
		echo "$(RED)🧹 Limpiando todo...$(NC)"; \
		$(COMPOSE) down -v --rmi all; \
		docker system prune -af; \
		echo "$(GREEN)✅ Limpieza completada$(NC)"; \
	else \
		echo "$(YELLOW)❌ Operación cancelada$(NC)"; \
	fi

restart-dev: ## 🔄 Reinicia TODOS los servicios de desarrollo
	@echo "$(BLUE)🔄 Reiniciando servicios de desarrollo...$(NC)"
	$(COMPOSE_DEV) restart
	@echo "$(GREEN)✅ Servicios reiniciados$(NC)"

restart-prod: ## 🔄 Reinicia TODOS los servicios de producción
	@echo "$(BLUE)🔄 Reiniciando servicios de producción...$(NC)"
	$(COMPOSE_PROD) restart
	@echo "$(GREEN)✅ Servicios reiniciados$(NC)"

# ============================================
# DEPLOY
# ============================================
deploy: ## 🚀 Despliegue COMPLETO a producción
	@echo "$(GREEN)🚀 Iniciando despliegue a producción...$(NC)"
	@echo "$(BLUE)1/4 Construyendo imágenes...$(NC)"
	$(MAKE) build-all-prod
	@echo "$(BLUE)2/4 Deteniendo servicios existentes...$(NC)"
	$(COMPOSE_PROD) down
	@echo "$(BLUE)3/4 Iniciando servicios...$(NC)"
	$(COMPOSE_PROD) up -d
	@echo "$(BLUE)4/4 Verificando estado...$(NC)"
	sleep 5
	$(MAKE) health-all-prod
	@echo "$(GREEN)✅ Despliegue completado exitosamente!$(NC)"
	@echo "$(YELLOW)📝 Backend: http://localhost:8082$(NC)"
	@echo "$(YELLOW)📝 Frontend: http://localhost:4201$(NC)"

# ============================================
# DESARROLLO RÁPIDO
# ============================================
quick-dev: ## ⚡ Inicio rápido para desarrollo (build + up)
	@echo "$(BLUE)⚡ Inicio rápido de desarrollo...$(NC)"
	$(MAKE) build-all-dev
	$(MAKE) dev

quick-prod: ## ⚡ Inicio rápido para producción (build + up)
	@echo "$(BLUE)⚡ Inicio rápido de producción...$(NC)"
	$(MAKE) build-all-prod
	$(MAKE) prod

# ============================================
# PRUEBAS
# ============================================
test-backend-dev: ## 🧪 Ejecuta pruebas del backend en desarrollo
	@echo "$(BLUE)🧪 Ejecutando pruebas del backend (desarrollo)...$(NC)"
	docker exec -it appointment_api_dev dotnet test

test-backend-prod: ## 🧪 Ejecuta pruebas del backend en producción
	@echo "$(BLUE)🧪 Ejecutando pruebas del backend (producción)...$(NC)"
	docker exec -it appointment_api_prod dotnet test