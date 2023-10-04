locals {
  fixed_capacity       = var.min_capacity == var.max_capacity
  fixed_capacity_count = local.fixed_capacity ? var.min_capacity : 0

  use_load_balancer = var.lb_vpc_id != null && var.lb_listener_arn != null && length(var.lb_hosts) > 0

  logConfiguration = {
    "logDriver" : "awslogs",
    "options" : {
      "awslogs-group" : var.log_group_name,
      "awslogs-region" : data.aws_region.current.name,
      "awslogs-stream-prefix" : var.name
    }
  }

  mountPoints = replace(
    replace(jsonencode(var.mountPoints), "/\"1\"/", "true"),
    "/\"0\"/",
    "false",
  )

  volumesFrom = replace(
    replace(jsonencode(var.volumesFrom), "/\"1\"/", "true"),
    "/\"0\"/",
    "false",
  )

  container_definitions = jsonencode([{
    command                = length(var.command) == 0 ? null : var.command
    cpu                    = var.cpu == 0 ? null : var.cpu
    disableNetworking      = var.disableNetworking
    dnsSearchDomains       = length(var.dnsSearchDomains) == 0 ? null : var.dnsSearchDomains
    dnsServers             = length(var.dnsServers) == 0 ? null : var.dnsServers
    dockerLabels           = length(var.dockerLabels) == 0 ? null : var.dockerLabels
    dockerSecurityOptions  = length(var.dockerSecurityOptions) == 0 ? null : var.dockerSecurityOptions
    entryPoint             = length(var.entryPoint) == 0 ? null : var.entryPoint
    environment            = length(var.environment) == 0 ? null : var.environment
    essential              = var.essential
    extraHosts             = length(var.extraHosts) == 0 ? null : var.extraHosts
    healthCheck            = jsonencode(var.healthCheck) == "{}" ? null : var.healthCheck
    hostname               = var.hostname == "" ? null : var.hostname
    image                  = "${var.image_repo}:${var.image_tag}"
    interactive            = var.interactive
    links                  = length(var.links) == 0 ? null : var.links
    linuxParameters        = var.linuxParameters
    logConfiguration       = local.logConfiguration
    memory                 = var.memory == 0 ? null : var.memory
    memoryReservation      = var.container_memory_soft_limit == 0 ? null : var.container_memory_soft_limit
    mountPoints            = local.mountPoints == "[]" ? null : jsondecode(local.mountPoints)
    name                   = var.name == "" ? null : var.name
    portMappings           = var.container_port == null ? null : [{ containerPort = var.container_port }]
    privileged             = var.privileged
    pseudoTerminal         = var.pseudoTerminal
    readonlyRootFilesystem = var.readonlyRootFilesystem
    repositoryCredentials  = jsonencode(var.repositoryCredentials) == "{}" ? null : var.repositoryCredentials
    resourceRequirements   = length(var.resourceRequirements) == 0 ? null : var.resourceRequirements
    secrets                = length(var.secrets) == 0 ? null : var.secrets
    systemControls         = length(var.systemControls) == 0 ? null : var.systemControls
    ulimits                = length(var.ulimits) == 0 ? null : var.ulimits
    user                   = var.user == "" ? null : var.user
    volumesFrom            = local.volumesFrom == "[]" ? null : jsondecode(local.volumesFrom)
    workingDirectory       = var.workingDirectory
  }])
}
