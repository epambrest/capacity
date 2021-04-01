# **[Apllication Web-site](https://capacity2020.azurewebsites.net "")**
# Technical documentation on Capacity. 
## General description 
This web application can be used for flexible organizing working time in different fields with Scrum principles. In general you can create and manage teams, sprints, tasks and team members.
## Starting the project
We recommend the following developer tools for running and development project:  
* **[IDE Visual Studio 2019 Community](https://visualstudio.microsoft.com/ru/vs/community/ "")** 
* **[MS-SQL Server Express 2017](https://www.microsoft.com/en-us/download/details.aspx?id=55994 "")** 
* **[Git Bash for windows](https://gitforwindows.org/ "")**
* **[Microsoft SQL Server Managment Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15 "")**
### Steps for running project:
1. Download and install MS Visual Studio. When you install VS 2019 Community make sure you select **“ASP.NET and web development”** package.  
**[Please follow the link for more information](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2019 "")**
2. Download and install MS-SQL Server Express. **[Guide](https://www.sqlshack.com/how-to-install-sql-server-express-edition/ "")**
3. Download and install Git Bash for windows with default installation settings. 
4. Open Git Bash and set-up your name and e-mail by using next commands: 
 - **git config --global user.name "your full name"**
 - **git config --global user.email "your email"**  
**[Please follow the link for more information](https://git-scm.com/book/en/v2/Getting-Started-First-Time-Git-Setup "")**
5. Open Git console, push and select  the path where the project will be located  by using the command: **cd "your full path"**
6. Clone this repository by using the command: **git clone “https url”**
7. Go to the root project directory by using the command: **cd “path for root directory”**
8. Open the file **“Teams.sln”** in the root directory
9. Select **"Teams.Web"** as a start-up project
10. Click on the run button (CTRL + F5). Please note, the first start-up can be long.    
### «!» If you have some problems with project, you can write to someone from the team. 
## Common Information about project
### Our project uses next stack of technologies (List can be changed in the future): 
* C#; 
* Asp Net Core MVC;
* JavaScript;
* Jquery;
* Entity Framework Core;
* Bootstrap;
* HTML;
* CSS;
* SQL;
* Docker.
### In the main this project consists of the following six parts:
1. **Teams.Business**  part contains all business application logic. We use services and Dependency Injection for flexible and right organization of program. Almost all of our data and process functions are managed with services.
2. **Teams.Data**  part contains all project models, migrations, repositories and application context. Models describe all the program entities. Migrations are needed for update database condition. Repositories allow the application to manage database. Application context links all entities with each other
3. **Teams.Security** part checks the authorization access for a current user.
4. **Teams.Web**  part contains all program controllers, views, modelVIews, localization files. Controllers link the business logic and the visual part of the application (Views). Localization files are used for translating text information in project’s views.
5. **Teams.Business** tests part contains all unit tests for services. They test most of methods which are in services. We have comprehensible tests names and thus you can understand what part of program is tested.
6. **Teams.Data.Tests** part contains all unit tests for repositories. In the main all repositories are tested for their stable work with SQL database.  
### Running unit-tests.
You can run those tests in Visual Studio tests explorer. if you press **"CTRL + E"**, T tests explorer will be opened on the left part of the screen, after this press **"CTRL + R, V"** (this will run all tests in the project, if you need to run one test or a part you can use the panel with buttons in the tests explorer).
### Github actions:
1. **“.NET_Auto_Tests”** runs all unit tests in the project when someone create a new pull request or when someone merges a branch with the master.
2. **“Build and Push Docker Container to Azure Container Registry”** builds and pushes the docker container to Azure Container Registry when someone from the team merges a branch with the master.
3. **“Create versioning PR”** updates current application version when someone from the team merges a branch with the master. You can see the current application version in the file **“src/Teams.Web/appsettings.json”**.
## Workflow to merge and deliver application. Steps (for team members):
1. If you want to get task, go to **[issues](https://github.com/epambrest/capacity/issues "")** and assign yourself to any tasks which you want to do.
2. All tasks have **Story Points**. "Story points are units of measure for expressing an estimate of the overall effort required to fully implement a product backlog item or any other piece of work." Story points are added in weekly team meeting.
3. Go to **[Projects](https://github.com/epambrest/capacity/projects "")** and drag and drop your task from **“To do”** to **“In progress”**.
4. Create a new Branch with the name of the current task number. You can use the following command: **git branch “number of task”**.
5. Please, when you are doing task don't forget to make a commit from time in time. You can use the following command: **git commit -m “name of commit”**
6. After your task has been done, you have to push your branch to repository.
7. Go to **[repository](https://github.com/epambrest/capacity "")** in your browser and press **“compare & pull request” button**. If your branch doesn’t have problems with tests, you can go to the next step.
8. Describe your changes as detailed as you can in the English language. We recommend attaching screenshots and videos if possible.
9. Send the link to your pull request to our skype chat and wait unti team members approve your pull request. After that you can merge your branch with the master.
10. Automated pull request appears after merging, you and three other team member have to approve it for upping current version.
11. Go to **[Projects](https://github.com/epambrest/capacity/projects "")** and drag and drop your task from **“In progress”** to **“Done”**.
12. If you notice some a bug or you have a new offer for the project, you can create new issue and describe them. Please, write your thoughts as detailed as you can. You can attach screenshots and videos. Don’t forget to set the right number for the issue.
13. The number of tasks InProgress per person cannot be more than 2.
14. Priority of tasks from top to bottom.
