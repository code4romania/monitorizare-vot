locals {
  tags = concat(
    [
      {
        key                 = "Name"
        value               = "${var.name}-ecs-node"
        propagate_at_launch = true
      },
      {
        key                 = "AmazonECSManaged"
        value               = "true"
        propagate_at_launch = true
      }
    ],
    var.asg_tags,
    local.tags_asg_format,
  )

  tags_asg_format = null_resource.tags_as_list_of_maps.*.triggers
}

resource "null_resource" "tags_as_list_of_maps" {
  count = length(keys(var.tags))

  triggers = {
    "key"                 = keys(var.tags)[count.index]
    "value"               = values(var.tags)[count.index]
    "propagate_at_launch" = "true"
  }
}
