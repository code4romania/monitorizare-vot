resource "random_string" "secrets_suffix" {
  length  = 8
  special = false
  upper   = false
  numeric = false

  lifecycle {
    ignore_changes = [
      length,
      special,
      upper,
      numeric,
    ]
  }
}

# JWT_SIGNING_KEY
resource "random_password" "jwt_signing_key" {
  length  = 64
  special = false

  lifecycle {
    ignore_changes = [
      length,
      special
    ]
  }
}

resource "aws_secretsmanager_secret" "jwt_signing_key" {
  name = "${local.namespace}-jwt_signing_key-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "jwt_signing_key" {
  secret_id     = aws_secretsmanager_secret.jwt_signing_key.id
  secret_string = random_password.jwt_signing_key.result
}


# HASH_SALT
resource "random_password" "hash_salt" {
  length  = 32
  special = false

  lifecycle {
    ignore_changes = [
      length,
      special
    ]
  }
}

resource "aws_secretsmanager_secret" "hash_salt" {
  name = "${local.namespace}-hash_salt-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "hash_salt" {
  secret_id     = aws_secretsmanager_secret.hash_salt.id
  secret_string = random_password.hash_salt.result
}


# FIREBASE_SERVERKEY
resource "aws_secretsmanager_secret" "firebase_serverkey" {
  name = "${local.namespace}-firebase_serverkey-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "firebase_serverkey" {
  secret_id     = aws_secretsmanager_secret.firebase_serverkey.id
  secret_string = var.firebase_serverkey
}

# LOKI URI
resource "aws_secretsmanager_secret" "loki_uri" {
  name = "${local.namespace}-loki_uri-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "loki_uri" {
  secret_id     = aws_secretsmanager_secret.loki_uri.id
  secret_string = var.loki_uri
}

# LOKI USER
resource "aws_secretsmanager_secret" "loki_user" {
  name = "${local.namespace}-loki_user-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "loki_user" {
  secret_id     = aws_secretsmanager_secret.loki_user.id
  secret_string = var.loki_user
}

# LOKI PASSWORD
resource "aws_secretsmanager_secret" "loki_password" {
  name = "${local.namespace}-loki_password-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "loki_password" {
  secret_id     = aws_secretsmanager_secret.loki_password.id
  secret_string = var.loki_password
}