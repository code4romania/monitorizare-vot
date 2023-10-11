module "ecs_migrator" {
  source = "./modules/ecs-service"

  depends_on = [
    module.ecs_cluster
  ]

  name         = "migrator-${var.env}"
  cluster_name = module.ecs_cluster.cluster_name
  min_capacity = 0
  max_capacity = 0

  image_repo = local.images.migrator.image
  image_tag  = local.images.migrator.tag

  container_memory_soft_limit = 128
  container_memory_hard_limit = 256

  log_group_name                 = module.ecs_cluster.log_group_name
  service_discovery_namespace_id = module.ecs_cluster.service_discovery_namespace_id

  network_mode            = "awsvpc"
  network_security_groups = [aws_security_group.ecs.id]
  network_subnets         = [aws_subnet.private.0.id]

  #   task_role_arn          = aws_iam_role.ecs_task_role.arn
  #   enable_execute_command = var.enable_execute_command

  #   ordered_placement_strategy = [
  #     {
  #       type  = "binpack"
  #       field = "memory"
  #     }
  #   ]

  environment = [
    {
      name  = "Logging__LogLevel__Microsoft.EntityFrameworkCore"
      value = "Warning"
    },
  ]

  secrets = [
    {
      name      = "ConnectionStrings__DefaultConnection"
      valueFrom = aws_secretsmanager_secret.rds.arn
    },
  ]

  allowed_secrets = [
    aws_secretsmanager_secret.rds.arn,
  ]
}
