variable "env" {
  description = "Environment"
  type        = string

  validation {
    condition     = contains(["production", "staging", "development"], var.env)
    error_message = "Allowed values for env are \"production\", \"staging\" or \"development\"."
  }
}

variable "slug" {
  description = "Deployment slug"
  type        = string
}

variable "region" {
  description = "Region"
  type        = string
}

variable "domain_name" {
  description = "Domain name used by the application. Must belong to the Route 53 zone defined in `route_53_zone_id`."
  type        = string
}

variable "route_53_zone_id" {
  type = string
}

variable "bastion_public_key" {
  description = "Public SSH key used to connect to the bastion"
  type        = string
}

variable "firebase_serverkey" {
  description = "Firebase server key"
  type        = string
}

variable "use_load_balancer" {
  type    = bool
  default = true
}

variable "create_iam_service_linked_role" {
  description = "Whether to create `AWSServiceRoleForECS` service-linked role. Set it to `false` if you already have an ECS cluster created in the AWS account and AWSServiceRoleForECS already exists."
  type        = bool
  default     = true
}

variable "enable_execute_command" {
  description = "Enable aws ecs execute_command"
  type        = bool
  default     = false
}

variable "docker_tag" {
  description = "Docker image tag"
  type        = string
}

variable "loki_uri" {
  description = "Loki uri"
  type        = string
  default     = null
}

variable "loki_user" {
  description = "Loki user"
  type        = string
  default     = null
}

variable "loki_password" {
  description = "Loki password"
  type        = string
  default     = null
}

variable "invalid_credentials_error_message" {
  description = "Error message to show when invalid credentials are provided"
  type        = string
  default     = "{ \"error\": \"A apărut o eroare la logarea în aplicație. Vă rugăm să verificați că ați introdus corect numărul de telefon și codul de acces, iar dacă eroarea persistă, vă rugăm contactați serviciul de suport la numarul 07......\" }"
}
