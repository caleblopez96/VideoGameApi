gets the weather forecast based on controllers
https://localhost:7227/weatherforecast

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

taking break video is paused at 23:40









* type 'prop' then press tab and it fills in the property syntax for you*