
resource "aws_eip" "nat_gateway" {
  domain = "vpc"
  tags = {
    Name = "${local.namespace}-nat-gateway"
  }
}

resource "aws_eip" "bastion" {
  instance = aws_instance.bastion.id
  domain   = "vpc"
  tags = {
    Name = "${local.namespace}-bastion"
  }
}
