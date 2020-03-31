# Seven days Challenge

Best RESTful API to manage a small movie rental.

![Diagram](/Docs/Diagram.PNG?raw=true "API Diagram")

## Prerequisites 🔧

* .NET Core 3.1
* Visual Studio 2019
* SQL Server Management
* Postman
* Docker Desktop

## Installation 🔨

### 1. Create Database

You can find the script to create the database and load test data. Path: **Database\7Days_DataBase.sql**.
Also you can use my database in azure: *sevendays.database.windows.net,1433* just send me your IP and I will give you access.


### 2. Deploy Api

There two options that I recommend: run the project locally (localhost using docker or IIS express) or deploy in Azure. You can find the project in the path **SevenDays\SevenDaysApi**.
If you decide to use another database, don't forget to change the connection string in *appsettings.json* or *appsettings.Development.json*

```javascript
"ConnectionStrings": {
    "SevenDaysConnectionString": "Server=....."
 }
```

## Usage ⚙️

You can import the **Postman** collections included in the **Docs** folder and test all the operations availables. There are two files to test local api or my api exposed in Azure.
* **SevenDaysChallenge.postman_collection_AZUREAPP.json**
* **SevenDaysChallenge.postman_collection_LOCAL.json**

You can visualize all the API documentation in the follow URL:
* [http://sevendayschallengeapi.azurewebsites.net/swagger/index.html](http://sevendayschallengeapi.azurewebsites.net/swagger/index.html)

Also you can check a simple web app that consume the API to get the movies in the follow URL:
* [https://sevendayschallengeweb.azurewebsites.net](https://sevendayschallengeweb.azurewebsites.net)

## Built With

* [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) - The Api and Web framework used
* [Azure SQL Database](https://azure.microsoft.com/es-es/services/sql-database/) - Cloud storage
* [Swagger UI](https://swagger.io/tools/swagger-ui/) - Used to the visual documentation
* [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - Used to create a very simple web UI

## Author ✒️

* **Victor Maravilla** - [Likedin](https://www.linkedin.com/in/vamaravilla/)

## License
[MIT](https://choosealicense.com/licenses/mit/)