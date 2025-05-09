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
      public class VideoGame <br>
      { <br>
          public int Id { get; set; } <br>
          public string? Title { get; set; } <br>
          public string? Platform { get; set; } <br>
          public string? Developer { get; set; } <br>
          public string? Publisher { get; set; } <br>
      } <br>
  
4. CREATE YOUR CONTROLLERS/DEFINE YOUR ROUTES:
   A controller is a C# class that defines your routes and endpoints for handling incoming HTTP requests (like GET, POST, PUT, DELETE).
    - Each method inside a controller maps to an endpoint and is responsible for handling a specific request type or operation.
    - It's typically best practice to create a Controllers folder so do that first.
    - Refer to Controllers.VideoGameController to see a really good example of what a controller file looks like (there's at least one example of each HTTP method).
    - Define and test your routes with scalar.
    - Routes will be: [HttpGet], [HttpPut], [HttpPost], [HttpDelete], etc...
    - The routes should return HTTP response codes (200, 201, 400, 404 etc...)
    
5. IMPLEMENT YOUR DATABASE CONTEXT AND ENTITY FRAMEWORK:
   Implementing a db allows you to store data persistently.
   The Entity Framework (EF Core) is an Object-Relational Mapper (ORM) that makes it easier to work with your database using C# objects instead of writing raw SQL queries.
    - To start, create a folder to hold your db context. This example uses Data.VideoGameDbContext.cs
    - You'll need to install the MicrosoftEntityFrameworkCore. You can have the IDE do it for you by typing in:
      public class {{ApiName}DbContext}(DbContextOptions<{{ApiName}DbContext}> options) : DbContext(options).
    - Example: public class VideoGameDbContext(DbContextOptions<VideoGameDbContext> options) : DbContext(options)
        - This should trigger red squigglies. ctrl + . brings up a context menu to debug
        - Inside of there should be an option: install package 'Microsoft.Entity.FrameworkCore
        - Once you have installed the latest version, ctrl + . and add the reference (using Microsoft.EntityFrameworkCore)








you can use sql scripts to manage writing everything to a database and what not
to utilize this, you need to do a code first migration (46:00).

taking a break. i just ran Update-Database command in the package manager console ()








* type 'prop' then press tab and it fills in the property syntax for you *
* left ctrl + . brings up a list of options to fix your code *