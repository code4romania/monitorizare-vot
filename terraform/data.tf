data "aws_availability_zones" "current" {}

data "aws_region" "current" {}

data "aws_route53_zone" "main" {
  zone_id = var.route_53_zone_id
}
