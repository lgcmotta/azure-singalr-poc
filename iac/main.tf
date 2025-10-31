terraform {
  required_version = ">= 1.10"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.50.0"
    }
  }
  backend "s3" {
    bucket = var.aws.bucket
    key    = var.aws.key
    region = var.aws.region
  }
}

provider "azurerm" {
  features {

  }
  subscription_id = var.azure.subscription_id
  tenant_id       = var.azure.tenant_id
}

resource "random_string" "this" {
  length  = 6
  upper   = false
  special = false
  numeric = false
}

resource "azurerm_resource_provider_registration" "this" {
  for_each = toset(["Microsoft.App"])
  name     = each.value
}

resource "azurerm_resource_group" "this" {
  name     = "rg-signalr-poc-dev"
  location = "brazilsouth"
}

resource "azurerm_signalr_service" "this" {
  name                = "motta-signalr-poc-${random_string.this.result}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  depends_on          = [azurerm_resource_group.this, random_string.this]
  sku {
    capacity = 1
    name     = "Standard_S1"
  }
}

resource "azurerm_api_management" "this" {
  name                = "apim-${random_string.this.result}"
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  publisher_email     = "lgcmotta@outlook.com"
  publisher_name      = "Luiz Motta"
  sku_name            = "Developer_1"
  depends_on          = [azurerm_resource_group.this, random_string.this, azurerm_signalr_service.this]
}

module "realm_file" {
  source             = "./modules/realm"
  api_management_url = azurerm_api_management.this.gateway_url
  depends_on         = [azurerm_api_management.this]
}

module "storage_account" {
  source              = "./modules/storage_account"
  resource_suffix     = random_string.this.result
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location
  storage_share_name  = var.keycloak.storage_name
  storage_share_files = [
    {
      name   = var.keycloak.realm_filename
      source = module.realm_file.filename
    }
  ]
  depends_on = [module.realm_file]
}

module "containers_env" {
  source              = "./modules/container_apps_env"
  resource_suffix     = random_string.this.result
  location            = azurerm_resource_group.this.location
  resource_group_name = azurerm_resource_group.this.name
  depends_on          = [azurerm_resource_group.this, module.storage_account]
}

module "keycloak_app" {
  source                             = "./modules/keycloak_app"
  container_app_environment_id       = module.containers_env.environment_id
  resource_group_name                = azurerm_resource_group.this.name
  api_manager_host                   = module.realm_file.api_management_host
  volume_name                        = var.keycloak.volume_name
  keycloak_admin_password            = var.keycloak_admin_password
  storage_account_name               = module.storage_account.name
  storage_account_primary_access_key = module.storage_account.primary_access_key
  storage_share_name                 = module.storage_account.storage_share_name
  depends_on = [
    azurerm_resource_group.this,
    azurerm_api_management.this,
    module.realm_file,
    module.storage_account,
    module.containers_env
  ]
}

module "keycloak_api" {
  source              = "./modules/keycloak_api"
  api_management_name = azurerm_api_management.this.name
  container_app_fqdn  = module.keycloak_app.keycloak_container_fqdn
  resource_group_name = azurerm_resource_group.this.name
  depends_on          = [azurerm_api_management.this, azurerm_resource_group.this, module.keycloak_app]
}


#
# resource "azurerm_api_management_api" "this" {
#   name                  = "signalr-connect"
#   resource_group_name   = azurerm_resource_group.this.name
#   api_management_name   = azurerm_api_management.this.name
#   revision              = "1"
#   display_name          = "SignalR connect"
#   protocols             = ["wss"]
#   api_type              = "websocket"
#   service_url           = "wss://${azurerm_signalr_service.this.hostname}/client/"
#   subscription_required = true
#   depends_on            = [azurerm_signalr_service.this]
# }

#
# resource "azurerm_container_app" "this" {
#   name                         = "poc-signalr-backend"
#   container_app_environment_id = azurerm_container_app_environment.this.id
#   resource_group_name          = azurerm_resource_group.this.name
#   revision_mode                = "Single"
#
#   ingress {
#     external_enabled = true
#     target_port      = 8080
#     transport        = "auto"
#   }
#
#   template {
#     container {
#       name   = "api"
#       image  = "ghcr.io/lgcmotta/azure-signalr-poc/backend:latest"
#       cpu    = 0.25
#       memory = "0.5Gi"
#       env {
#         name  = "Azure__SignalR__ConnectionString"
#         value = "${azurerm_signalr_service.this.primary_connection_string};ClientEndpoint=${azurerm_api_management.this.gateway_url}"
#       }
#     }
#   }
# }
