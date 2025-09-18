# Jira.Database.Querier
A collection of database query methods for [Atlassian JIRA](http://www.atlassian.com/software/jira), recommended for use with readonly database.

## Support Notice
Developed based on the description of the [JIRA v8.20 Database Schema](https://developer.atlassian.com/server/jira/platform/database-schema/)
Support may not extend to all subsequent versions.

## Project Overview
Project | Description
--- | ---
[Jira.Database.EntityFrameworkCore](./Jira.Database.EntityFrameworkCore) | EFCore DB Context for JIRA
[Jira.Database.EntityFrameworkCore.MySQL](./Jira.Database.EntityFrameworkCore.MySQL) | Subclass of JIRA DB Context, facilitating connections to MySQL JIRA database
[Jira.Database.Querier](./Jira.Database.Querier) | Collection of methods for querying the JIRA database
[Jira.Database.Querier.RestApi](./Jira.Database.Querier.RestApi) | Extension methods to connect with the JIRA REST API for comprehensive coverage, as described in [Querier with REST API](./docs/querier-with-rest-api.md)
[Jira.Database.FieldOption.CodeGen](./Jira.Database.FieldOption.CodeGen) | Generates options for built-in fields like IssueType and Status, as well as Select Custom Fields, as detailed in [How to Automatically Generate Issue Field Options](./docs/how-to-auto-generate-field-option.md)
[Jira.Database.OOO](./Jira.Database.OOO) | Example project using [Jira.Database.FieldOption.CodeGen](./Jira.Database.FieldOption.CodeGen) for field definition
[Jira.Database.Querier.Faker](./Jira.Database.Querier.Faker) | Collection of methods for creating fake data for the JIRA database
[Jira.Database.Querier.Test](./Jira.Database.Querier.Test) | Creates fake data in-memory and reads it to verify functionality correctness

## License
This project is licensed under [MIT](/LICENSE.md).

## Dependencies & Requirements
- [Microsoft.EntityFrameworkCore.Relational](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Relational)
- [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging)
- Tested with JIRA v8.20

## Documentation
The documentation is placed under the [docs](./docs) folder.
### Basic Usage
  * [How to query Issue](./docs/how-to-query-issue.md)
  * [How to query Project](./docs/how-to-query-project.md)
  * [How to query User](./docs/how-to-query-user.md)
  * [How to define Custom Field](./docs/how-to-define-custom-field.md)
### Generating Field Option Libraries
  * [How to Automatically Generate Issue Field Options](./docs/how-to-auto-generate-field-option.md)
### Connecting to External Data
  * [Querier with REST API](./docs/querier-with-rest-api.md)
### Additional Customizations
  * [Injecting Custom Services or Default Settings](./docs/querier-builder-configuration.md)
  