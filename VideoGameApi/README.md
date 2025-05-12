# ASP.NET Core Web API Development Guide

## 1. CREATE PROJECT

Create new `ASP.NET Core Web API` project:

- Name it whatever you want.
- ✓ Enable OpenAPI support.
- ✓ Use controllers (controllers provide endpoints).

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
- EXAMPLE: `https://localhost:7227/openapi/v1.json`
- NOTE: You can find the localhost port in launchSettings.json

### 2.b ENABLING SCALAR (RECOMMENDED)

1. Install the required package:
   - Browse the package manager for Scalar and download `Scalar.AspNetCore`.

2. Add the required using statement to Program.cs:
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
- EXAMPLE: `https://localhost:7227/scalar/v1`
- NOTE: You can find the localhost port in launchSettings.json

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

```csharp
// VideoGameDbContext.cs

using Microsoft.EntityFrameworkCore; //* Add this using statment here*
using VideoGameApi.Models;

namespace VideoGameApi.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        // Define your database sets
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
    }
}
```

### 5.b CONFIGURE DATABASE CONNECTION

1. Open `appsettings.json` and add your connection string:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLExpress;Database=VideoGameDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
```

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

2. Register the DbContext in `Program.cs` using dependency injection.
 - Add `Microsoft.EntityFrameworkCore` to `Program.cs`
```csharp
using Microsoft.EntityFrameworkCore;
```
- Next add the following line of code to `Program.cs`
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

1. Make sure you've installed `Microsoft.EntityFrameworkCore.Tools` NuGet package

2. Open the Package Manager Console and make sure the default project is set to your project

3. Create the initial migration:
   ```
   Add-Migration Initial
   ```
   Running this command should create and open a migration file like `Migrations.20250508194021_Initial.cs`. This file contains the code that creates the SQL columns based off of C# code.

4. Next, Create the database by applying the migration:
   ```
   Update-Database
   ```

5. Open SQL Server and verify your database was created under the name specified in your connection string

## 7. ADD SEED DATA

Seed data allows you to populate your database with initial data without writing SQL queries.

1. Navigate to your `DbContext` class (e.g., `VideoGameDbContext.cs`) and override the `OnModelCreating()` method:

```csharp
// Data/VideoGameDbContext.cs

using Microsoft.EntityFrameworkCore;
using VideoGameApi.Models;

namespace VideoGameApi.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add seed data
            modelBuilder.Entity<VideoGame>().HasData(
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
            );
        }
    }
}
```

2. Create a new migration for the seed data:
   ```
   Add-Migration Seeding
   ```

3. Apply the migration to insert the seed data:
   ```
   Update-Database
   ```

4. Verify the seed data was added by:
   - Opening SQL Server Management Studio (SSMS)
   - Navigate to VideoGameDb > Tables > dbo.VideoGames
   - Right-click and select "Select Top 1000 Rows" to view your data

## 8. IMPLEMENT CRUD WITH ENTITY FRAMEWORK

In `VideoGameController.cs` the line of code:
`private readonly VideoGameDbContext _context = context;` adds the db context
you use to reference objects from.

Before this change is made you might just add some mock data to use temporarily. Delete the mock data and replace it with the db context. Below is an example
    of before and after. 

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

- In `VideoGameController.cs`, `ActionResult<T>` got wrapped in a `Task<T>` and made methods async with `async/await`.
- Additionally, instead of referring to the videoGames list:
  `List<VideoGame> VideoGames = [...];`, refer to the db context (_content).

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


**Helpful Tips:**

- Type 'prop' then press tab to automatically create property syntax
- Press Ctrl + . to bring up quick fix options for your code
- Use the `async/await` pattern with Entity Framework for better performance
- Remember to add the appropriate using statements at the top of each file
- Always return appropriate HTTP status codes (200, 201, 400, 404, etc.)
