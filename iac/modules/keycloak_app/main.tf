variable "container_app_environment_id" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "api_manager_host" {
  type = string
}

variable "volume_name" {
  type = string
}

variable "keycloak_admin_password" {
  type      = string
  sensitive = true
}

variable "storage_account_name" {
  type = string
}

variable "storage_account_primary_access_key" {
  type = string
}

variable "storage_share_name" {
  type = string
}

resource "azurerm_container_app_environment_storage" "this" {
  name                         = var.volume_name
  container_app_environment_id = var.container_app_environment_id
  account_name                 = var.storage_account_name
  access_key                   = var.storage_account_primary_access_key
  share_name                   = var.storage_share_name
  access_mode                  = "ReadOnly"
}

resource "azurerm_container_app" "this" {
  name                         = "keycloak"
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
    container {
      name   = "keycloak"
      image  = "quay.io/keycloak/keycloak:26.3"
      cpu    = 1.0
      memory = "2Gi"
      args = [
        "start-dev",
        "--hostname",
        var.api_manager_host,
        "--http-relative-path",
        "/keycloak",
        "--proxy-headers",
        "xforwarded",
        "--import-realm"
      ]
      env {
        name  = "KC_BOOTSTRAP_ADMIN_USERNAME"
        value = "admin"
      }
      env {
        name  = "KC_BOOTSTRAP_ADMIN_PASSWORD"
        value = var.keycloak_admin_password
      }
      volume_mounts {
        name = azurerm_container_app_environment_storage.this.name
        path = "/opt/keycloak/data/import"
      }
    }
    volume {
      name          = azurerm_container_app_environment_storage.this.name
      storage_name  = azurerm_container_app_environment_storage.this.name
      storage_type  = "AzureFile"
      mount_options = "dir_mode=0777,file_mode=0777"
    }
  }
}

output "keycloak_container_fqdn" {
  value = azurerm_container_app.this.ingress[0].fqdn
}
