# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

terraform {
    required_providers {
        aws = {
            source  = "hashicorp/aws"
            version = "~> 3.0"
        }
    }
}

provider "aws" {
    region = "eu-west-2"
}

data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

locals {
    parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-housing-development"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/housing-register-api/state"
  }
}

resource "aws_sns_topic" "housingregister_topic" {
  name                        = "housingregister.fifo"
  fifo_topic                  = true
  content_based_deduplication = true
  kms_master_key_id           = "alias/aws/sns"
}

resource "aws_ssm_parameter" "housingregister_sns_arn" {
  name  = "/sns-topic/development/housingregister/arn"
  type  = "String"
  value = aws_sns_topic.housingregister_topic.arn
}

module "housingregister_api_cloudwatch_dashboard" {
  source              = "git::git@github.com:LBHackney-IT/aws-hackney-common-terraform.git//modules/cloudwatch/dashboards/api-dashboard"
  environment_name    = var.environment_name
  api_name            = "housing-register-api"
  sns_topic_name      = aws_sns_topic.housingregister_topic.name
  dynamodb_table_name = aws_dynamodb_table.housingregisterapi_dynamodb_table.name
}

resource "aws_s3_bucket" "housingregister_bucket" {
  bucket = "housingregister-exports-${var.environment_name}-bucket"
  acl    = "private"

  tags = {
    Name = "housingregister-exports-${var.environment_name}-bucket"
    Environment = var.environment_name
  }
}

resource "aws_sns_topic" "novaletexport_topic" {
  name                        = "novaletexport-sns-topic"
  fifo_topic                  = false
  kms_master_key_id           = "alias/aws/sns"
}

resource "aws_ssm_parameter" "novalet_sns_arn" {
  name  = "/sns-topic/development/novaletexport-sns-topic/arn"
  type  = "String"
  value = aws_sns_topic.novaletexport_topic.arn
}
