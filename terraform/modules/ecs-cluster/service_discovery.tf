resource "aws_service_discovery_private_dns_namespace" "ecs" {
  name = var.service_discovery_domain
  vpc  = var.vpc_id
}
