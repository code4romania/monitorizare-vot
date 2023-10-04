resource "aws_ecs_task_definition" "this" {
  container_definitions = local.container_definitions
  execution_role_arn    = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn         = var.task_role_arn
  family                = var.name
  ipc_mode              = var.ipc_mode
  network_mode          = var.network_mode
  pid_mode              = var.pid_mode

  memory = var.container_memory_hard_limit

  dynamic "placement_constraints" {
    for_each = var.placement_constraints
    content {
      expression = lookup(placement_constraints.value, "expression", null)
      type       = placement_constraints.value.type
    }
  }

  requires_compatibilities = var.requires_compatibilities

  dynamic "volume" {
    for_each = var.volumes

    content {
      host_path = lookup(volume.value, "host_path", null)
      name      = volume.value.name

      dynamic "docker_volume_configuration" {
        for_each = lookup(volume.value, "docker_volume_configuration", [])

        content {
          autoprovision = lookup(docker_volume_configuration.value, "autoprovision", null)
          driver        = lookup(docker_volume_configuration.value, "driver", null)
          driver_opts   = lookup(docker_volume_configuration.value, "driver_opts", null)
          labels        = lookup(docker_volume_configuration.value, "labels", null)
          scope         = lookup(docker_volume_configuration.value, "scope", null)
        }
      }

      dynamic "efs_volume_configuration" {
        for_each = lookup(volume.value, "efs_volume_configuration", [])

        content {
          file_system_id     = element(lookup(efs_volume_configuration.value, "file_system_id", null), 0)
          root_directory     = lookup(efs_volume_configuration.value, "root_directory", null)
          transit_encryption = lookup(efs_volume_configuration.value, "transit_encryption", null)
        }
      }
    }
  }

  tags = var.tags
}
