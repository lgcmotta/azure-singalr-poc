variable "resource_suffix" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "location" {
  type = string
}

variable "storage_share_name" {
  type = string
}

variable "storage_share_files" {
  type = list(object({
    name   = string
    source = string
  }))
}

resource "azurerm_storage_account" "this" {
  name                     = "storage${var.resource_suffix}"
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_share" "this" {
  name               = var.storage_share_name
  storage_account_id = azurerm_storage_account.this.id
  quota              = 5
  depends_on         = [azurerm_storage_account.this]
}


resource "azurerm_storage_share_file" "this" {
  for_each         = { for _, file in var.storage_share_files : file.name => file }
  name             = each.key
  storage_share_id = azurerm_storage_share.this.url
  source           = each.value.source
  lifecycle {
    prevent_destroy       = false
    create_before_destroy = true
  }
}

output "name" {
  value = azurerm_storage_account.this.name
}

output "primary_access_key" {
  value = azurerm_storage_account.this.primary_access_key
}

output "storage_share_name" {
  value = azurerm_storage_share.this.name
}
