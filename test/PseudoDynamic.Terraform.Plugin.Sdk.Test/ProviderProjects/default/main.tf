terraform {
  required_providers {
    debug = {
      source  = "pseudo-dynamic/debug"
      version = "0.1.0"
    }
  }

  provider_meta "debug" {
    string = "string"
    number = 1
    bool = true
  }
}

locals {
    object = {
        string = "string"
        number = 1
        bool = true    
    }

    object_including_ranges = {
        list = tolist([local.object,local.object])
        set = toset([local.object,local.object])

        map = tomap({
            "one" = "1"
            "two" = "2"
        })
    }
}

provider "debug" {
    string = local.object.string
    number = local.object.number
    bool = local.object.bool
    list = local.object_including_ranges.list
    set = local.object_including_ranges.set
    map = local.object_including_ranges.map
    
    single_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    list_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    list_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    set_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    set_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    map_nested "one" {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }
    
    map_nested "two" {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }
}

resource "debug_empty" "default" {
    string = local.object.string
    number = local.object.number
    bool = local.object.bool
    list = local.object_including_ranges.list
    set = local.object_including_ranges.set
    map = local.object_including_ranges.map
    
    single_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    list_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    list_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    set_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    set_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    map_nested "one" {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }
    
    map_nested "two" {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }
}

data "debug_empty" "default" {
    string = local.object.string
    number = local.object.number
    bool = local.object.bool
    list = local.object_including_ranges.list
    set = local.object_including_ranges.set
    map = local.object_including_ranges.map
    
    single_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    list_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    list_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    set_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    set_nested {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }

    map_nested "one" {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }
    
    map_nested "two" {
        string = local.object.string
        number = local.object.number
        bool = local.object.bool
        list = local.object_including_ranges.list
        set = local.object_including_ranges.set
        map = local.object_including_ranges.map
    }
}