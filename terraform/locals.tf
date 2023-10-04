locals {
  namespace         = "votemonitor-${var.env}"
  availability_zone = data.aws_availability_zones.current.names[0]

  images = {
    api = {
      image = "code4romania/monitorizare-vot-api"
      tag   = "edge"
    }

    migrator = {
      image = "code4romania/monitorizare-vot-migrator"
      tag   = "edge"
    }

    seed = {
      image = "code4romania/monitorizare-vot-seed"
      tag   = "edge"
    }
  }

  ecs = {
    instance_types = {
      # "m5.large"  = ""
      # "m5a.large" = ""
      "t3a.medium" = ""
    }
  }

  db = {
    name           = "votemonitor"
    instance_class = "db.t4g.micro"
    # instance_class = "db.t4g.small"
  }

  networking = {
    cidr_block = "10.0.0.0/16"

    public_subnets = [
      "10.0.1.0/24",
      "10.0.2.0/24",
      "10.0.3.0/24"
    ]

    private_subnets = [
      "10.0.4.0/24",
      "10.0.5.0/24",
      "10.0.6.0/24"
    ]
  }
}
