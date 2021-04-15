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

    tags = {
        Name              = "housing-register-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }
}

resource "aws_iam_policy" "housingregisterapi_dynamodb_table_policy" {
    name                  = "lambda-dynamodb-housingregister-api"
    description           = "A policy allowing read/write operations on housingregister dynamoDB for the Housing Register API"
    path                  = "/housing-register-api/"

    policy                = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                        "dynamodb:BatchGetItem",
                        "dynamodb:GetItem",
                        "dynamodb:Query",
                        "dynamodb:Scan",
                        "dynamodb:BatchWriteItem",
                        "dynamodb:PutItem",
                        "dynamodb:UpdateItem"
                     ],
            "Resource": "${aws_dynamodb_table.housingregisterapi_dynamodb_table.arn}"
        }
    ]
}
EOF
}