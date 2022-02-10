resource "aws_dynamodb_table" "housingregisterapi_dynamodb_table" {
    name                  = "HousingRegister"
    billing_mode          = "PROVISIONED"
    read_capacity         = 10
    write_capacity        = 10
    hash_key              = "id"

    autoscaling_enabled = true
    autoscaling_read = {
        target_value       = 70
        max_capacity       = 40
    }

    autoscaling_write = {
        target_value       = 70
        max_capacity       = 40
    }

    attribute {
        name              = "id"
        type              = "S"
    }

    tags = {
        Name              = "housing-register-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }
}
