# NBA Schedule Downloader Demo
Sample .NET 8.0 Console Application that does the following:

1. Connects to Sportradar NBA API and downloads NBA game schedule.
1. Stores the schedule in a SQL Server Express database.

# Highlights
- Uses .NET 8.0.
- Stores configuration details in `appsettings.json`.
- Leverages a <b>SQL Server Express</b> `localdb` database to store schedule data.
- Uses <b>Entity Framework Core</b> to handle reading and writing to the database.
- Implements `ILogger` using <b>NLog</b> to write logs to a local log file.
- Includes basic unit tests to verify functionality.

# How to run
1. Clone the Repo.
1. Open in <b>Visual Studio</b>.
1. Right-click on the `Nba.Sql` <b>SQL Server Database Project</b>, and Publish. 
	- If you have a <b>SQL Server Express</b> `localdb` instance running locally, you can publish directly to that.
	- The <b>SQL Server Database Project</b> utilizes a Pre-Deploy script that will automatically create an NBA database.
	- If you prefer to connect to a separate database server, you may wish to remove the Pre-Deploy script.
	- If you prefer, you can manually run the `optional-deploy-script.sql` deploy script housed in this repo's root folder.
1. Build and run the `Nba.ConsoleApp` program.
	1. If you use `localdb`, the program should run.
	1. If you prefer to connect to a separate database server, you will need to update the connection string `appsettings.json`.

You will find logs writing to the `Nba.ConsoleApp\bin` folder.

# Future Updates
If I were to continue to develop this program further, here is a list of items I would consider.

#### Overall Updates
- Review the overall program structure, namespaces, class names, and styling conventions and would ensure that it corresponds with other programs in the ecosystem.
- Incorporate logging in a fashion consistent with other programs in the ecosystem.
- Incorporate additional monitoring criteria as appropriate.
- Leverage a third-party library to automate the process of registering services in the service collection.
- Leverage a third-party library to automate generation of Sportradar data model classes.
- Leverage a third-party library such as Poly to implement retry handling for service and database calls.
- Incorporate negative unit tests to cover failure scenarios. Current unit tests were written with a focus to assist in program creation, so that business logic could run without external services (API/Database).
- Evaluate Sportradar API documentation to determine if there is more efficient ways to only fetch data that has changed since a prior run.
- Consider modifying the routine that converts Sportradar models into database models to take advantage of parallelization/multi-threading to improve performance. 
- Consider incorporating history/audit database tables to enumerate data changes.

#### Data Updates
- Consider splitting team name into City/Location and Nickname.
- Consider splitting game datetime into separate Date and Time elements. 
- Consider storing additional data, such as broadcast information, type of game (regular season, play-in tournament, all-star game, etc).
- Consider creation of a GameTeam cross-reference table that captures the two teams for each game, to improve query efficiency when finding the season schedule for a team.