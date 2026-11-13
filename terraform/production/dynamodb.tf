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
        Environment       = "prod"
        Confidentiality   = "Internal"
        terraform-managed = true
        project_name      = var.project_name
        Application       = "Housing Register"
        TeamEmail         = "developementteam@hackney.gov.uk"
        BackupPolicy      = "Prod"
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

resource "aws_sns_topic" "housingregister_dynamodb_alerts" {
  name = "housingregister-dynamodb-alerts"
}

resource "aws_cloudwatch_metric_alarm" "housingregister_dynamodb_read_spike" {
  alarm_name          = "housing-register-dynamodb-read-spike-${var.environment_name}"
  alarm_description   = "DynamoDB ConsumedReadCapacityUnits is unusually high — possible runaway scan or bulk operation."
  namespace           = "AWS/DynamoDB"
  metric_name         = "ConsumedReadCapacityUnits"
  dimensions = {
    TableName = aws_dynamodb_table.housingregisterapi_dynamodb_table.name
  }
  statistic           = "Sum"
  period              = 300
  evaluation_periods  = 1
  threshold           = 100000 # 100,000 RCUs / 5 min ≈ 333 RCU/s sustained; above report generation peak
  comparison_operator = "GreaterThanThreshold"
  treat_missing_data  = "notBreaching"
  alarm_actions       = [aws_sns_topic.housingregister_dynamodb_alerts.arn]
  ok_actions          = [aws_sns_topic.housingregister_dynamodb_alerts.arn]
}
