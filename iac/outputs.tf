output "resource_group" {
  value = azurerm_resource_group.this.name
}

output "signalr_connection_string" {
  sensitive = true
  value     = azurerm_signalr_service.this.primary_connection_string
}

output "api_management_gateway" {
  value = azurerm_api_management.this.gateway_url
}

output "subscription_keys" {
  value     = module.subscription_keys.subscriptions
  sensitive = true
}
