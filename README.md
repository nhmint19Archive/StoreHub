# SWE30003 Assignment 3

## Project set up

The project uses .NET - a framework developed by Microsoft to develop applications in C#.

1. Install [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) to your local machine.
2. Install [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/)
    - Select the .NET desktop component. Make sure you include `SQL Server Express 2019 LocalDB` since this is required by the project.
3. Run `dotnet restore` to restore dependencies of the project.
4. Run `dotnet run "/path/to/Assignment3.Application.csproj"` to start the application.

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
