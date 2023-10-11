resource "aws_ecs_service" "this" {
  name            = var.name
  task_definition = aws_ecs_task_definition.this.arn
  cluster         = data.aws_ecs_cluster.this.arn

  desired_count = local.fixed_capacity_count

  deployment_maximum_percent         = var.deployment_maximum_percent
  deployment_minimum_healthy_percent = var.deployment_minimum_healthy_percent
  health_check_grace_period_seconds  = var.health_check_grace_period_seconds
  force_new_deployment               = var.force_new_deployment

  enable_execute_command = var.enable_execute_command

  service_registries {
    registry_arn = aws_service_discovery_service.this.arn
  }

  dynamic "network_configuration" {
    for_each = var.network_mode == "awsvpc" ? [1] : []

    content {
      assign_public_ip = false
      security_groups  = var.network_security_groups
      subnets          = var.network_subnets
    }
  }

  dynamic "load_balancer" {
    for_each = local.use_load_balancer ? [1] : []

    content {
      target_group_arn = aws_lb_target_group.this.0.arn
      container_name   = var.name
      container_port   = var.container_port
    }
  }

  dynamic "ordered_placement_strategy" {
    for_each = var.ordered_placement_strategy
    content {
      type  = lookup(ordered_placement_strategy.value, "type")
      field = lookup(ordered_placement_strategy.value, "field")
    }
  }

  lifecycle {
    ignore_changes = [desired_count]
  }

  tags = var.tags
}
