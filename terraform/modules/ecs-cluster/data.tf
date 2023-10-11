### Latest Amazon Linux ECS Optimized AMI
data "aws_ami" "this" {
  most_recent = true

  owners = ["amazon"]

  filter {
    name   = "name"
    values = ["amzn2-ami-ecs-kernel-5.10-hvm-*-ebs"]
  }

  filter {
    name   = "owner-alias"
    values = ["amazon"]
  }

  filter {
    name   = "architecture"
    values = ["x86_64"]
  }
}
