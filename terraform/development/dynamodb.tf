locals {
  defaultCapacity = 10
  minCapacity = 2
  maxCapacity = 50
  resourceNames = toset([
    "table/HousingRegister",
    "table/HousingRegister/index/HousingRegisterAll",
    "table/HousingRegister/index/HousingRegisterStatus",
    "table/HousingRegister/index/HousingRegisterAssignedTo",
    "table/HousingRegister/index/HousingRegisterStatusAssignedTo"
  ])
}

resource "aws_dynamodb_table" "housingregisterapi_dynamodb_table" {
    name                  = "HousingRegister"
    billing_mode          = "PROVISIONED"
    read_capacity         = locals.defaultCapacity
    write_capacity        = locals.defaultCapacity
    hash_key              = "id"

    attribute {
        name              = "id"
        type              = "S"
    }

    attribute {
        name              = "activeRecords"
        type              = "N"
    }

    attribute {
        name              = "sortKey"
        type              = "S"
    }

    attribute {
        name              = "status"
        type              = "S"
    }

    attribute {
        name              = "assignedTo"
        type              = "S"
    }
    attribute {
        name              = "statusAssigneeKey"
        type              = "S"
    }

    tags = {
        Name              = "housing-register-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }

    global_secondary_index {
        name              = "HousingRegisterAll"
        read_capacity     = locals.defaultCapacity
        write_capacity    = locals.defaultCapacity
        hash_key          = "activeRecords"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterStatus"
        read_capacity     = locals.defaultCapacity
        write_capacity    = locals.defaultCapacity
        hash_key          = "status"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterAssignedTo"
        read_capacity     = locals.defaultCapacity
        write_capacity    = locals.defaultCapacity
        hash_key          = "assignedTo"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }
    global_secondary_index {
        name              = "HousingRegisterStatusAssignedTo"
        read_capacity     = locals.defaultCapacity
        write_capacity    = locals.defaultCapacity
        hash_key          = "statusAssigneeKey"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_table_read_target" {
  for_each = locals.resourceNames
  max_capacity       = locals.maxCapacity
  min_capacity       = locals.minCapacity
  resource_id        = each.key
  scalable_dimension = "dynamodb:table:ReadCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_table_read_policy" {
  for_each = locals.resourceNames
  name               = "DynamoDBReadCapacityUtilization:${each.key}"
  policy_type        = "TargetTrackingScaling"
  resource_id        = each.key
  scalable_dimension = "dynamodb:table:ReadCapacityUnits"
  service_namespace  = "dynamodb"

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBReadCapacityUtilization"
    }

    target_value = 70
  }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_table_write_target" {
  for_each = locals.resourceNames
  max_capacity       = locals.maxCapacity
  min_capacity       = locals.minCapacity
  resource_id        = "table/HousingRegister"
  scalable_dimension = "dynamodb:table:WriteCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_table_write_policy" {
  for_each = locals.resourceNames
  name               = "DynamoDBWriteCapacityUtilization:${each.key}"
  policy_type        = "TargetTrackingScaling"
  resource_id        = each.key
  scalable_dimension = "dynamodb:table:WriteCapacityUnits"
  service_namespace  = "dynamodb"

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBWriteCapacityUtilization"
    }

    target_value = 70
  }
}
