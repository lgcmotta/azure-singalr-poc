variable "resource_group_name" {
  type = string
}

variable "api_management_name" {
  type = string
}

variable "signalr_hostname" {
  type = string
}

resource "azurerm_api_management_api" "negotiate" {
  name                  = "signalr-negotiate"
  resource_group_name   = var.resource_group_name
  api_management_name   = var.api_management_name
  revision              = "1"
  display_name          = "SignalR negotiate"
  protocols             = ["https"]
  path                  = "signalr/client/negotiate"
  service_url           = "https://${var.signalr_hostname}/client/negotiate"
  subscription_required = true
}

resource "azurerm_api_management_api_operation" "options" {
  api_management_name = var.api_management_name
  resource_group_name = var.resource_group_name
  api_name            = azurerm_api_management_api.negotiate.name
  display_name        = "negotiate preflight"
  method              = "OPTIONS"
  operation_id        = "negotiate-options"
  url_template        = "/"
}

resource "azurerm_api_management_api_operation" "post" {
  api_management_name = var.api_management_name
  resource_group_name = var.resource_group_name
  api_name            = azurerm_api_management_api.negotiate.name
  display_name        = "negotiate"
  method              = "POST"
  operation_id        = "negotiate-post"
  url_template        = "/"
}

resource "azurerm_api_management_api" "this" {
  name                  = "signalr-connect"
  resource_group_name   = var.resource_group_name
  api_management_name   = var.api_management_name
  revision              = "1"
  display_name          = "SignalR connect"
  protocols             = ["wss"]
  api_type              = "websocket"
  path                  = "signalr/client"
  service_url           = "wss://${var.signalr_hostname}/client/"
  subscription_required = true
}

output "wss_api_id" {
  value = azurerm_api_management_api.this.id
}

output "https_api_id" {
  value = azurerm_api_management_api.negotiate.id
}

output "wss_api_name" {
  value = azurerm_api_management_api.this.name
}

output "https_api_name" {
  value = azurerm_api_management_api.negotiate.name
}
