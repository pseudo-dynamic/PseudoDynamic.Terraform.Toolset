terraform {
  required_providers {
    debug = {
      source  = "registry.terraform.io/pseudo-dynamic/debug"
      version = "0.1.0"
    }
  }
}

resource "debug_validate" "default" {
    value = toset(["tf_first_csharp_second","tf_second_csharp_first"])
}