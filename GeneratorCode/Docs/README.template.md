# {{namespace}}

## Overview
This project was generated using Clean Architecture pattern with the following layers:
- Domain Layer: Contains business entities and interfaces
- Application Layer: Contains business logic and application services
- Infrastructure Layer: Contains implementations of interfaces defined in the domain layer
- Presentation Layer: Contains API controllers and DTOs

## Project Structure
```
{{namespace}}/
├── src/
│   ├── {{namespace}}.Domain/
│   ├── {{namespace}}.Application/
│   ├── {{namespace}}.Infrastructure/
│   └── {{namespace}}.API/
└── tests/
    ├── {{namespace}}.UnitTests/
    └── {{namespace}}.IntegrationTests/
```

## Technologies Used
- ASP.NET Core {{version}}
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- xUnit (for testing)

## Getting Started
1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run database migrations: `dotnet ef database update`
4. Run the application: `dotnet run`

## API Documentation
The API documentation is available through Swagger UI at `/swagger` when running in development mode.

## Available Endpoints
{{#each entities}}
### {{name}}
- GET /api/{{name}} - Get all {{name}}s
- GET /api/{{name}}/{id} - Get {{name}} by ID
- POST /api/{{name}} - Create new {{name}}
- PUT /api/{{name}}/{id} - Update {{name}}
- DELETE /api/{{name}}/{id} - Delete {{name}}
{{/each}}

## Contributing
Please read CONTRIBUTING.md for details on our code of conduct, and the process for submitting pull requests. 