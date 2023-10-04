### IAM Resources
data "aws_iam_policy_document" "ecs" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "ecs" {
  name               = "${var.name}-ecs-instance"
  path               = var.iam_path
  assume_role_policy = data.aws_iam_policy_document.ecs.json
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role",
    "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore",
    "arn:aws:iam::aws:policy/CloudWatchAgentAdminPolicy"
  ]

  tags = var.tags
}

resource "aws_iam_instance_profile" "ecs" {
  name = "${var.name}-ecs-instance"
  role = aws_iam_role.ecs.name
}
