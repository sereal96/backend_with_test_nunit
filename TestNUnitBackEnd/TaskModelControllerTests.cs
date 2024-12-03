using backend.Controllers;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;


namespace TestNUnitBackEnd
{
    [TestFixture]
    public class TaskModelControllerTests
    {
        private TaskModelController? _controller;
        private Mock<AppDbConn>? _mockDbContext;        
        private AppDbConn? _dbContext;

        [SetUp]
        public void Setup()
        {            
            var options = new DbContextOptionsBuilder<AppDbConn>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new AppDbConn(options);
            _mockDbContext = new Mock<AppDbConn>(options);            
            _controller = new TaskModelController(context);
            _dbContext = new AppDbConn(options);

            _dbContext.task.AddRange(new List<TaskModel>
            {
                new TaskModel { TaskId = 1,
                                Title = "Task 1",
                                Description = "Description 1",
                                DueDate = System.DateTime.Now,
                                IsCompleted = 0 },

                new TaskModel { TaskId = 2,
                                Title = "Task 2",
                                Description = "Description 2",
                                DueDate = System.DateTime.Now,
                                IsCompleted = 1 }
            });

            _dbContext.SaveChanges();
            _controller = new TaskModelController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {   
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task GetAllTask_ReturnsAllTasks()
        {
            // Act
            var result = await _controller.GetAllTask();

            // Assert            
            Assert.That(result, Is.InstanceOf<OkObjectResult>());


            var okResult = result as OkObjectResult;
            var tasks = okResult?.Value as List<TaskModel>;

            Assert.That(tasks, Is.Not.Null, "The task collection is null.");
            Assert.That(2, Is.EqualTo(tasks.Count), "The values ​​do not match.");            
            Assert.That("Task 1", Is.EqualTo(tasks.First().Title), "The values ​​do not match..");
        }

        [Test]
        public async Task AddTask2_AddsNewTask()
        {            
            DateTime futureDate = new DateTime(2025, 12, 2, 15, 30, 0);

            // Arrange
            var newTask = new TaskModel
            {
                TaskId = 3,
                Title = "Task 3",
                Description = "Description 3",
                DueDate = futureDate, //System.DateTime.Now,
                IsCompleted = 0
            };

            // Act
            var result = await _controller.AddTask2(newTask);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var addedTask = _dbContext.task.SingleOrDefault(t => t.TaskId == 3);
            Assert.That(addedTask, Is.Not.Null, "The task collection is null.");
            Assert.That("Task 3", Is.EqualTo(addedTask.Title), "The values ​​do not match.");            
        }

        [Test]
        public async Task DeleteTask2_RemovesTask()
        {
            // Arrange
            var taskToDelete = new TaskModel { TaskId = 1 };

            // Act
            var result = await _controller.DeleteTask2(taskToDelete);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var deletedTask = _dbContext.task.SingleOrDefault(t => t.TaskId == 1);
            Assert.That(deletedTask, Is.Null, "The object is not null.");
        }

        [Test]
        public async Task UpdateTask2_UpdatesTask()
        {
            // Arrange
            DateTime fechaManual = new DateTime(2025, 12, 2, 15, 30, 0);            
            var updatedTask = new TaskModel
            {
                TaskId = 2,
                Title = "Updated Task 2",
                Description = "Updated Description 2",
                DueDate = fechaManual, //System.DateTime.Now,
                IsCompleted = 0
            };

            // Act
            var result = await _controller.UpdateTask2(updatedTask);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var taskInDb = _dbContext.task.SingleOrDefault(t => t.TaskId == 2);
            Assert.That(taskInDb, Is.Not.Null, "The task collection is null.");
            Assert.That("Updated Task 2", Is.EqualTo(taskInDb.Title), "The values ​​do not match.");
        }

        [Test]
        public async Task AddTask2_ShouldAddTaskToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbConn>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new AppDbConn(options);
            var controller = new TaskModelController(dbContext);

            var task = new TaskModel
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = 0
            };

            // Act
            var result = await controller.AddTask2(task);

            // Assert
            var tasksInDb = dbContext.task.ToList();
            Assert.That(tasksInDb.Count, Is.EqualTo(1), "No tasks were added to the database.");
            Assert.That(tasksInDb.First().Title, Is.EqualTo("Test Task"), "The task title does not match.");
            Assert.That(tasksInDb.First().IsCompleted, Is.EqualTo(0), "Task status does not match.");
        }

        [Test]
        public async Task AddTask2_ShouldReturnBadRequest_WhenModelIsInvalid()
        {   
            // Arrange         
            var options = new DbContextOptionsBuilder<AppDbConn>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new AppDbConn(options);
            var controller = new TaskModelController(dbContext);

            var task = new TaskModel
            {
                Title = null, // Invalid according to validations
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = 0
            };

            controller.ModelState.AddModelError("Title", "Title is required.");

            //Act
            var result = await controller.AddTask2(task) as BadRequestObjectResult;

            //Result
            Assert.That(result, Is.Not.Null, "BadRequest was not returned.");
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest), "The status code is not 400.");
        }

        [Test]
        public async Task AddTask2_Should_Return_CreatedAtAction_When_Valid_Task()
        {
            // Arrange
            var validTask = new TaskModel
            {
                TaskId = 3,
                Title = "Valid Task",
                Description = "Description of the task",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = 0
            };

            // Act
            var result = await _controller.AddTask2(validTask);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null, "The task collection is null.");
            var response = createdResult.Value as dynamic;
            Assert.That(response, Is.Not.Null, "The response object is null.");            
            //Assert.That(response.Message.ToString(), Is.EqualTo("Task added successfully"));
            //Assert.That(response.Task.Title.ToString(), Is.EqualTo(validTask.Title));
        }

        [Test]
        public async Task AddTask2_Should_Return_BadRequest_When_Title_Is_Empty()
        {
            // Arrange
            var invalidTask = new TaskModel
            {
                Title = "  ", // Empty title
                Description = "Description of the task",
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = 0
            };

            // Act
            var result = await _controller.AddTask2(invalidTask);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value.ToString(), Does.Contain("The title cannot be empty"));
        }

        [Test]
        public async Task AddTask2_Should_Return_BadRequest_When_DueDate_Is_Past()
        {
            // Arrange
            var invalidTask = new TaskModel
            {
                Title = "Valid Task",
                Description = "Description of the task",
                DueDate = DateTime.UtcNow.AddDays(-1), // Past date
                IsCompleted = 0
            };

            // Act
            var result = await _controller.AddTask2(invalidTask);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value.ToString(), Does.Contain("The expiration date must be a future date"));
        }        
    }
}
