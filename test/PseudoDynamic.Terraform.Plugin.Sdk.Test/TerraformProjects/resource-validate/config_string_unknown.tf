terraform {
  required_providers {
    debug = {
      source  = "registry.terraform.io/pseudo-dynamic/debug"
      version = "0.1.0"
    }

    value = {
      source  = "registry.terraform.io/pseudo-dynamic/value"
      version = "0.5.0"
    }
  }
}

resource "value_promise" "default" {
    value = null
}

resource "debug_validate" "default" {
    value = value_promise.default.result
}