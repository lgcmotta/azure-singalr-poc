variable "container_environment_id" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "gateway_url" {
  type = string
}

variable "signalr_primary_connection_string" {
  type      = string
  sensitive = true
}

resource "azurerm_container_app" "this" {
  name                         = "hub-api"
  container_app_environment_id = var.container_environment_id
  resource_group_name          = var.resource_group_name
  revision_mode                = "Single"

  ingress {
    external_enabled = true
    target_port      = 8080
    transport        = "auto"
  }

  template {
    container {
      name   = "api"
      image  = "ghcr.io/lgcmotta/azure-signalr-poc/api:latest"
      cpu    = 0.25
      memory = "0.5Gi"
      env {
        name  = "Azure__SignalR__ConnectionString"
        value = "${var.signalr_primary_connection_string};ClientEndpoint=${var.gateway_url}"
      }
    }
  }
}
