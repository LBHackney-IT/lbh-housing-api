resource "aws_dynamodb_table" "housingregisterapi_dynamodb_table" {
    name                  = "HousingRegister"
    billing_mode          = "PROVISIONED"
    read_capacity         = 10
    write_capacity        = 10
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


    tags = {
        Name              = "housing-register-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }

    global_secondary_index {
        name              = "HousingRegisterAll"
        read_capacity     = 10
        write_capacity    = 10
        hash_key          = "activeRecords"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterStatus"
        read_capacity     = 10
        write_capacity    = 10
        hash_key          = "status"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }

    global_secondary_index {
        name              = "HousingRegisterAssignedTo"
        read_capacity     = 10
        write_capacity    = 10
        hash_key          = "assignedTo"
        range_key         = "sortKey"
        projection_type   = "ALL"
    }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_table_read_target" {
  max_capacity       = 50
  min_capacity       = 5
  resource_id        = "table/HousingRegister"
  scalable_dimension = "dynamodb:table:ReadCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_table_read_policy" {
  name               = "DynamoDBReadCapacityUtilization:${aws_appautoscaling_target.housingregisterapi_dynamodb_table_read_target.resource_id}"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.housingregisterapi_dynamodb_table_read_target.resource_id
  scalable_dimension = aws_appautoscaling_target.housingregisterapi_dynamodb_table_read_target.scalable_dimension
  service_namespace  = aws_appautoscaling_target.housingregisterapi_dynamodb_table_read_target.service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBReadCapacityUtilization"
    }

    target_value = 70
  }
}

resource "aws_appautoscaling_target" "housingregisterapi_dynamodb_table_write_target" {
  max_capacity       = 50
  min_capacity       = 5
  resource_id        = "table/HousingRegister"
  scalable_dimension = "dynamodb:table:WriteCapacityUnits"
  service_namespace  = "dynamodb"
}

resource "aws_appautoscaling_policy" "housingregisterapi_dynamodb_table_write_policy" {
  name               = "DynamoDBWriteCapacityUtilization:${aws_appautoscaling_target.housingregisterapi_dynamodb_table_write_target.resource_id}"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.housingregisterapi_dynamodb_table_write_target.resource_id
  scalable_dimension = aws_appautoscaling_target.housingregisterapi_dynamodb_table_write_target.scalable_dimension
  service_namespace  = aws_appautoscaling_target.housingregisterapi_dynamodb_table_write_target.service_namespace

  target_tracking_scaling_policy_configuration {
    predefined_metric_specification {
      predefined_metric_type = "DynamoDBWriteCapacityUtilization"
    }

    target_value = 70
  }
}
