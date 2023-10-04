resource "aws_appautoscaling_target" "this" {
  count = local.fixed_capacity ? 0 : 1

  service_namespace  = "ecs"
  resource_id        = "service/${data.aws_ecs_cluster.this.cluster_name}/${aws_ecs_service.this.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  min_capacity       = var.min_capacity
  max_capacity       = var.max_capacity
}

resource "aws_appautoscaling_policy" "this" {
  count = local.fixed_capacity ? 0 : 1

  name               = "${var.name}-target-scaling"
  resource_id        = aws_appautoscaling_target.this.0.resource_id
  scalable_dimension = aws_appautoscaling_target.this.0.scalable_dimension
  service_namespace  = aws_appautoscaling_target.this.0.service_namespace
  policy_type        = "TargetTrackingScaling"

  target_tracking_scaling_policy_configuration {
    target_value       = var.target_value
    scale_in_cooldown  = var.scale_in_cooldown
    scale_out_cooldown = var.scale_out_cooldown

    predefined_metric_specification {
      predefined_metric_type = var.predefined_metric_type
    }
  }
}
