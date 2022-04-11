# NAT API Solution

## Installation
### .Net Core 6
* [ASP.NET Core Runtime 6.0.3](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

### PostgreSQL
* [Latest PostgreSQL](https://www.postgresql.org/download/)

## Run
### Add Following into environment variables
* ASPNETCORE_ENVIRONMENT - Development
* ConnectionStrings:Default - Server=127.0.0.1;Port=5432;Database=postgres;User Id=[user id];Password=[pwd];
* Serilog:WriteTo:1:Args:connectionString - Server=127.0.0.1;Port=5432;Database=postgres;User Id=[user id];Password=[pwd];
* UIOriginURL - https://localhost:5000

### Run project
* From command line run - dotnet run --project [path before nat-api folder]\nat-api\nat-api.api\nat-api.api.csproj



## EF DB Migrations
### Install CLI tools
* dotnet tool install --global dotnet-ef

### Create database migration command ###
* dotnet ef --startup-project nat-api.api/nat-api.api.csproj migrations add *migration_name_here* -p nat-api.data/nat-api.data.csproj

### Remove latest database migration command
* dotnet ef --startup-project nat-api.api/nat-api.api.csproj migrations remove -p nat-api.data/nat-api.data.csproj

### Update database command ###
* dotnet ef --startup-project nat-api.api/nat-api.api.csproj database update
  * Initial run will create the database
* dotnet ef --startup-project nat-api.api/nat-api.api.csproj database update '*migration name to revert to*'
---
