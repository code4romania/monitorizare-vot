resource "aws_service_discovery_service" "this" {
  name = var.name

  dynamic "dns_config" {
    for_each = var.service_discovery_namespace_id == null ? [] : [1]

    content {
      namespace_id   = var.service_discovery_namespace_id
      routing_policy = "MULTIVALUE"

      dns_records {
        ttl  = 10
        type = "A"
      }
    }
  }
}
