resource "random_string" "suffix" {
  length  = 4
  special = false
  upper   = false
  numeric = false
}
