resource "aws_db_instance" "main" {
  identifier          = local.namespace
  db_name             = local.db.name
  instance_class      = local.db.instance_class
  publicly_accessible = false
  multi_az            = false
  deletion_protection = true

  availability_zone = local.availability_zone

  username = local.db.name
  password = random_password.database.result
  port     = 5432

  iam_database_authentication_enabled = true

  engine                      = "postgres"
  engine_version              = "15.4"
  allow_major_version_upgrade = false
  auto_minor_version_upgrade  = true

  # storage
  allocated_storage     = 20
  max_allocated_storage = 100
  storage_type          = "gp3"
  storage_encrypted     = true

  # backup
  backup_retention_period   = 30
  backup_window             = "04:00-04:30"
  copy_tags_to_snapshot     = true
  skip_final_snapshot       = var.env != "production"
  final_snapshot_identifier = "${local.namespace}-final-snapshot"

  maintenance_window = "Mon:04:45-Mon:06:00"

  performance_insights_enabled = false
  # performance_insights_retention_period = 7

  db_subnet_group_name   = aws_db_subnet_group.db_subnet_group.name
  vpc_security_group_ids = [aws_security_group.database.id]
}

resource "aws_security_group" "database" {
  name        = "${local.namespace}-rds"
  description = "Inbound - Security Group attached to the RDS instance"
  vpc_id      = aws_vpc.main.id

  ingress {
    from_port   = 5432
    to_port     = 5432
    protocol    = "tcp"
    cidr_blocks = [local.networking.cidr_block]
  }

  egress {
    from_port = 5432
    to_port   = 5432
    protocol  = "tcp"
    self      = true
  }
}

resource "random_password" "database" {
  length  = 32
  special = false

  lifecycle {
    ignore_changes = [
      length,
      special
    ]
  }
}

resource "aws_secretsmanager_secret" "rds" {
  name = "${local.namespace}-db_credentials-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "rds" {
  secret_id = aws_secretsmanager_secret.rds.id
  secret_string = format("Server=%s;Port=%d;Database=%s;User Id=%s;Password=%s;",
    aws_db_instance.main.address,
    aws_db_instance.main.port,
    aws_db_instance.main.db_name,
    aws_db_instance.main.username,
    aws_db_instance.main.password,
  )
}
