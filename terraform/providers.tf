terraform {
  required_version = "~> 1.5"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.16"
    }
  }

  backend "s3" {
    bucket = "code4-terraform-state"
    key    = "votemonitor/terraform.tfstate"
    region = "eu-west-1"
  }
}

provider "aws" {
  region = var.region

  default_tags {
    tags = {
      app = "votemonitor"
      env = var.env
    }
  }
}
