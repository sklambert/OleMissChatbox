This is a real-time web-based chat application. There are two types of users. 0 indicating student and 2 indicating professor. At the moment, you will need to hard-code the professor user in the Users.json file. Users are able to register and login (they will be student users by default). Users are able to join existing chatrooms. Users are able to send and receive messages in their respective chatrooms. Professors are able to create chatrooms.

Frameworks:
Microsoft.AspNetCore.App
Microsoft.NETCore.App

Packages:
Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (5.0.3)
Microsoft.Azure.SignalR (1.4.0)
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
