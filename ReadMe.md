# DotNetThoughts

A collection of useful .NET libraries and tools, including functional programming patterns, time manipulation utilities, SQL migrations, and more.

## TL;DR - Quick Start

DotNetThoughts provides several independent libraries that you can use individually or together:

- **Results** - A Railway-oriented programming Result type for better error handling
- **TimeKeeping** - Mock and manipulate time for testing and deterministic behavior  
- **LocalTimeKit** - Unambiguous time representation with timezone awareness
- **Sql.Migrations** - Simple SQL Server schema migrations using .sql files
- **Results.Parsing** - Type-safe parsing and validation with Results
- **Results.Json** - JSON serialization support for Result types
- **UserSecrets** - Tools and templates for managing user secrets in development

Each library is available as a separate NuGet package. See the individual project README files for detailed documentation and usage examples.

## Projects

### Azure DevOps

[AzureDevOpsReadMe.md](AzureDevOps/ReadMe.md)

CI/CD pipeline configurations and automation tools for Azure DevOps.

### Results

[Results/DotNetThoughts.Results/ReadMe.md](Results/DotNetThoughts.Results/ReadMe.md)

A functional programming Result type for C# that represents the result of operations that can succeed or fail, inspired by railway-oriented programming.

### Results.Parsing

[Results/DotNetThoughts.Results.Parsing/ReadMe.md](Results/DotNetThoughts.Results.Parsing/ReadMe.md)

Type-safe parsing and validation utilities that work with the Result type to convert and validate input data.

### Results.Json

[Results/DotNetThoughts.Results.Json/ReadMe.md](Results/DotNetThoughts.Results.Json/ReadMe.md)

JSON serialization and deserialization support for Result types, including proper error handling during serialization.

### TimeKeeping

[TimeKeeping/DotNetThoughts.TimeKeeping/ReadMe.md](TimeKeeping/DotNetThoughts.TimeKeeping/ReadMe.md)

Time manipulation utilities for testing, including the ability to freeze, advance, and control time in your applications.

### LocalTimeKit

[LocalTimeKit/DotNetThoughts.LocalTimeKit/ReadMe.md](LocalTimeKit/DotNetThoughts.LocalTimeKit/ReadMe.md)

Unambiguous date and time representation that includes timezone information, eliminating confusion around local vs UTC times.

### Sql.Migrations

[Sql/DotNetThoughts.Sql.Migrations/ReadMe.md](Sql/DotNetThoughts.Sql.Migrations/ReadMe.md)

A simple and straightforward SQL Server database migration system using plain .sql files as embedded resources.

### UserSecrets

[UserSecrets/ReadMe.md](UserSecrets/ReadMe.md)

Tools and configuration templates to help manage user secrets during development, making it easier to keep secrets synchronized across team members.
