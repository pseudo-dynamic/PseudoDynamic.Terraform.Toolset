terraform {
  required_providers {
    debug = {
      source  = "registry.terraform.io/pseudo-dynamic/debug"
      version = "0.1.0"
    }
  }
}

resource "debug_validate" "default" {
    value = parseint("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",16)
}