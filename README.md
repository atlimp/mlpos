# mlpos


&copy; 2025 by Atli Marcher Pálsson

A simple POS designed for self service.

**Link to demo environment:** https://mlpos.marcher.is/

## Getting started:
To build and run mlpos locally you will need the following:

  * [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
  * [node.js](https://nodejs.org/en)
  * Access to a Postgresql server.

## Installation
### Clone and build
Start by cloning the repository
```
git clone https://github.com/atlimp/mlpos.git
```

Build the POS front end
```
cd mlpos/MLPos.Web/PosClient
npm install
npm run build
```
Build the web application
```
cd mlpos/MLPos.Web
dotnet build
```

### Use prebuilt release
Alternatively you can use a prebuilt self contained web app from the latest [release](https://github.com/atlimp/mlpos/releases/latest).  You can simply download either the Windows x64 version or Linux x64 version and run the MLPos.Web binary.

### Initializing the database
mlpos uses Postgresql as a dbms under `mlpos/MLPos.Data/Postgres/scripts` you will find sql scripts for initializing the database.
```
01_DB.sql
02_UPDATES.sql
```
Start by creating a new database.
Then run the scripts in the order that you see them above.  The database should now be initialized and ready for use.

Set the following environment variables for communication with the database
```
MLPOS_DBHost=your_db_host
MLPOS_DBPort=your_db_port
MLPOS_DBName=your_db_name
MLPOS_DBUser=your_db_user
MLPOS_DBPass=your_db_pass
```

Run the web app with:
```
cd mlpos/MLPos.Web
dotnet run
```

The web app should now be accessible on http://localhost:5047

### Registering a user
User registration is done via commandline tool under `mlpos/MLPos.Utilities`.  Run the utilities program and select the `CreateUser` function.  Follow the instructions and you get a username & hashed password combo which you can then manually insert into your newly created database.

Example:
```
INSERT INTO mluser(username, hashed_password) values('admin', 'AQAAAAIAAYagAAAAEA8zdhBEDyKqPmKQyal7ioUlBWldfOYVGT7b1eIbexJE6H+yJFgtqdXTHr4KhAw+yg==');
```

## Repository structure:
The repository is split up into the following projects:
 * MLPos.Core
    * Includes domain entities as well as application interfaces.
 * MLPos.Data
    * Includes implementations for repositories defined in MLPos.Core.
 * MLPos.Services
    * Includes implementations for services defined in MLPos.Core.
 * MLPos.Web
    * Includes web app as well as react front POS front end.
 * MLPos.Utilities
    * Miscellaneous utitilies for use outside of application.  Application is not dependent on this project.
