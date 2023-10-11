output "task_arn" {
  value = aws_ecs_task_definition.this.arn
}

output "service_name" {
  value = aws_ecs_service.this.name
}

output "service_arn" {
  value = aws_ecs_service.this.id
}
