# Database

This project uses DynamoDB for the database layer. Included is a `example_table.json` file to allow for easy setup with some sample data.

## Setup

- Follow the instructions to [run DynamoDB locally](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DynamoDBLocal.DownloadingAndRunning.html)
- Download [NoSQL workbench](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/workbench.settingup.html) to provide a visual GUI for the database
- Add the local DynamoDB connection to NoSQL workbench.
- Import data model using the `example_table.json` file.
- Go to 'Visualizer' and commit to DynamoDB
- Verify that the data is added by using the 'Operation Builder'

**Note**
NoSQL workbench can connect to remotes (AWS instances) as well.