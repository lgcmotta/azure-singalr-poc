variable "resource_suffix" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

resource "azurerm_log_analytics_workspace" "this" {
  name                = "law-${var.resource_suffix}"
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "this" {
  name                       = "aca-dev-${var.resource_suffix}"
  location                   = var.location
  resource_group_name        = var.resource_group_name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.this.id
  depends_on                 = [azurerm_log_analytics_workspace.this]
}

output "environment_id" {
  value = azurerm_container_app_environment.this.id
}
