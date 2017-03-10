# DC Snoop

DC Snoop is web application that provides a searchable database of Washington, DC voter registration information. It is built using ASP.Net Core, Entity Framework Core, PostgreSQL, and a little HTML and jQuery for the UI.

![DC Snoop Banner](/img/long-banner.png)

## Setup

This is a standard ASP.Net Core application, so it follows the standard ASP.NET Core setup.

### Prerequisites
* Install the latest version of [.Net Core](https://www.microsoft.com/net/core)
* Install and run [PostgreSQL](https://www.postgresql.org/)

### Updating the configuration
* Update the connection string in appsettings.json to match the settings you used when installing PostgreSQL.

### Build and Run
1. Restoring Nuget depencencies - From the root directory on the command line run
```
dotnet restore
```

2. Setup the database - From the root directory on the command line run
```
dotnet ef database update
```

3. Build and run - From the root directory on the command line run
```
dotnet run
```

The application should now be running on localhost:5000. The RESTful API exposes three methods.
```
http://localhost:5000/api/person/{id}
http://localhost:5000/api/address/{id}
http://localhost:5000/api/search?term={search_terms}
```

4. In the `UI` folder open `index.html` in your favorite browser to view the UI. You may need to update the URL used for AJAX calls within `UI/js/app.js` to connect to your locally running API.

### Additional Notes

These steps will create the database and get the application up and running, but they will not populate the database with any voter registration data. There is a seperate tool for that [here](https://github.com/sethpuckett/dc-snoop-database-writer).