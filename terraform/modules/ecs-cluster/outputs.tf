output "cluster_id" {
  value = aws_ecs_cluster.ecs.id
}

output "cluster_name" {
  value = aws_ecs_cluster.ecs.name
}

output "log_group_name" {
  value = aws_cloudwatch_log_group.ecs.name
}

output "asg_arn" {
  value = aws_autoscaling_group.ecs.arn
}

output "asg_name" {
  value = aws_autoscaling_group.ecs.name
}

output "service_discovery_namespace_id" {
  value = aws_service_discovery_private_dns_namespace.ecs.id
}
