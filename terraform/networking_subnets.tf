resource "aws_subnet" "public" {
  count                   = length(data.aws_availability_zones.current.names)
  vpc_id                  = aws_vpc.main.id
  cidr_block              = element(local.networking.public_subnets, count.index)
  availability_zone       = element(data.aws_availability_zones.current.names, count.index)
  map_public_ip_on_launch = true

  tags = {
    Name   = "${local.namespace}-public"
    access = "public"
  }
}

resource "aws_subnet" "private" {
  count                   = length(data.aws_availability_zones.current.names)
  vpc_id                  = aws_vpc.main.id
  cidr_block              = element(local.networking.private_subnets, count.index)
  availability_zone       = element(data.aws_availability_zones.current.names, count.index)
  map_public_ip_on_launch = false

  tags = {
    Name   = "${local.namespace}-private"
    access = "private"
  }
}

resource "aws_db_subnet_group" "db_subnet_group" {
  name       = "${local.namespace}-db-private"
  subnet_ids = aws_subnet.private.*.id

  tags = {
    access = "private"
  }
}
