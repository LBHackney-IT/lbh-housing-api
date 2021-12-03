# Hackney: Housing register API

An API to submit and review applications to the housing register. Using the Base API as boilerplate code.

## What does it do?

[**🚀 Swagger**](https://app.swaggerhub.com/apis-docs/Hackney/housing-register-api/1.0.0) ([Edit it here](https://app.swaggerhub.com/apis/Hackney/housing-register-api/1.0.0))

## Stack

- .NET Core as a web framework.
- nUnit as a test framework.
- DynamoDB for a data store.

## Dependencies

- [Gov.uk Notify](https://gov.uk/notify)
- [Evidence API](https://github.com/LBHackney-IT/evidence-api)
- [Activity History API](https://github.com/LBHackney-IT/activity-history-api)

## Contributing

### Setup

1. Install [Docker][docker-download].
2. Install [AWS CLI][AWS-CLI].
3. Clone this repository.
4. Open it in your IDE.

### Development

To serve the application, run it using your IDE of choice, we use Visual Studio CE and JetBrains Rider on Mac.

**Note**
When running locally the appropriate database connection details are still needed.

#### DynamoDb

To use a local instance of DynamoDb, this can be [installed](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html) or run via [Docker](https://www.docker.com/products/docker-desktop).

The following command will start up a new Docker container with DynamoDb.
```
docker-compose up -d dynamodb-database
```

To check that a local DynamoDb instance is up and has valid data, you can use the AWS CLI:
```
aws dynamodb list-tables --endpoint-url http://localhost:8000
```

If you would like to see what is in your local DynamoDb instance using a GUI, then [this admin tool](https://github.com/aaronshaf/dynamodb-admin) or [NoSQL workbench](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.settingup.html) are good tools for this.

#### Docker

The application can also be served locally using docker.

Build and serve the application. It will be available in the port 3000.
```sh
$ make build && make serve
```

#### Notify

The application uses GOV.UK Notify to send emails.

When running the application locally we make calls to the Notify service and so you need to:

1. Ask to be invited to the _Housing Register applications_ service as a team member to access the Notify dashboard
2. Update your local `.env` file with the correct values for the properties `NOTIFY_TEMPLATE_VERIFY_CODE*` and `NOTIFY_API_KEY` (these should use the **development** API key)

### Release process

We use a pull request workflow, where changes are made on a branch and approved by one or more other maintainers before the developer can merge into `master` branch.

![Circle CI Workflow Example](docs/circle_ci_workflow.png)

Then we have an automated six step deployment process, which runs in CircleCI.

1. Automated tests (nUnit) are run to ensure the release is of good quality.
2. The application is deployed to development automatically, where we check our latest changes work well.
3. We manually confirm a staging deployment in the CircleCI workflow once we're happy with our changes in development.
4. The application is deployed to staging.
5. We manually confirm a production deployment in the CircleCI workflow once we're happy with our changes in staging.
6. The application is deployed to production.

Our staging and production environments are hosted by AWS. We would deploy to production per each feature/config merged into  `master`  branch.

### Creating A PR

To help with making changes to code easier to understand when being reviewed, we've added a PR template.
When a new PR is created on a repo that uses this API template, the PR template will automatically fill in the `Open a pull request` description textbox.
The PR author can edit and change the PR description using the template as a guide.

## Static Code Analysis

### Using [FxCop Analysers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.FxCopAnalyzers)

FxCop runs code analysis when the Solution is built.

Both the API and Test projects have been set up to **treat all warnings from the code analysis as errors** and therefore, fail the build.

However, we can select which errors to suppress by setting the severity of the responsible rule to none, e.g `dotnet_analyzer_diagnostic.<Category-or-RuleId>.severity = none`, within the `.editorconfig` file.
Documentation on how to do this can be found [here](https://docs.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2019).

## Testing

### Run the tests

```sh
$ make test
```

To run database tests locally (e.g. via Visual Studio), then a local instance of DynamoDB will need to be running.

The required tables and data will be initialised as part of the test.

### Agreed Testing Approach
- Use nUnit, FluentAssertions and Moq
- Always follow a TDD approach
- Tests should be independent of each other
- Gateway tests should interact with a real test instance of the database
- Test coverage should never go down
- All use cases should be covered by E2E tests
- Optimise when test run speed starts to hinder development
- Unit tests and E2E tests should run in CI
- Test database schemas should match up with production database schema
- Have integration tests which test from the DynamoDB database to API Gateway

## Contacts

### Active Maintainers

- **Selwyn Preston**, Lead Developer at London Borough of Hackney (selwyn.preston@hackney.gov.uk)
- **Mirela Georgieva**, Lead Developer at London Borough of Hackney (mirela.georgieva@hackney.gov.uk)
- **Matt Keyworth**, Lead Developer at London Borough of Hackney (matthew.keyworth@hackney.gov.uk)

### Contributors

- **Thomas Morris**, Senior Developer at Manifesto (thomas.morris@hackney.gov.uk)
- **Tony Gledhill**, Senior Developer at Manifesto (anthony.gledhill@hackney.gov.uk)

### Other Contacts

- **Rashmi Shetty**, Product Owner at London Borough of Hackney (rashmi.shetty@hackney.gov.uk)

[docker-download]: https://www.docker.com/products/docker-desktop
[AWS-CLI]: https://aws.amazon.com/cli/
