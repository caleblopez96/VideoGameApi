1. CREATE PROJECT:   create new ASP.NET core Web API project
    - name it whatever you want
    - CHECK enable OpenAPI support
    - CHECK use controllers (controllers provide end points)                      
2. SET UP SCALAR:
   When you create an api, you need to also create documentation and you need a way to test is as you work on it.
    - Previously, the solution was to use swagger. it was just a ui that allowed you to work with your api, like postman.
    - Since its no longer being supported, you can use the openapi spec or Scalar (recommended).
    - Open API spec comes default in the project. Its configured in the http request pipeline in Program.cs (app.MapOpenApi()).
    - To use the OpenAPI spec go to: https://{localhost:XXXX}/openapi/v1/json -> https://localhost:7227/openapi/v1.json
    - To use Scalar.
        - Right click add a new NuGet package to you Api.
        - Browse the package manager for scalar and download the package Scalar.AspNetCore.
        - Next, add the following code to your Program.cs in the http configuration request pipeline:
          app.MapScalarApiReference();
        - Then open: https://{localhost:XXXX}/openapi/v1/json -> https://localhost:7227/scalar/v1
        - *NOTE: If you need the localhost, go look in launchSettings.json*
        
3. CREATE YOUR MODEL(S):
   A model is a simple class that represents a data structure in your app — essentially a blueprint for the kind of data you’ll work with.
    - Example model:
```C#
public class VideoGame
    {
       public int Id { get; set; }
       public string? Title { get; set; }
       public string? Platform { get; set; }
       public string? Developer { get; set; }
       public string? Publisher { get; set; }
    }
```

4. CREATE YOUR CONTROLLERS/DEFINE YOUR ROUTES:
   A controller is a C# class that defines your routes and endpoints for handling incoming HTTP requests (like GET, POST, PUT, DELETE).
    - Each method inside a controller maps to an endpoint and is responsible for handling a specific request type or operation.
    - It's typically best practice to create a Controllers folder so do that first.
    - Refer to Controllers.VideoGameController to see a really good example of what a controller file looks like (there's at least one example of each HTTP method).
    - Define and test your routes with scalar.
    - Routes will be: [HttpGet], [HttpPut], [HttpPost], [HttpDelete], etc...
    - The routes should return HTTP response codes (200, 201, 400, 404 etc...)    
5. IMPLEMENT YOUR DATABASE CONTEXT AND ENTITY FRAMEWORK:
   Implementing a db allows you to store data persistently. Make sure to have SQL Server installed.
   The Entity Framework (EF Core) is an Object-Relational Mapper (ORM) that makes it easier to work with your database using C# objects instead of writing raw SQL queries.
    - To start, create a folder to hold your db context. This example uses Data.VideoGameDbContext.cs.
    - You'll need to install the MicrosoftEntityFrameworkCore. You can have the IDE do it for you by typing in:
      public class {{ApiName}DbContext}(DbContextOptions<{{ApiName}DbContext}> options) : DbContext(options).
    - Example: ```C# public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options);```
        - This should trigger red squigglies. ctrl + . brings up a context menu to debug.
        - Inside of there should be an option: install package 'Microsoft.Entity.FrameworkCore.
        - Once you have installed the latest version, ctrl + . and add the reference (using Microsoft.EntityFrameworkCore).
    - Next step is to add a database set:
        - To do so, open the dbContext.cs file and add the following:
        ```C# public DbSet<VideoGame> VideoGames => Set<VideoGame>();```
    - Next you need to tell the application where to find the database:
        - To do so, open appsettings.json and add your connection string:
          ConnectionStrings": {"DefaultConnection": "Server=localhost\\SQLExpress;Database=VideoGameDb;Trusted_Connection=true;TrustServerCertificate=true"}
          *Note this way is preferred because you can use the same name in Azure*
    - Register the DbContext now in Program.cs using dependency injection:
        - before you do, download the Microsoft.EntityFrameworkCore.SqlServer NuGet package:
        - then add: ```C# builder.Services.AddDbContext<VideoGameDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));```
        - add using Microsoft.EntityFrameworkCore (ctrl + .)

6. IMPLEMENT CODE-FIRST MIGRATIONS:
   Code-first migration allows you to write C# code and turn it into database related stuff.
   - First step is to download the NuGet package Microsoft.Entity.FrameworkCore.Tools
   - Then Open the package manager console and make sure the default project is set to the correct project.
   - Run the following command in the package manager console: Add-Migration Initial
      - running this command should create and open a migration file. (Migrations.20250508194021_Initial.cs)
      - this file contains the code that creates the sql columns based off the C# code.
   - Now, despite not having a db yet, run the following command in the package manager console: Update-Database
      - this command creates the database 
   - Now open up sql server and make sure youre connected properly to LAPTOP/SQLEXPRESS, 
      - under databases you should see your newly created database, but the data should be empty
   - At this point, you can write manual sql queries to add data or you can seed data. Seed data is prob best.

7. ADD SEED DATA:
   Seed data allows you to avoid writing sql queries to seed data into the db.
   - To do so, navigate to VideoGameContext.cs class and override the OnModelCreating() method:
      - ```C# protected override void OnModelCreating(ModelBuilder modelBuilder){} 
        ```
      - next add: ```C# base.OnModelCreating(modelBuilder); ```
      - next add: ```C# modelBuilder.Entity<VideoGame>().HasData(data); ``` add your data in place of the data argument (see VideoGameDbContext.cs for an example)
   - Next run the following command in the package manager console: Add-Migration Seeding
      - this should create another migration file (20250508223857_Seeing.cs)
   - Next run the following command in the package manager console: Update-Database
      - This inserts the seed data to database. 
   - Go confirm it worked:
      - open SSMS: VideoGameDb > Tables > dbo.VideoGames. 
      - right click execute SQL to force update. You should see your seed data.
    
8. IMPLEMENT CRUD WITH ENTITY FRAMEWORK:
   - The way this tutorial plays out, you set up the project without, then you add CRUD with entity framework after.
   - As I was writing the notes, I already completed the project, so you don't see "without entity framework" code.
   In VideoGameController.cs the line of code: private readonly VideoGameDbContext _context = context; adds the db context you use to reference objects from.
    - prior to this there were three VideoGame objects being used. Delete the mock data and replace it with the db context.
    BEFORE: 
    ```c#
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
    ```
    AFTER: ```C# private readonly VideoGameDbContext _context = context;```
   In your VideoGameController.cs, ActionResult's got wrapped in a Task and add the async keyword.
      - BEFORE: ```C# public <ActionResult<VideoGame> GetVideoGameById(int id)```
      - AFTER: ```C# public async Task<ActionResult<VideoGame>> GetVideoGameById(int id)```



* type 'prop' then press tab and it fills in the property syntax for you *
* left ctrl + . brings up a list of options to fix your code *