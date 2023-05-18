# SWE30003 Assignment 3

## Project set up

The project uses .NET - a framework developed by Microsoft to develop applications in C#.

1. Install [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) to your local machine.
2. Install your IDE/text editor. Recommended options:
    - [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/)
    - [Visual Studio Code](https://www.jetbrains.com/rider/)
    - [JetBrains Rider]() (free if you subscribe with your student email)
3. Run `dotnet restore` to restore dependencies of the project.
4. Run `dotnet run "/path/to/Assignment3.Application.csproj"` to start the application.

## Database migration

The project uses Entity Framework Core to handle object-database mappings.
If the database schema is changed, you need to perform a migration.

First, download the EF CLI tool:
```
dotnet tool install --global dotnet-ef
```

Run the following command to perform a migration
```
dotnet ef migrations add <migration name>
dotnet ef database update
```
## Coding conventions

### Naming styles

- Use `PascalCase` for names of: classes, properties, constant fields, enums, methods, events.
- Use `camelCase` for names of: local variables, method parameters, class fields.
    - Append the prefix `_` to names of private class fields e.g., `_foo`, `_bar`, etc.

### Newlines

- Enter newlines before opening braces, `try` - `catch` - `finally` statements
- Enter newlines before `.` if chained method calls are too long
    - Example:
    ```csharp
        // BAD
        var product = context.Products.Where(x => x.Quantity > 20).Select(x => { x.Name, x.Description, x.Quantity }).OrderBy(x => x.Quantity).AsEnumerable();

        // GOOD
        var product = context
            .Products
            .Where(x => x.Quantity > 20)
            .Select(x => { x.Name, x.Description, x.Quantity })
            .OrderBy(x => x.Quantity)
            .AsEnumerable();
    ```

### Miscellaneous

- Prefer `var` over explicit typing (unless required)
- Prefer local methods over anonymous functions
- Prefer `foreach` loops over explicit loop indexing
