### IAM resources
data "aws_iam_policy_document" "ecs_task_assume" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "ecs_secret_policy" {
  statement {
    actions   = ["secretsmanager:GetSecretValue"]
    resources = var.allowed_secrets
  }
}

data "aws_iam_policy_document" "ssm_policy" {
  statement {
    actions = [
      "ssmmessages:CreateControlChannel",
      "ssmmessages:CreateDataChannel",
      "ssmmessages:OpenControlChannel",
      "ssmmessages:OpenDataChannel"
    ]
    resources = ["*"]
  }
}

resource "aws_iam_role" "ecs_task_execution_role" {
  name                = "${var.name}-ecs-task-execution-role"
  assume_role_policy  = data.aws_iam_policy_document.ecs_task_assume.json
  managed_policy_arns = concat(["arn:aws:iam::aws:policy/service-role/AmazonEC2ContainerServiceforEC2Role"], var.managed_policies)

  dynamic "inline_policy" {
    for_each = var.allowed_secrets == null ? [] : [1]

    content {
      name   = "SecretsPolicy"
      policy = data.aws_iam_policy_document.ecs_secret_policy.json
    }
  }

  dynamic "inline_policy" {
    for_each = var.enable_execute_command ? [1] : []

    content {
      name   = "SSMPolicy"
      policy = data.aws_iam_policy_document.ssm_policy.json
    }
  }

  dynamic "inline_policy" {
    for_each = var.additional_policy == null ? [] : [1]

    content {
      name   = "AdditionalPolicy"
      policy = var.additional_policy
    }
  }

  tags = var.tags
}
