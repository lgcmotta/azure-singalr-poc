terraform {
  required_version = ">= 1.10"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.51.0"
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

module "hub_app" {
  source                            = "./modules/hub_app"
  container_app_environment_id      = module.containers_env.environment_id
  resource_group_name               = azurerm_resource_group.this.name
  gateway_url                       = azurerm_api_management.this.gateway_url
  signalr_primary_connection_string = azurerm_signalr_service.this.primary_connection_string
  depends_on = [
    module.containers_env,
    azurerm_resource_group.this,
    azurerm_api_management.this,
    azurerm_signalr_service.this
  ]
}

module "keycloak_api" {
  source              = "./modules/keycloak_api"
  api_management_name = azurerm_api_management.this.name
  container_app_fqdn  = module.keycloak_app.keycloak_container_fqdn
  resource_group_name = azurerm_resource_group.this.name
  depends_on          = [azurerm_api_management.this, azurerm_resource_group.this, module.keycloak_app]
}

module "hub_api" {
  source              = "./modules/hub_api"
  api_management_name = azurerm_api_management.this.name
  container_app_fqdn  = module.hub_app.hub_container_fqdn
  resource_group_name = azurerm_resource_group.this.name
  depends_on          = [azurerm_api_management.this, azurerm_resource_group.this, module.hub_app]
}

module "signalr_api" {
  source              = "./modules/signalr_api"
  resource_group_name = azurerm_resource_group.this.name
  api_management_name = azurerm_api_management.this.name
  signalr_hostname    = azurerm_signalr_service.this.hostname
  depends_on          = [azurerm_api_management.this, azurerm_resource_group.this, azurerm_signalr_service.this]
}

module "subscription_keys" {
  source              = "./modules/subscription_key"
  resource_group_name = azurerm_resource_group.this.name
  api_management_name = azurerm_api_management.this.name
  keys = [
    {
      name         = var.api_management.subscription_name
      display_name = var.api_management.subscription_display_name
    }
  ]
  depends_on = [module.hub_api, module.signalr_api]
}
