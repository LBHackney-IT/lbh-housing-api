resource "aws_dynamodb_table" "housingregisterapi_dynamodb_table" {
    name         = "HousingRegister"
    billing_mode = "PAY_PER_REQUEST"
    hash_key     = "id"

    attribute {
        name = "id"
        type = "S"
    }

    attribute {
        name = "activeRecord"
        type = "N"
    }

    attribute {
        name = "sortKey"
        type = "S"
    }

    attribute {
        name = "status"
        type = "S"
    }

    attribute {
        name = "reference"
        type = "S"
    }

    attribute {
        name = "assignedTo"
        type = "S"
    }

    attribute {
        name = "statusAssigneeKey"
        type = "S"
    }

    tags = {
        Name              = "housing-register-api-${var.environment_name}"
        Environment       = "stg"
        Confidentiality   = "Internal"
        terraform-managed = true
        project_name      = var.project_name
        Application       = "Housing Register"
        TeamEmail         = "developementteam@hackney.gov.uk"
        BackupPolicy      = "Stg"
    }

    global_secondary_index {
        name            = "HousingRegisterAll"
        hash_key        = "activeRecord"
        range_key       = "sortKey"
        projection_type = "ALL"
    }

    global_secondary_index {
        name            = "HousingRegisterStatus"
        hash_key        = "status"
        range_key       = "sortKey"
        projection_type = "ALL"
    }

    global_secondary_index {
        name            = "HousingRegisterAssignedTo"
        hash_key        = "assignedTo"
        range_key       = "sortKey"
        projection_type = "ALL"
    }

    global_secondary_index {
        name            = "HousingRegisterStatusAssignedTo"
        hash_key        = "statusAssigneeKey"
        range_key       = "sortKey"
        projection_type = "ALL"
    }

    global_secondary_index {
        name            = "HousingRegisterReference"
        hash_key        = "reference"
        range_key       = "sortKey"
        projection_type = "ALL"
    }
}
