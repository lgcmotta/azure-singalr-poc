variable "api_management_url" {
  type = string
}

variable "swa_hostname" {
  type = string
}

locals {
  api_management_host = trimsuffix(replace(var.api_management_url, "https://", ""), "/")

  content = replace(
    replace(
      file("${path.module}/realm.json"),
      "__API_MANAGEMENT_HOST__",
      local.api_management_host
    ),
    "__STATIC_WEB_APP_HOST__",
    var.swa_hostname
  )
}

resource "local_file" "this" {
  filename = "${path.module}/realm.rendered.json"
  content  = local.content
}

output "filename" {
  value = local_file.this.filename
}

output "api_management_host" {
  value = local.api_management_host
}
