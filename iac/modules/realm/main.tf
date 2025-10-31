variable "api_management_url" {
  type = string
}

locals {
  api_management_host = trimsuffix(replace(var.api_management_url, "https://", ""), "/")
}

resource "local_file" "this" {
  filename = "${path.module}/realm.rendered.json"
  content  = replace(file("${path.module}/realm.json"), "__API_MANAGEMENT_HOST__", local.api_management_host)
}

output "filename" {
  value = local_file.this.filename
}

output "api_management_host" {
  value = local.api_management_host
}
