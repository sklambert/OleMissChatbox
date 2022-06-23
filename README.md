Frameworks:
Microsoft.AspNetCore.App
Microsoft.NETCore.App

Packages:
Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (5.0.3)
Microsoft.EntityFrameworkCore.Design (5.0.3)
Microsoft.EntityFrameworkCore.SqlServer (5.0.3)
Microsoft.EntityFrameworkCore.Tools (5.0.1)

The Users.json file has data which you can alter to your liking as long as its the same format
Open a command line prompt, navigate to the project file, run these commands:
>dotnet tool install dotnet-ef -g
>dotnet ef database update
>dotnet ef migrations add InitialDb
>dotnet ef database update
>dotnet run /seed
