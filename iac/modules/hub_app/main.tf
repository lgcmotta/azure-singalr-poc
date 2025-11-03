variable "container_app_environment_id" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "gateway_url" {
  type = string
}

variable "swa_hostname" {
  type = string
}

variable "signalr_primary_connection_string" {
  type      = string
  sensitive = true
}

variable "min_replicas" {
  type = number
}

variable "max_replicas" {
  type = number
}

resource "azurerm_container_app" "this" {
  name                         = "hub-app"
  container_app_environment_id = var.container_app_environment_id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"

  ingress {
    external_enabled = true
    target_port      = 8080
    transport        = "auto"
    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = var.min_replicas
    max_replicas = var.max_replicas
    container {
      name   = "hub-app"
      image  = "ghcr.io/lgcmotta/azure-signalr-poc/api:latest"
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "Azure__SignalR__ConnectionString"
        value = "${var.signalr_primary_connection_string};ClientEndpoint=${var.gateway_url}/signalr/"
      }

      env {
        name  = "Keycloak__AdminUrl"
        value = "${var.gateway_url}/keycloak/api/v1"
      }

      env {
        name  = "Keycloak__Realm"
        value = "PocSignalR"
      }

      env {
        name  = "Keycloak__AuthServerUrl"
        value = "${var.gateway_url}/keycloak"
      }

      env {
        name  = "Keycloak__SslRequired"
        value = "external"
      }

      env {
        name  = "Keycloak__Resource"
        value = "hub_client"
      }

      env {
        name  = "Keycloak__PublicClient"
        value = "true"
      }

      env {
        name  = "Keycloak__ConfidentialPort"
        value = "0"
      }

      env {
        name  = "Keycloak__VerifyTokenAudience"
        value = "false"
      }

      env {
        name  = "Keycloak__RequireHttpsMetadata"
        value = "true"
      }

      env {
        name  = "Keycloak__Scopes__0"
        value = "openid"
      }

      env {
        name  = "Keycloak__Scopes__1"
        value = "email"
      }

      env {
        name  = "Keycloak__Scopes__2"
        value = "profile"
      }

      env {
        name  = "OpenApi__Title"
        value = "Hub API"
      }

      env {
        name  = "OpenApi__Server"
        value = "${var.gateway_url}/api"
      }

      env {
        name  = "OpenApi__SubscriptionRequired"
        value = "true"
      }

      env {
        name  = "OpenApi__DocumentEndpoint"
        value = "swagger/openapi/v1.json"
      }

      env {
        name  = "OpenApi__SwaggerEndpoint"
        value = "openapi/v1.json?subscription-key={apimKey}"
      }

      env {
        name  = "OpenApi__Contact__Name"
        value = "Luiz Motta"
      }

      env {
        name  = "OpenApi__Contact__Email"
        value = "lgcmotta@outlook.com"
      }

      env {
        name  = "CorsOrigins__0"
        value = var.gateway_url
      }

      env {
        name  = "CorsOrigins__1"
        value = "http://localhost:5281"
      }

      env {
        name  = "CorsOrigins__2"
        value = "https://${var.swa_hostname}"
      }

      env {
        name  = "CorsOrigins__3"
        value = "http://localhost:5173"
      }

      env {
        name  = "AppContainer_Revision"
        value = "r${timestamp()}"
      }

      readiness_probe {
        port             = 8080
        transport        = "HTTP"
        path             = "/healthz/ready"
        initial_delay    = 30
        interval_seconds = 10
      }

      liveness_probe {
        port             = 8080
        transport        = "HTTP"
        path             = "/healthz/live"
        initial_delay    = 30
        interval_seconds = 10
      }
    }
  }
}


output "hub_container_fqdn" {
  value = azurerm_container_app.this.ingress[0].fqdn
}
