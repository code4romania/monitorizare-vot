resource "aws_ecs_cluster" "ecs" {
  name = var.name
  tags = var.tags
}

### Spot Capacity
resource "aws_launch_template" "ecs" {
  name                   = "${var.name}-ecs-nodes"
  image_id               = data.aws_ami.this.id
  instance_type          = var.default_instance_type
  vpc_security_group_ids = var.security_groups
  user_data = base64encode(
    templatefile("${path.module}/assets/user_data.sh", {
      ecs_cluster_name            = var.name
      disk_path                   = var.disk_path
      maintenance_log_group_name  = aws_cloudwatch_log_group.userdata.name
      additional_user_data_script = var.additional_user_data_script
    })
  )
  disable_api_termination = var.disable_api_termination

  ebs_optimized = true

  block_device_mappings {
    device_name = "/dev/xvda"

    ebs {
      volume_size = 30
      volume_type = "gp3"
      encrypted   = true
    }
  }

  block_device_mappings {
    device_name = "/dev/xvdcz"

    ebs {
      volume_size = 22
      volume_type = "gp3"
      encrypted   = true
    }
  }

  iam_instance_profile {
    name = aws_iam_instance_profile.ecs.name
  }

  metadata_options {
    http_endpoint = "enabled"
    http_tokens   = "required"
  }

  tags = var.tags
}

resource "aws_autoscaling_group" "ecs" {
  name                  = "${var.name}-ecs-nodes"
  min_size              = var.min_size
  max_size              = var.max_size
  vpc_zone_identifier   = var.ecs_subnets
  capacity_rebalance    = var.capacity_rebalance
  protect_from_scale_in = var.protect_from_scale_in
  enabled_metrics       = var.enabled_metrics

  mixed_instances_policy {
    instances_distribution {
      on_demand_base_capacity                  = var.on_demand_base_capacity
      on_demand_percentage_above_base_capacity = var.on_demand_percentage_above_base_capacity
      spot_allocation_strategy                 = var.spot_allocation_strategy
      spot_instance_pools                      = var.spot_instance_pools
    }

    launch_template {
      launch_template_specification {
        launch_template_id = aws_launch_template.ecs.id
        version            = var.launch_template_version
      }

      dynamic "override" {
        for_each = var.instance_types
        content {
          instance_type = override.key
        }
      }
    }
  }

  dynamic "tag" {
    for_each = local.tags
    content {
      key                 = tag.value["key"]
      value               = tag.value["value"]
      propagate_at_launch = tag.value["propagate_at_launch"]
    }
  }
}

resource "aws_ecs_capacity_provider" "ecs" {
  name = "capacity-provider-${var.name}"

  auto_scaling_group_provider {
    auto_scaling_group_arn         = aws_autoscaling_group.ecs.arn
    managed_termination_protection = var.managed_termination_protection

    managed_scaling {
      status                    = "ENABLED"
      instance_warmup_period    = var.instance_warmup_period
      minimum_scaling_step_size = var.minimum_scaling_step_size
      maximum_scaling_step_size = var.maximum_scaling_step_size
      target_capacity           = var.target_capacity
    }
  }

  tags = var.tags
}

resource "aws_ecs_cluster_capacity_providers" "ecs" {
  cluster_name       = aws_ecs_cluster.ecs.name
  capacity_providers = [aws_ecs_capacity_provider.ecs.name]
}
