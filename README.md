# **[Apllication Web-site](https://capacity2020.azurewebsites.net "")**
# Capacity technical documentation. 
## Common description 
This web application can be used for organize flexible your working time in different areas. This application is being developed with Scrum principles. In the common you can create and manage teams, sprints, tasks and team members.
## Starting the project
We are recommend next developer tools for running and develop project: 
* **[IDE Visual Studio 2019 Community](https://visualstudio.microsoft.com/ru/vs/community/ "")** 
* **[MS-SQL Server Express 2017](https://www.microsoft.com/en-us/download/details.aspx?id=55994 "")** 
* **[Git Bash for windows](https://gitforwindows.org/ "")**
* **[Microsoft SQL Server Managment Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15 "")**
### Steps for running project:
1. Download and install MS Visual Studio. Please, pay attention when will you install VS 2019 Community you have to select **“ASP.NET and web development”** package.  
**[Please follow to link for more information](https://docs.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2019 "")**
2. Download and install MS-SQL Server Express. **[Guide](https://www.sqlshack.com/how-to-install-sql-server-express-edition/ "")**
3. Download and install Git Bash for windows with default installation settings. 
4. Open Git Bash and set-up your name and e-mail by using next commands: 
	  - **git config --global user.name "your full name"**
	  - **git config --global user.email "your email"**  
**[Please follow to link for more information](https://git-scm.com/book/en/v2/Getting-Started-First-Time-Git-Setup "")**
5. Open console of git push and select path where will be project by using command: **cd "your full path"**
6. Clone this repository by using command: **git clone “https url”**
7. Go to root project directory by using command: **cd “path for root directory”**
8. Open file **“Teams.sln”** in root directory
9. Select **"Teams.Web"** as start-up project
10. Click on run button (CTRL + F5). Please pay attention, the first start-up can be long.  
### «!» If you have some problems with project, please write to someone from team. 
## Common Information about project
### Our project uses next stack of technologies (List can be changed in the future): 
*	C# 
*	Asp Net Core MVC
*	JavaScript
*	Jquery
*	Entity Framework Core
*	Bootstrap
*	HTML
*	CSS
*	SQL
*	Docker
### In the common this project consists of four next parts:
1. Teams.Business  part contains all bussines application logic. We use services and Dependency Injection for flexiable and right organization of program. Almost all our data and process functions are managed with services.
2. Teams.Data  part contains all project models, migrations, repositories and application context. Models describe all the program entities. Migrations are needed for update database condition. Repository allow to manage application database. Application context links all entities with each other
3. Teams.Security part checks authorization access for current user.
4. Teams.Web  part contains all program contollers, views, modelVIews, localization files. Controllers link the business logic and the visual part of the application (Views). Localization files are used for translate text information in project’s views.
5. Teams.Business.Tests this part contains all unit tests for services. They test most of methods which are in services. We have understandable tests name and that is why you can understand what part of program is tested.
6. Temas.Data.Tests  this part contains all unit tests for repositories. In the common all repositories are tested for them stability work with SQL database.  
### Running unit-tests
You can run those tests in Visual Studio tests explorer. if you press **"CTRL + E"**, T tests explorer will be opened on the left part of the screen after this press **"CTRL + R, V"** (this will run all tests in project, if you need run one tests or part you can use panel with buttons in tests explorer).
### Github actions:
1. **“.NET_Auto_Tests”** run all unit tests in project when someone create new pull request or when someone merge  branch to master
2. **“Build and Push Docker Container to Azure Container Registry”** is building and pushing docker container to Azure Container Registry when someone from team merge branch to master
3. **“Create versioning PR”** update current application version when someone from team merge branch to master. You can see current application version in file **“src/Teams.Web/appsettings.json”**
## Workflow to merge and deliver application. Steps (for team members):
1. If you want to get task, go to **[issues](https://github.com/epambrest/capacity/issues "")** and assign yourself to any task which you want do
2. All tasks have **Story Points**. "Story points are units of measure for expressing an estimate of the overall effort required to fully implement a product backlog item or any other piece of work." Story points are added in weekly team meeting.
3. Go to **[Projects](https://github.com/epambrest/capacity/projects "")** and drag and drop your task from **“To do”** to **“In progress”**.
4. Create new Branch with name of current task number. You can use next command: **git branch “number of task”**.
5. Please, when you are doing task don't forget make commit from time in time. You can use next command: **git commit -m “name of commit”**
6. After your task has been done, you have to push your branch to repository.
7. Go to **[repository](https://github.com/epambrest/capacity "")** in your browser and press **“compare & pull request” button**. If your branch won’t have problem with tests, you can go to next step.
8. Describe your changes detail as you can on English language. We are recommended to attach screenshots and videos if it possible.
9. Send link to pull request to our skype chat and wait when for at least three members from team to approve your pull request. After that you can merge your branch to master.
10. Automated pull request is appeared after merging, you and three another team members have to approve this for upping current version
11. Go to **[Projects](https://github.com/epambrest/capacity/projects "")** and drag and drop your task from **“In progress”** to **“Done”**
12. If you noticed some bug or you have a new offer for project, you can create new issue and describe this. Please, write your thoughts detail as you can. You can attach screenshots and videos. Don’t forget set right number for issue.
