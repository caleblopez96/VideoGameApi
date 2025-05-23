# ASP.NET Core Web API Development Guide
This is a step by step guide on how to create an ASP.NET Core Web Api. 
I learned by watching Patrick God's .Net 9 Web API & Entity Framework
tutorial on youtube: 

Source: https://www.youtube.com/watch?v=AKjG2tjI07U&list=LL&index=1

## 1. CREATE PROJECT

Create new `ASP.NET Core Web API` project:

1. Name your project.
2. Select where to save the project.
3. Choose a solution name:
 - The solution name should be the same as the project name for small, single-project solutions, or a broader, parent-level name if you plan to include multiple projects in the solution.
4. Choose whether or not to save the project and solution in the same directory: 
 - It’s generally recommended to keep them in the same directory for simple projects and separate them for larger, multi-project solutions to keep things organized.
5. Choose your project framework:
 - In this example, I used .NET 9.0.
6. Authentication type is NONE for this example.
7. ✓ Configure for HTTPS.
8. Container OS defaults to Linux (leave it).
9. Container Build Type defaults to Dockerfile (leave it). 
10. ✓ Enable OpenAPI support.
11. ✓ Use controllers (controllers provide endpoints).

This scaffolds out a new ASP.NET core web api project with the:
- `Connected Services`: Shows your database connection
- `Dependencies`: Contains the `Analyzers`, `Frameworks`, and Nuget `Packages` being used by the project
- `Properties`: Contains the `launchSettings.json` file
- `Controllers`: By default, you'll see `WeatherForecastController.cs`. It's included as an example, therefore, you can delete it.
- `appsettings.json`: Contains settings for your app like connection strings and logging configuration. No need to modify for now.
- `Program.cs`: Contains the entry point for your application and the code to configure services, middleware, and route handling for your Web API.
- `WeatherForecast.cs` is the WeatherForecast model. It's included as an example, therefore, you can delete it.
- `{projectname}.http` is a Visual Studio/Visual Studio Code HTTP request file used for testing your API endpoints directly from the editor — without needing something external like Postman or curl.

Now that you understand a little about the project, follow the next steps to create the api.

## 2. SET UP TESTING WITH OPENAPI / SCALAR

When you create an API, you need to create documentation and you need a way to test it as you work on it. Previously, the solution was to use Swagger - a UI that allowed you to work with your API, like Postman. Since it's no longer being supported, you can use the `OpenAPI` spec or `Scalar` (recommended).

### 2.a ENABLING OPENAPI SUPPORT

If you ✓ enabled OpenAPI support during project creation, the following code and comments are auto-generated in Program.cs:

```csharp
// Program.cs

// Configure the HTTP request pipeline.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // This line enables OpenAPI support
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

To test/view/use the `OpenAPI` spec go to:
- `https://{localhost:XXXX}/openapi/v1/json`

_NOTE: You can find the localhost port in `launchSettings.json` under `applicationUrl`_

### 2.b ENABLING SCALAR (RECOMMENDED)

1. Install `Scalar.AspNetCore` `NuGet` package to the project.

2. Add the required using statement to `Program.cs`:
   ```csharp
   using Scalar.AspNetCore;
   ```

3. Add the following code to `Program.cs` in the HTTP configuration request pipeline:
   ```csharp
   app.MapScalarApiReference();
   ```

Here's how your `Program.cs` should look after adding Scalar support:

```csharp
// Program.cs

using Scalar.AspNetCore;  // Add this using statement

// Configure the HTTP request pipeline.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Add this line to enable Scalar API support
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

To test/view/use the `Scalar` spec go to:
- `https://{localhost:XXXX}/scalar/v1`

_NOTE: You can find the localhost port in `launchSettings.json` under `applicationUrl`_

```json
// launchSettings.json

"https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7227;http://localhost:5121",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
```

## 3. CREATE YOUR MODEL(S)

A model is a class that represents a data structure in your app — essentially a blueprint for the kind of data you'll work with.

1. Create a Models folder in your project
2. Add your model classes to this folder

Example model:

```csharp
// VideoGame.cs

namespace VideoGameApi.Models
{
    public class VideoGame
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Platform { get; set; }
        public string? Developer { get; set; }
        public string? Publisher { get; set; }
    }
}
```

## 4. CREATE YOUR CONTROLLERS/DEFINE YOUR ROUTES

A controller is a C# class that defines your routes and endpoints for handling incoming HTTP requests such as `GET`, `PUT`, `POST`, `DELETE` → `[HttpGet]`, `[HttpPut]`, `[HttpPost]`, `[HttpDelete]`, etc.

Each method inside the controller should map to an endpoint and should be responsible for handling a specific request type or operation.

1. Create a Controllers folder in your project
2. Create your controller classes
3. Test your routes using Scalar

Example HTTP method returning HTTP response code:

```csharp
// VideoGameController.cs

[HttpGet] 
public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
{
    return Ok(await _context.VideoGames.ToListAsync()); // the Ok() method returns HTTP status code 200 (Ok) if found.
}
```
Broken down:

- The HTTP attribute
```csharp
[HttpGet]
```

- GetVideoGame() method
```
public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
```
 - access modifier: `public` 
    - The access modifier being used so this method can be used outside of the controller.
 - return type: `Task<ActionResult<List<VideoGame>>>`
    - `Task<>`: indicates that the method is asynchronous 
    - `ActionResult<List<VideoGame>>`:
        - Wraps the HTTP response.
        - Can return HTTP status codes and `List<VideoGame>` payload.
    - `GetVideoGame()`: name of the method.


- Return
```
{
    return Ok(await _context.VideoGames.ToListAsync());
}
```
 - `return Ok(...)`
    - A built-in ASP.NET Core helper method.
    - Returns an HTTP 200 OK response with the retrieved list of video games as the response body.

- `await _context.VideoGames.ToListAsync()` 
    - Accesses the VideoGames table via _context (your Entity Framework Core database context).
    - Asynchronously retrieves all video game records and converts them into a List<VideoGame>.

 


## 5. IMPLEMENT YOUR DATABASE CONTEXT AND ENTITY FRAMEWORK

Implementing a database allows you to store data persistently. For the next steps, make sure to have `SQL Server` installed.

The `Entity Framework` (EF Core) is an Object-Relational Mapper (ORM) that makes it easier to work with your database using C# objects and migrations instead of writing raw SQL queries.

### 5.a CREATE DATABASE CONTEXT

1. Create a folder (e.g., `Data`) to hold your database context
2. Install the required packages into your project:
   - `Microsoft.EntityFrameworkCore` 
   - `Microsoft.EntityFrameworkCore.SqlServer`
   - `Microsoft.EntityFrameworkCore.Tools`

_* you dont need to add these packages to any specific file yet, they just need to be added to the project *_


3. Create your DbContext class:
 - Add the `Microsoft.EntityFrameworkCore` dependency to `VideoGameDbContext.cs`
```csharp    
using Microsoft.EntityFrameworkCore;    
```
- Next, define your database set inside the class
```csharp
public DbSet<VideoGame> VideoGames => Set<VideoGame>();
```

```csharp
// VideoGameDbContext.cs

using Microsoft.EntityFrameworkCore; //* Add this using statment here*
using VideoGameApi.Models;

namespace VideoGameApi.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        // * Add code here *
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
    }
}
```

### 5.b CONFIGURE DATABASE CONNECTION

Open `appsettings.json` and add your connection string:

1. Define a new `ConnectionStrings` object with a connection name like `DefaultConnection`
```json
"ConnectionStrings": {
    "DefaultConnection": " "
  }
```
2. Add the connection string to `DefaultConnection`
```csharp
"Server=localhost\\SQLExpress;Database={DatabaseName};Trusted_Connection=true;TrustServerCertificate=true"
```

Finished Product: 
```json
// appsettings.json

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLExpress;Database=VideoGameDb;Trusted_Connection=true;TrustServerCertificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

3. Now, register the DbContext in `Program.cs` using dependency injection.
 - To do so, add `Microsoft.EntityFrameworkCore` to `Program.cs`
```csharp
using Microsoft.EntityFrameworkCore;
```
- Next add the db context and configure connection with the db using the following line of code in `Program.cs`
```csharp
builder.Services.AddDbContext<VideoGameDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

```csharp
// Program.cs

using Microsoft.EntityFrameworkCore;  //* Add the using statement here *
using Scalar.AspNetCore;
using VideoGameApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register your DbContext
// * Add this line here *
builder.Services.AddDbContext<VideoGameDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 6. IMPLEMENT CODE-FIRST MIGRATIONS

Code-first migration allows you to write C# code and turn it into database structures.

1. Make sure you've installed `Microsoft.EntityFrameworkCore.Tools` `NuGet` package

2. Open the `Package Manager Console` and make sure the default project is set to your project

3. Create the initial migration by typing the following command in the `Package Manager Console`:
   ```
   Add-Migration Initial
   ```
   Running this command should create and open a migration file like `Migrations.20250508194021_Initial.cs`. This file contains the code that creates the SQL columns based off of C# code.

4. Now, despite not having a database yet, apply the migration and create the database by typing the following command in the `Package Manager Console`:
   ```
   Update-Database
   ```

5. Next, open `SSMS` and verify your database was created under the name specified in your `connection string` in the file `appsettings.json`

## 7. ADD SEED DATA

Seed data allows you to avoid writing SQL queries to seed data into the db.

Navigate to `VideoGameDbContext.cs` class and override the `OnModelCreating()` method:
```csharp 
protected override void OnModelCreating(ModelBuilder modelBuilder){ }
```

```csharp
VideoGameDbContext.cs

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    // override the OnModelCreating method
    //* code goes here: *
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    {
}
```

- Next add your model builder by adding `base.OnModelCreating(modelBuilder);` to `VideoGameDbContext.cs`
```csharp
base.OnModelCreating(modelBuilder);
```

```csharp
// VideoGameDbContext.cs

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    // override the OnModelCreating method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //* add code here: *
        base.OnModelCreating(modelBuilder);
    {
}
```

- Next, add the following line of code to `VideoGameDbContext.cs`:
```csharp
modelBuilder.Entity<VideoGame>().HasData();
```

```csharp
VideoGameDbContext.cs

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    // best practice:
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // * code goes here (ideally with more seed data): *
        modelBuilder.Entity<VideoGame>().HasData(
            new VideoGame
            {
                Id = 1,
                Title = "Spider-Man 2",
                Platform = "PS5",
                Developer = "Insomniac Games",
                Publisher = "Sony Interactive Entertainment"
            },...
        );
    }
}
```

- Now run the following command in the `Package Manager Console`:
  `Add-Migration Seeding`
  - This should create another migration file. Somthing like `20250508223857_Seeding.cs`

- Next insert the seed data into the db by runing `Update-Database` in the `Package Manager Console`:
  - To confirm it worked:
    - Open `SSMS`: VideoGameDb > Tables > dbo.VideoGames
    - Right click `execute SQL` to force update. You should see your seed data

## 8. IMPLEMENT CRUD WITH ENTITY FRAMEWORK

In `VideoGameController.cs` the following line of code adds the db context you use to reference objects from:
```csharp
private readonly VideoGameDbContext _context = context;
```

_In the BEFORE example below, I started the project with 3 VideoGames just to test things until I added my db context. The before shows the code with the 3 video games, then replaced for the db context_

BEFORE:
```csharp
// VideoGameController.cs

namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController(VideoGameDbContext context) : ControllerBase
    {
        public static List<VideoGame> VideoGames = [
           new VideoGame
           {
               Id = 1,
               Title = "Spider-Man 2",
               Platform = "PS5",
               Developer = "Insomniac Games",
               Publisher = "Sony Interactive Entertainment"
           },
           new VideoGame
           {
               Id = 2,
               Title = "The Legend of Zelda: Breath of the Wild",
               Platform = "Nintendo Switch",
               Developer = "Nintendo EPD",
               Publisher = "Nintendo"
            },
            new VideoGame
            {
                Id = 3,
                Title = "CyberPunk 2077",
                Platform = "PC",
                Developer = "CD Projekt Red",
                Publisher = "CD Projekt"
            }
         ];
         // get all video games
        [HttpGet]
        public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
        {
            return Ok(await _context.VideoGames.ToListAsync()); // returns 200 (Ok) if found
        }
    }
}
```

AFTER:

```csharp
// VideoGameController.cs

namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController(VideoGameDbContext context) : ControllerBase
    {
        // * This line: *
        private readonly VideoGameDbContext _context = context; // this makes it so that you can reference the database

        [HttpGet]
        public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
        {
            return Ok(await _context.VideoGames.ToListAsync()); // returns 200 (Ok) if found
        }
    }
}
```

Next, in `VideoGameController.cs`, `ActionResult<T>` got wrapped in a `Task<T>` and made methods were made async with `async/await`.

Additionally, the way you refer to your objects changes. 
- Before you would reference objects from
```csharp
List<VideoGame> VideoGames = [...];
```
- Now, reference the objects by the db context
```csharp
_context.VideoGames
```
BEFORE:

```csharp
// VideoGameController.cs

    [HttpGet]
    // before using ActionResult<T<T>>
    public ActionResult<List<VideoGame>> GetVideoGame()
    {
        // before:
        return Ok(VideoGames); // returns 200 (Ok) if found
    }
```

- AFTER:

```csharp
// VideoGameController.cs

    [HttpGet]
    // after using async and Task<T<T<T>>>:
    public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
    {
        // after using await and referring to the db context (_context)
        return Ok(await _context.VideoGames.ToListAsync()); // returns 200 (Ok) if found
    }
```

---
test

**Helpful Tips:**

- Type 'prop' then press tab to automatically create property syntax
- Press Ctrl + . to bring up quick fix options for your code
- Use the `async/await` pattern with Entity Framework for better performance
- Remember to add the appropriate using statements at the top of each file
- Always return appropriate HTTP status codes (200, 201, 400, 404, etc.)
