terraform {
  required_providers {
    debug = {
      source  = "pseudo-dynamic/debug"
      version = "0.1.0"
    }
  }
}

resource "debug_empty" "default" {}