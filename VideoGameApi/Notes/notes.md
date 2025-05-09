# ASP.NET Core Web API Development Guide

## 1. CREATE PROJECT
Create new `ASP.NET core Web API` project
- Name it whatever you want
- ✓ enable OpenAPI support
- ✓ use controllers (controllers provide end points)

## 2. SET UP TESTING WITH SCALAR
When you create an API, you need to also create documentation and you need a way to test it as you work on it.
- Previously, the solution was to use Swagger - a UI that allowed you to work with your API, like Postman
- Since it's no longer being supported, you can use the `OpenAPI` spec or `Scalar` (recommended)
- The `OpenAPI` is configured in the HTTP request pipeline in `Program.cs`
```csharp
Program.cs
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // this line handles OpenApi and is added by default when you ✓ enable OpenAPI support
    app.MapOpenApi();
}
```
- To test/view/use the `OpenAPI` spec go to: `https://{localhost:XXXX}/openapi/v1/json` → `https://localhost:7227/openapi/v1.json`
- To use `Scalar`:
  - Right click add a new NuGet package to your API
  - Browse the package manager for Scalar and download the package `Scalar.AspNetCore`
  - Next, add the following code to your `Program.cs` in the HTTP configuration request pipeline:
```csharp
if (app.Environment.IsDevelopment())
{
    // after the package has been added, add this line to utilize scalar:
    app.MapScalarApiReference();
    // this line handles OpenApi and is added by default when you ✓ enable OpenAPI support
    app.MapOpenApi();
}
```
  - Then open: `https://{localhost:XXXX}/scalar/v1` → `https://localhost:7227/scalar/v1`
  - *NOTE: If you need the localhost, you can find it in launchSettings.json*
```json
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
A model is a simple class that represents a data structure in your app — essentially a blueprint for the kind of data you'll work with.
 - Create a Models folder and start creating Models.
Example `VideoGame.cs`:
```csharp
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
A controller is a C# class that defines your routes and endpoints for handling incoming HTTP requests `GET`, `PUT`, `POST`, `DELETE` → `[HttpGet]`, `[HttpPut]`, `[HttpPost]`, `[HttpDelete]`, etc...
- Each method inside the controller should map to an endpoint and should be responsible for handling a specific request type or operation.
- Create a Controllers folder.
- Begin creating your Controllers (refer to `VideoGameController.cs` to see a really good example of what a controller file looks like).
- Create and test each route using Scalar. Test each route after you create it. Make sure it works before moving on.
- The routes should return HTTP response codes (200, 201, 400, 404 etc...)
```csharp
// EXAMPLE of an HTTP method returning HTTP response code:
[HttpGet]
   public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
   {
       return Ok(await _context.VideoGames.ToListAsync()); // the Ok() method returns HTTP status code 200 (Ok) if found.
   }
```

## 5. IMPLEMENT YOUR DATABASE CONTEXT AND ENTITY FRAMEWORK
Implementing a database allows you to store data persistently. Make sure to have `SQL Server` installed.

The `Entity Framework` (EF Core) is an Object-Relational Mapper (ORM) that makes it easier to work with your database using C# objects instead of writing raw SQL queries.

- To start, create a folder to hold your db context. This example uses `Data.VideoGameDbContext.cs`
- You'll need to install the `MicrosoftEntityFrameworkCore` and add the following using statement:
 ```csharp
// add this line:
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
  - This should trigger red squigglies. `ctrl + .` brings up a context menu to debug
  - Inside of there should be an option: install package `Microsoft.Entity.FrameworkCore`
  - If the option isn't in the little menu, just right click on your Api folder and add NuGet package. search for: `Microsoft.Entity.FrameworkCore`

- Next step is to add a database set:
  - To do so, open the `VideoGameDbContext.cs` file and add the following:
```csharp
using Microsoft.EntityFrameworkCore;

namespace VideoGameApi.Data
{
    // add this class and extend DbContext(options):
    // add this line:
    public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
    {
        // best practice:
        public DbSet<VideoGame> VideoGames => Set<VideoGame>();
    }
}
```

- Next you need to tell the application where to find the database:
  - To do so, open `appsettings.json` and add your connection string:
```json
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
    *Note this way is preferred because you can use the same name in Azure*

- Register the DbContext now in `Program.cs` using dependency injection:
  - Before you do, download the `Microsoft.EntityFrameworkCore.SqlServer` NuGet package
  - Make sure to include `using Microsoft.EntityFrameworkCore`
  - Then add this line: `builder.Services.AddDbContext<VideoGameDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));`

```csharp
// These are the dependencies we just added + the Scalar dependency from earlier
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VideoGameApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add this line:
builder.Services.AddDbContext<VideoGameDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // this line handles scalar
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 6. IMPLEMENT CODE-FIRST MIGRATIONS
Code-first migration allows you to write C# code and turn it into database related stuff.

- First step is to download the NuGet package `Microsoft.Entity.FrameworkCore.Tools`
- Then open the package manager console and make sure the default project is set to the correct project
- Run the following command in the package manager console: `Add-Migration Initial`
  - Running this command should create and open a migration file. Something like: `Migrations.20250508194021_Initial.cs`
  - This file contains the code that creates the SQL columns based off the C# code
- Now, despite not having a db yet, run the following command in the package manager console: `Update-Database`
  - This command creates the database
- Now open up SQL Server and make sure you're connected properly to `LAPTOP/SQLEXPRESS`
  - Under databases you should see your newly created database, but the data should be empty
- At this point, you can write manual SQL queries to add data or you can seed data. Seed data is probably best.

## 7. ADD SEED DATA
Seed data allows you to avoid writing SQL queries to seed data into the db.

- To do so, navigate to `VideoGameDbContext.cs` class and `override` the `OnModelCreating()` method: `protected override void OnModelCreating(ModelBuilder modelBuilder){ }`
```csharp
public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    // best practice:
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    // override the OnModelCreating method
    // add this line:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    {
}
```
  - Next add: `base.OnModelCreating(modelBuilder);`
```csharp
public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    // best practice:
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    // override the OnModelCreating method
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // add this:
        base.OnModelCreating(modelBuilder);
    {
}
```
  - Next add: `modelBuilder.Entity<VideoGame>().HasData();`
```csharp
public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
{
    // best practice:
    public DbSet<VideoGame> VideoGames => Set<VideoGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // add this just with more seed data
        // add this:
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

- Next run the following command in the package manager console: `Add-Migration Seeding`
  - This should create another migration file. Somthing like: (20250508223857_Seeding.cs)
- Next run the following command in the package manager console: `Update-Database`
  - This inserts the seed data to database
- To confirm it worked:
  - Open SSMS: VideoGameDb > Tables > dbo.VideoGames
  - Right click execute SQL to force update. You should see your seed data

## 8. IMPLEMENT CRUD WITH ENTITY FRAMEWORK
- The way this tutorial plays out, you set up the project without features, then you add CRUD with entity framework after.

In `VideoGameController.cs` the line of code: `private readonly VideoGameDbContext _context = context;` adds the db context you use to reference objects from.

- Prior to this there were three VideoGame objects being used. 
 - Delete the mock data and replace it with the db context. Below is an example of before and after.
BEFORE:
```csharp
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
    }
}
```

AFTER: 
```csharp
namespace VideoGameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGameController(VideoGameDbContext context) : ControllerBase
    {
        // This line:
        private readonly VideoGameDbContext _context = context; // this makes it so that you can reference the database 

        // get all video games
        [HttpGet]
        public async Task<ActionResult<List<VideoGame>>> GetVideoGame()
        {
            return Ok(await _context.VideoGames.ToListAsync()); // returns 200 (Ok) if found
        }
    }
}
```

- In `VideoGameController.cs`, `ActionResult<T>` got wrapped in a `Task<T>` and made methods async with `async/await`.
- Additionally, instead of referring to the videoGames list: `List<VideoGame> VideoGames = [...];`, refer to the db context (_content)
- BEFORE: 
```csharp
// get all video games
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
// get all video games
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
* Type 'prop' then press tab and it fills in the property syntax for you
* Left ctrl + . brings up a list of options to fix your code