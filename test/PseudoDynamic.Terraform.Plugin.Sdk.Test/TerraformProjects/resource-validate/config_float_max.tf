terraform {
  required_providers {
    debug = {
      source  = "registry.terraform.io/pseudo-dynamic/debug"
      version = "0.1.0"
    }
  }
}

resource "debug_validate" "default" {
    value = 3.40282346638528859e+38
}