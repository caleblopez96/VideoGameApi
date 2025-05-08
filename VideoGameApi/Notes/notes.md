1. create new .net framework (go back and see exact one)
2. 






Scala is what you use to debug/test your api. its absolutely essential when developing/working with an API.
After you start the program, copy the link below in the browser and use the browser to work with your API.
https://localhost:7227/scalar/v1

when you create an api, typically you want to create documentation and a way to test is as you work on it.
previously, the solution was to use swagger. it was just a ui that allowed you to work with your api, like postman.
since its no longer being supported, you can use the openapi spec:
https://localhost:7227/openapi/v1.json

OR

you can use scalar but you have to download the package to your project using nuget(nuget is the .net version of npm).
once youve got it downloaded to your project, you need to add a line of code to your program.cs:
change this:
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

to this:
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(); (ctrl + . to add the proper using statement)
    app.MapOpenApi();
}
then use the following:
https://localhost:7227/scalar/v1


you can use sql scripts to manage writing everything to a database and what not
to utilize this, you need to do a code first migration (46:00).

taking a break. i just ran Update-Database command in the package manager console ()








* type 'prop' then press tab and it fills in the property syntax for you*