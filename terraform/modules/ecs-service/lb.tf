resource "aws_lb_target_group" "this" {
  count = var.use_load_balancer ? 1 : 0

  name        = var.name
  port        = var.container_port
  protocol    = "HTTP"
  vpc_id      = var.lb_vpc_id
  target_type = "ip"

  health_check {
    enabled             = var.lb_health_check_enabled
    healthy_threshold   = var.lb_healthy_threshold
    interval            = var.lb_interval
    protocol            = var.lb_protocol
    matcher             = var.lb_matcher
    timeout             = var.lb_timeout
    path                = var.lb_path
    unhealthy_threshold = var.lb_unhealthy_threshold
  }
}

resource "aws_lb_listener_rule" "routing" {
  count = var.use_load_balancer ? 1 : 0

  listener_arn = var.lb_listener_arn

  action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.this.0.arn
  }

  condition {
    host_header {
      values = var.lb_hosts
    }
  }
}
