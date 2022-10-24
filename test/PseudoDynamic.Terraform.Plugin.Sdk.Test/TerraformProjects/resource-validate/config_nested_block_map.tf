terraform {
  required_providers {
    debug = {
      source  = "registry.terraform.io/pseudo-dynamic/debug"
      version = "0.1.0"
    }
  }
}

resource "debug_validate" "default" {
    value "first_nested_block" {
        value = "first_nested_block_attribute"
    }

    value "second_nested_block" {
        value = "second_nested_block_attribute"
    }
}