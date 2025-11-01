variable "resource_group_name" {
  type = string
}

variable "api_management_name" {
  type = string
}

variable "keys" {
  type = list(object({
    name         = string
    display_name = string
  }))
}

resource "azurerm_api_management_subscription" "this" {
  for_each            = { for _, key in var.keys : key.name => key }
  resource_group_name = var.resource_group_name
  api_management_name = var.api_management_name
  display_name        = each.value.display_name
  allow_tracing       = true
  state               = "active"
}

output "subscriptions" {
  value = {
    for _, sk in azurerm_api_management_subscription.this :
    sk.display_name => {
      primary_key   = sk.primary_key
      secondary_key = sk.secondary_key
    }
  }
}
