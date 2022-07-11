data "aws_vpc" "development_vpc" {
  tags = {
    Name = "housing-dev"
  }
}

data "aws_subnet_ids" "development" {
  vpc_id = data.aws_vpc.development_vpc.id
  filter {
    name   = "tag:Type"
    values = ["private"]
  }
}

module "elasticsearch_db_development" {
  source              = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/elasticsearch"
  vpc_id              = data.aws_vpc.development_vpc.id
  environment_name    = var.environment_name
  port                = 443
  domain_name         = "${lower(var.project_name)}-api-es"
  subnet_ids          = [tolist(data.aws_subnet_ids.development.ids)[0]]
  project_name        = lower(var.project_name)
  es_version          = "7.10"
  encrypt_at_rest     = "true"
  instance_type       = "t3.small.elasticsearch"
  instance_count      = "2"
  ebs_enabled         = "true"
  ebs_volume_size     = "10"
  region              = data.aws_region.current.name
  account_id          = data.aws_caller_identity.current.account_id
  create_service_role = "false"
}

resource "aws_ssm_parameter" "search_elasticsearch_domain" {
  name  = "${lower(var.project_name)}/${var.environment_name}/elasticsearch-domain"
  type  = "String"
  value = "https://${module.elasticsearch_db_development.es_endpoint_url}"
}