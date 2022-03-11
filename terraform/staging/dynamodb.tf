locals {
  defaultCapacity = 50
  minCapacity = 2
  maxCapacity = 50
  indexNames = toset([
    "table/HousingRegister/index/HousingRegisterAll",
    "table/HousingRegister/index/HousingRegisterStatus",
    "table/HousingRegister/index/HousingRegisterAssignedTo",
    "table/HousingRegister/index/HousingRegisterStatusAssignedTo",
    "table/HousingRegister/index/HousingRegisterReference"
  ])
}

resource "aws_dynamodb_table" "housingregisterapi_dynamodb_table" {
    name                  = "HousingRegister"
    billing_mode          = "PROVISIONED"
    read_capacity         = local.defaultCapacity
    write_capacity        = local.defaultCapacity
    hash_key              = "id"

    attribute {
        name              = "id"
        type              = "S"
    }

    attribute {
        name              = "activeRecord"
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
        name              = "reference"
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
        read_capacity     = local.defaultCapacity
        write_capacity    = local.defaultCapacity
        hash_key          = "activeRecord"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterStatus"
        read_capacity     = local.defaultCapacity
        write_capacity    = local.defaultCapacity
        hash_key          = "status"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterAssignedTo"
        read_capacity     = local.defaultCapacity
        write_capacity    = local.defaultCapacity
        hash_key          = "assignedTo"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterStatusAssignedTo"
        read_capacity     = local.defaultCapacity
        write_capacity    = local.defaultCapacity
        hash_key          = "statusAssigneeKey"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterReference"
        read_capacity     = local.defaultCapacity
        write_capacity    = local.defaultCapacity
        hash_key          = "reference"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_table_read_target" {
  max_capacity       = local.maxCapacity
  min_capacity       = local.minCapacity
  resource_id        = "table/HousingRegister"
  scalable_dimension = "dynamodb:table:ReadCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_table_read_policy" {
  name               = "DynamoDBReadCapacityUtilization:table/HousingRegister"
  policy_type        = "TargetTrackingScaling"
  resource_id        = "table/HousingRegister"
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
  max_capacity       = local.maxCapacity
  min_capacity       = local.minCapacity
  resource_id        = "table/HousingRegister"
  scalable_dimension = "dynamodb:table:WriteCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_table_write_policy" {
  name               = "DynamoDBWriteCapacityUtilization:table/HousingRegister"
  policy_type        = "TargetTrackingScaling"
  resource_id        = "table/HousingRegister"
  scalable_dimension = "dynamodb:table:WriteCapacityUnits"
  service_namespace  = "dynamodb"

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBWriteCapacityUtilization"
    }

    target_value = 70
  }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_index_read_target" {
  for_each = local.indexNames
  max_capacity       = local.maxCapacity
  min_capacity       = local.minCapacity
  resource_id        = each.key
  scalable_dimension = "dynamodb:index:ReadCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_index_read_policy" {
  for_each = local.indexNames
  name               = "DynamoDBReadCapacityUtilization:${each.key}"
  policy_type        = "TargetTrackingScaling"
  resource_id        = each.key
  scalable_dimension = "dynamodb:index:ReadCapacityUnits"
  service_namespace  = "dynamodb"

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBReadCapacityUtilization"
    }

    target_value = 70
  }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_index_write_target" {
  for_each = local.indexNames
  max_capacity       = local.maxCapacity
  min_capacity       = local.minCapacity
  resource_id        = each.key
  scalable_dimension = "dynamodb:index:WriteCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_index_write_policy" {
  for_each = local.indexNames
  name               = "DynamoDBWriteCapacityUtilization:${each.key}"
  policy_type        = "TargetTrackingScaling"
  resource_id        = each.key
  scalable_dimension = "dynamodb:index:WriteCapacityUnits"
  service_namespace  = "dynamodb"

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBWriteCapacityUtilization"
    }

    target_value = 70
  }
}
