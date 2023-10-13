module "ecs_api" {
  source = "./modules/ecs-service"

  depends_on = [
    module.ecs_cluster
  ]

  name         = "api-${var.env}"
  cluster_name = module.ecs_cluster.cluster_name
  min_capacity = 1
  max_capacity = 2

  image_repo = local.images.api.image
  image_tag  = local.images.api.tag

  use_load_balancer       = var.use_load_balancer
  lb_dns_name             = aws_lb.main.dns_name
  lb_zone_id              = aws_lb.main.zone_id
  lb_vpc_id               = aws_vpc.main.id
  lb_listener_arn         = aws_lb_listener.https.arn
  lb_hosts                = [var.domain_name]
  lb_domain_zone_id       = data.aws_route53_zone.main.zone_id
  lb_health_check_enabled = true
  lb_path                 = "/health"

  container_memory_soft_limit = 512
  container_memory_hard_limit = 1024

  log_group_name                 = module.ecs_cluster.log_group_name
  service_discovery_namespace_id = module.ecs_cluster.service_discovery_namespace_id

  container_port          = 80
  network_mode            = "awsvpc"
  network_security_groups = [aws_security_group.ecs.id]
  network_subnets         = [aws_subnet.private.0.id]

  task_role_arn          = aws_iam_role.ecs_task_role.arn
  enable_execute_command = var.enable_execute_command

  predefined_metric_type = "ECSServiceAverageCPUUtilization"
  target_value           = 80

  ordered_placement_strategy = [
    {
      type  = "binpack"
      field = "memory"
    }
  ]

  environment = [
    {
      name  = "Logging__LogLevel__Microsoft.EntityFrameworkCore"
      value = "Warning"
    },
    {
      name  = "ASPNETCORE_URLS"
      value = "http://+:80"
    },
    {
      name  = "ASPNETCORE_ENVIRONMENT"
      value = var.env
    },
    {
      name  = "JwtIssuerOptions__Audience"
      value = var.domain_name
    },
    {
      name  = "MobileSecurityOptions__InvalidCredentialsErrorMessage"
      value = "{ \"error\": \"A aparut o eroare la logarea in aplicatie. Va rugam sa verificati ca ati introdus corect numarul de telefon si codul de acces, iar daca eroarea persista va rugam contactati serviciul de suport la numarul 07......\" }"
    },
    {
      name  = "MobileSecurityOptions__LockDevice"
      value = tostring(false)
    },
    {
      name  = "ApplicationCacheOptions__Implementation"
      value = "MemoryDistributedCache"
    },
    {
      name  = "ApplicationCacheOptions__Hours"
      value = 0
    },
    {
      name  = "ApplicationCacheOptions__Minutes"
      value = 30
    },
    {
      name  = "ApplicationCacheOptions__Seconds"
      value = 0
    },
    {
      name  = "HashOptions__HashServiceType"
      value = "Hash"
    },
    {
      name  = "FileStorageType"
      value = "S3Service"
    },
    {
      name  = "AWS__Region"
      value = var.region
    },
    {
      name  = "S3StorageOptions__BucketName"
      value = module.s3_private.bucket
    },
    {
      name  = "S3StorageOptions__PresignedUrlExpirationInMinutes"
      value = 10000
    },
    {
      name  = "EnableHealthChecks"
      value = tostring(true)
    },
    {
      name  = "DefaultNgoOptions__DefaultNgoId"
      value = 1
    },
    {
      name  = "PollingStationsOptions__OverrideDefaultSorting"
      value = tostring(false)
    },
    # {
    #   name  = "PollingStationsOptions__CodeOfFirstToDisplayCounty"
    #   value = null
    # },
  ]

  secrets = [
    {
      name      = "SecretKey"
      valueFrom = aws_secretsmanager_secret.jwt_signing_key.arn
    },
    {
      name      = "ConnectionStrings__DefaultConnection"
      valueFrom = aws_secretsmanager_secret.rds.arn
    },
    {
      name      = "FirebaseServiceOptions__ServerKey"
      valueFrom = aws_secretsmanager_secret.firebase_serverkey.arn
    },
    {
      name      = "HashOptions__Salt"
      valueFrom = aws_secretsmanager_secret.hash_salt.arn
    },
    {
      name      = "LokiConfig__Uri"
      valueFrom = aws_secretsmanager_secret.loki_uri.arn
    },
    {
      name      = "LokiConfig__User"
      valueFrom = aws_secretsmanager_secret.loki_user.arn
    },
    {
      name      = "LokiConfig__Password"
      valueFrom = aws_secretsmanager_secret.loki_password.arn
    },

  ]

  allowed_secrets = [
    aws_secretsmanager_secret.firebase_serverkey.arn,
    aws_secretsmanager_secret.jwt_signing_key.arn,
    aws_secretsmanager_secret.hash_salt.arn,
    aws_secretsmanager_secret.rds.arn,
    aws_secretsmanager_secret.loki_uri.arn,
    aws_secretsmanager_secret.loki_user.arn,
    aws_secretsmanager_secret.loki_password.arn,
  ]
}

module "s3_private" {
  source = "./modules/s3"

  name = "${local.namespace}-private"
}
