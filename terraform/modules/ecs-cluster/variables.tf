variable "name" {
  description = "Name to be used throughout the resources"
  type        = string
}

variable "disk_path" {
  description = "Path for which disk size will be monitored in CloudWatch"
  type        = string
  default     = "/"
}

variable "additional_user_data_script" {
  description = "Additional user data script to be added to the default one"
  type        = string
  default     = ""
}

variable "vpc_id" {
  description = "The VPC in which to launch the resources"
  type        = string
}

variable "ecs_cloudwatch_log_retention" {
  description = "Specifies the number of days you want to retain log events in the specified log group. Possible values are: 1, 3, 5, 7, 14, 30, 60, 90, 120, 150, 180, 365, 400, 545, 731, 1827, and 3653."
  type        = number
}

variable "userdata_cloudwatch_log_retention" {
  description = "Specifies the number of days you want to retain log events in the specified log group. Possible values are: 1, 3, 5, 7, 14, 30, 60, 90, 120, 150, 180, 365, 400, 545, 731, 1827, and 3653."
  type        = number
}

variable "default_instance_type" {
  description = "The default instance type that will be used by the ECS cluster"
  type        = string
  default     = "m5.large"
}

variable "security_groups" {
  description = "A list of security group IDs to associate to the ECS instance/s"
  type        = list(string)
}

variable "min_size" {
  description = "The minimum size of the auto scaling group."
  type        = string
}

variable "max_size" {
  description = "The maximum size of the auto scaling group."
  type        = string
}

variable "ecs_subnets" {
  description = "A list of subnets in which ECS will launch instances"
  type        = list(string)
}

variable "capacity_rebalance" {
  description = "(Optional) Indicates whether capacity rebalance is enabled. Otherwise, capacity rebalance is disabled"
  type        = string
  default     = false
}

variable "on_demand_base_capacity" {
  description = "Absolute minimum amount of desired capacity that must be fulfilled by on-demand instances"
  type        = number
}

variable "on_demand_percentage_above_base_capacity" {
  description = "Percentage split between on-demand and Spot instances above the base on-demand capacity"
  type        = number
}

variable "spot_allocation_strategy" {
  description = "How to allocate capacity across the Spot pools. Valid values: lowest-price / capacity-optimized / price-capacity-optimized"
  type        = string
  default     = "price-capacity-optimized"
}

variable "launch_template_version" {
  description = "Version of the launch template"
  type        = string
  default     = "$Latest"
}

variable "protect_from_scale_in" {
  description = "Allows setting instance protection. The autoscaling group will not select instances with this setting for termination during scale in events."
  type        = bool
  default     = false
}

variable "enabled_metrics" {
  description = "A list of metrics to collect. The allowed values are GroupDesiredCapacity, GroupInServiceCapacity, GroupPendingCapacity, GroupMinSize, GroupMaxSize, GroupInServiceInstances, GroupPendingInstances, GroupStandbyInstances, GroupStandbyCapacity, GroupTerminatingCapacity, GroupTerminatingInstances, GroupTotalCapacity, GroupTotalInstances."
  type        = list(string)
  default     = ["GroupDesiredCapacity", "GroupInServiceCapacity", "GroupPendingCapacity", "GroupMinSize", "GroupMaxSize", "GroupInServiceInstances", "GroupPendingInstances", "GroupStandbyInstances", "GroupStandbyCapacity", "GroupTerminatingCapacity", "GroupTerminatingInstances", "GroupTotalCapacity", "GroupTotalInstances"]
}


variable "disable_api_termination" {
  description = "If true, enables EC2 Instance Termination Protection: https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/terminating-instances.html#Using_ChangingDisableAPITermination"
  type        = bool
  default     = false
}

variable "instance_types" {
  description = "ECS node instance types. Maps of pairs like `type = weight`. Where weight gives the instance type a proportional weight to other instance types."
  type        = map(any)
}

variable "managed_termination_protection" {
  type        = string
  description = "Enables or disables container-aware termination of instances in the auto scaling group when scale-in happens. Valid values are ENABLED and DISABLED."
  default     = "DISABLED"
}

variable "instance_warmup_period" {
  description = "Period of time, in seconds, after a newly launched Amazon EC2 instance can contribute to CloudWatch metrics for Auto Scaling group. If this parameter is omitted, the default value of 300 seconds is used."
  type        = string
  default     = "300"
}

variable "minimum_scaling_step_size" {
  type        = number
  description = "The minimum step adjustment size. A number between 1 and 10,000."
}

variable "maximum_scaling_step_size" {
  type        = number
  description = "The maximum step adjustment size. A number between 1 and 10,000."
}

variable "target_capacity" {
  type        = number
  description = "The target utilization for the capacity provider. A number between 1 and 100."
}

variable "iam_path" {
  description = "Path in which to create the IAM resources"
  type        = string
  default     = "/"
}

variable "tags" {
  type        = map(string)
  default     = {}
  description = "Tags applied to resources"
}

variable "asg_tags" {
  description = "A list of tag blocks. Each element should have keys named key, value, and propagate_at_launch."
  type        = list(map(string))
  default     = []
}

variable "spot_instance_pools" {
  description = "Number of Spot pools per availability zone to allocate capacity. EC2 Auto Scaling selects the cheapest Spot pools and evenly allocates Spot capacity across the number of Spot pools that you specify. Only available with spot_allocation_strategy set to lowest-price. Otherwise it must be set to 0, if it has been defined before"
  type        = number
  default     = 2
}

variable "service_discovery_domain" {
  type    = string
  default = "ecs.svc"
}
