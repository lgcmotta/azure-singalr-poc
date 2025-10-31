variable "resource_group_name" {
  type = string
}

variable "container_app_fqdn" {
  type = string
}

variable "api_management_name" {
  type = string
}

resource "azurerm_api_management_api" "this" {
  name                  = "keycloak"
  display_name          = "Keycloak"
  api_management_name   = var.api_management_name
  resource_group_name   = var.resource_group_name
  revision              = "1"
  path                  = "keycloak"
  protocols             = ["https"]
  service_url           = "https://${var.container_app_fqdn}/keycloak"
  subscription_required = false
}

resource "azurerm_api_management_api_operation" "this" {
  for_each            = toset(["get", "post", "put", "delete", "patch", "options", "head"])
  api_management_name = azurerm_api_management_api.this.api_management_name
  resource_group_name = azurerm_api_management_api.this.resource_group_name
  api_name            = azurerm_api_management_api.this.name
  operation_id        = "kc-${each.key}"
  display_name        = "${upper(each.value)} /*"
  method              = upper(each.value)
  template_parameter {
    name     = "path"
    required = false
    type     = "string"
  }
  url_template = "/{*path}"
  depends_on   = [azurerm_api_management_api.this]
}
