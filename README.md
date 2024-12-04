# BACKEND 

#
After downloading this project remember to restor de Data Base with the file:
## "db_isu_corp_task.sql"

The connection string is using all default parameters.  Change it based in your local configuration. 

You could tested it using "Swagger" or running "https://localhost:7282/api/TaskModel/GetAllTask".

If you restore the database correctly you should see a JSON response like:

[
  {
    "TaskId": 118,
    "Title": "new Task",
    "Description": "This is a test",
    "DueDate": "2024-12-07T19:18:00",
    "IsCompleted": 0
  },
  {
    "TaskId": 120,
    "Title": "ffff",
    "Description": "dddd",
    "DueDate": "2024-12-19T19:27:00",
    "IsCompleted": 1
  }
]
#

# Technologies Used:
## Backend Framework: 
.NET Core 7
## Database: 
MySQL
## ORM: 
Entity Framework Core
## Testing: 
NUnit for integration and unit tests

