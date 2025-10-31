output "resource_group" {
  value = azurerm_resource_group.this.name
}

output "signalr_connection_string" {
  sensitive = true
  value     = azurerm_signalr_service.this.primary_connection_string
}

# output "api_management_gateway" {
#   value = azurerm_api_management.this.gateway_url
# }

# output "container_app_fqdn" {
#   value = azurerm_container_app.this.latest_revision_fqdn
# }
