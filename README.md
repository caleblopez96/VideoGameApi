# ASP.NET Core Web API Development Guide

## 1. CREATE PROJECT

Create new `ASP.NET core Web API` project:

- Name it whatever you want.
- ✓ enable OpenAPI support.
- ✓ use controllers (controllers provide end points).

## 2. SET UP TESTING WITH OPENAPI / SCALAR

When you create an API, you need to also create documentation and you need a way
to test it as you work on it. Previously, the solution was to use Swagger - a UI
that allowed you to work with your API, like Postman. Since it's no longer being
supported, you can use the `OpenAPI` spec or `Scalar` (recommended).

### 2.a ENABLING OPENAPI SUPPORT
- If you ✓ enable OpenAPI support, the following code below is auto generated in Program.cs and you don't need to make any changes:
```csharp
app.MapOpenApi();
```
- This gets configured in the HTTP request pipeline:
```csharp
// Program.cs

// AUTO GENERATED:
// Configure the HTTP request pipeline.
var builder = WebApplication.CreateBuilder(args);

// AUTO GENERATED:
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// AUTO GENERATED:
var app = builder.Build();

// AUTO GENERATED:
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // AUTO GENERATED:
    //* this line enables OpenApi support and is auto generated when you ✓ enable OpenAPI support*
    app.MapOpenApi();
}

// AUTO GENERATED:
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

- To test/view/use the `OpenAPI` spec go to:
  - `https://{localhost:XXXX}/openapi/v1/json`
  - EXAMPLE: `https://localhost:7227/openapi/v1.json`
- _NOTE: If you need the localhost, you can find it in launchSettings.json_

### 2.b ENABLING SCALAR (RECOMMENDED):
  - Browse the package manager for Scalar and download `Scalar.AspNetCore`.
  - Next, add the following code to `Program.cs` in the HTTP configuration
    request pipeline: 
```csharp
app.MapScalarApiReference();
```

```csharp
// Program.cs

// AUTO GENERATED:
// Configure the HTTP request pipeline.
var builder = WebApplication.CreateBuilder(args);

// AUTO GENERATED:
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// AUTO GENERATED:
var app = builder.Build();

// AUTO GENERATED:
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // AUTO GENERATED:
    app.MapOpenApi();
    //* Add this line to enable Scalar API support*
    app.MapScalarApiReference();
}

// AUTO GENERATED:
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

- After you've added the code to enable scalar, you can now view your api with a GUI.
- To test/view/use the `Scalar` spec go to:
  - `https://{localhost:XXXX}/scalar/v1`
  - EXAMPLE: `https://{localhost:7227}/scalar/v1`
- _NOTE: If you need the localhost, you can find it in launchSettings.json_

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

A model is a class that represents a data structure in your app —
essentially a blueprint for the kind of data you'll work with.

- Create a Models folder and start creating Models.

Example:

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

A controller is a C# class that defines your routes and endpoints for handling
incoming HTTP requests `GET`, `PUT`, `POST`, `DELETE` → `[HttpGet]`,
`[HttpPut]`, `[HttpPost]`, `[HttpDelete]`, etc...

Each method inside the controller should map to an endpoint and should be responsible for handling a specific request type or operation.

Create a folder to hold your Controllers:
- Create your Controllers (refer to `VideoGameController.cs` to see an example).
- Create and test your routes using Scalar. 
  - _The routes should return HTTP response codes (200, 201, 400, 404 etc...) using controller base methods_

Example of an HTTP method returning HTTP response code:
```csharp
// VideoGameController.cs

[HttpGet]
   public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
   {
       return Ok(await _context.VideoGames.ToListAsync()); // the Ok() method returns HTTP status code 200 (Ok) if found.
   }
```

## 5. IMPLEMENT YOUR DATABASE CONTEXT AND ENTITY FRAMEWORK

Implementing a database allows you to store data persistently. For the next steps make sure to have
`SQL Server` installed.

The `Entity Framework` (EF Core) is an Object-Relational Mapper (ORM) that makes
it easier to work with your database using C# objects and migrations instead of writing raw SQL
queries.

To start, create a folder to hold your db context. This example uses `VideoGameDbContext` in the `Data` folder.

Next Install the `MicrosoftEntityFrameworkCore`.
  - Don't forget to Add the proper using statment to your DbContext file:

```csharp
VideoGameDbContext.cs

//* add this line: *
using Microsoft.EntityFrameworkCore;

namespace VideoGameApi.Data
{
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        // best practice:
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
    }
}
```

Now add a database set:
 - To do so, open the your DbContext file `VideoGameDbContext.cs` and add the following:

```csharp
VideoGameDbContext.cs

using Microsoft.EntityFrameworkCore;

namespace VideoGameApi.Data
{
    //* add this class and extend DbContext(options): *
    //* add this line: *
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
    }
}
```

Next you need to tell the application where to find the database:
  - To do so, open `appsettings.json` and add your connection string:

```json
// appSettings.json

{
  // add this:
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

_This is the preferred way because you can use the same name in Azure_

Now register the DbContext in `Program.cs` using dependency injection:
- Before you do, make sure to download the `Microsoft.EntityFrameworkCore.SqlServer` NuGet package
    - include `using Microsoft.EntityFrameworkCore`
- Then add this line:
    `builder.Services.AddDbContext<VideoGameDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));`

```csharp
// Program.cs

// These are the dependencies we just added + the Scalar dependency from earlier
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VideoGameApi.Data;

// AUTO GENERATED:
var builder = WebApplication.CreateBuilder(args);

// AUTO GENERATED:
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//* Add this line: *
builder.Services.AddDbContext<VideoGameDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AUTO GENERATED:
var app = builder.Build();

// AUTO GENERATED:
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

// AUTO GENERATED:
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 6. IMPLEMENT CODE-FIRST MIGRATIONS

Code-first migration allows you to write C# code and turn it into database
related stuff.

To get started download `Microsoft.Entity.FrameworkCore.Tools` NuGet package.

Next open the package manager console and make sure the default project is set to the correct project
- Run the following command in the package manager console: `Add-Migration Initial`
  - Running this command should create and open a migration file. Something like: `Migrations.20250508194021_Initial.cs`. The file contains the code that creates the SQL columns based off the C# code
- Now, despite not having a db yet, run `Update-Database` in the package manager console to create the database.

Now open up SQL Server and make sure you're connected properly (`LAPTOP/SQLEXPRESS`)
  - Under databases you should see your newly created database, but the data should be empty.

At this point, you can write manual SQL queries but we're going to use seed migration instead.

## 7. ADD SEED DATA

Seed data allows you to avoid writing SQL queries to seed data into the db.

Navigate to `VideoGameDbContext.cs` class and override the `OnModelCreating()` method:
```csharp protected override void OnModelCreating(ModelBuilder modelBuilder){ }```

```csharp
VideoGameDbContext.cs

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    // override the OnModelCreating method
    //* add this line: *
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    {
}
```

- Next add your model builder: `base.OnModelCreating(modelBuilder);`

```csharp
VideoGameDbContext.cs

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    // override the OnModelCreating method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //* add this: *
        base.OnModelCreating(modelBuilder);
    {
}
```

- Next add: `modelBuilder.Entity<VideoGame>().HasData();`

```csharp
VideoGameDbContext.cs

public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    // best practice:
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // add this (ideally with more seed data):
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

- Next run the following command in the package manager console:
  `Add-Migration Seeding`
  - This should create another migration file. Somthing like:
    (20250508223857_Seeding.cs)

- Next insert the seed data into the db by run the following command in the package manager console: `Update-Database`
  - To confirm it worked:
    - Open SSMS: VideoGameDb > Tables > dbo.VideoGames
    - Right click execute SQL to force update. You should see your seed data

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

- Type 'prop' then press tab and it fills in the property syntax for you
- Left ctrl + . brings up a list of options to fix your code
