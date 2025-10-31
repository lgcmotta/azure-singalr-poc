variable "aws" {
  type = object({
    bucket = string
    key    = string
    region = string
  })
}

variable "azure" {
  type = object({
    subscription_id = string
    tenant_id       = string
  })
}

variable "keycloak" {
  type = object({
    storage_name   = string
    realm_filename = string
    volume_name    = string
  })
}

variable "keycloak_admin_password" {
  type      = string
  sensitive = true
}
