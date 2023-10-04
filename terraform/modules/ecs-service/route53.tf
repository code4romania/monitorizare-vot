# A record
resource "aws_route53_record" "ipv4" {
  count = length(var.lb_hosts)

  zone_id = var.lb_domain_zone_id
  name    = var.lb_hosts[count.index]
  type    = "A"

  alias {
    name                   = var.lb_dns_name
    zone_id                = var.lb_zone_id
    evaluate_target_health = true
  }
}

# AAAA record
resource "aws_route53_record" "ipv6" {
  count = length(var.lb_hosts)

  zone_id = var.lb_domain_zone_id
  name    = var.lb_hosts[count.index]
  type    = "AAAA"

  alias {
    name                   = var.lb_dns_name
    zone_id                = var.lb_zone_id
    evaluate_target_health = true
  }
}
