resource "aws_vpc" "main" {
  cidr_block           = local.networking.cidr_block
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name = "${local.namespace}-vpc"
  }
}
