variable "resource_suffix" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

resource "azurerm_static_web_app" "this" {
  name                = "signalr-poc-web-${var.resource_suffix}"
  resource_group_name = var.resource_group_name
  location            = var.location
  sku_tier            = "Free"
  sku_size            = "Free"
}

output "hostname" {
  value = azurerm_static_web_app.this.default_host_name
}

output "api_key" {
  value     = azurerm_static_web_app.this.api_key
  sensitive = true
}
