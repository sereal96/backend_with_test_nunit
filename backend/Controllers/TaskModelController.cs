using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskModelController : ControllerBase
    {
        private readonly AppDbConn dbContext_;

        public TaskModelController(AppDbConn dbContext)
        {
            dbContext_ = dbContext;
        }
                
        [HttpGet]
        [Route("GetAllTask")]
        public async Task<IActionResult> GetAllTask()
        {
            var tasksModel = await dbContext_.task.ToListAsync();

            return Ok(tasksModel);
        }

        [HttpPost]
        [Route("AddTask2")]
        public async Task<IActionResult> AddTask2([FromBody] TaskModel taskM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(taskM.Title))
            {
                return BadRequest(new { Error = "The title cannot be empty or just spaces." });
            }

            if (taskM.DueDate <= DateTime.UtcNow)
            {
                return BadRequest(new { Error = "The expiration date must be a future date." });
            }

            try
            {
                dbContext_.task.Add(taskM);
                await dbContext_.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAllTask), new { id = taskM.TaskId },
                    new { Message = "Task added successfully", Task = taskM });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Error = "Error saving data to database.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred.", Details = ex.Message });
            }        
        }

        [HttpDelete]
        [Route("DeleteTask2")]
        public async Task<IActionResult> DeleteTask2([FromBody] TaskModel taskM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (taskM == null || taskM.TaskId <= 0)
            {
                return BadRequest(new { Error = "You must provide a valid task ID." });
            }

            var existingTask = await dbContext_.task.FindAsync(taskM.TaskId);
            if (existingTask == null)
            {
                return NotFound(new { Error = $"No task was found with the ID {taskM.TaskId}." });
            }

            try
            {
                dbContext_.task.Remove(existingTask);
                await dbContext_.SaveChangesAsync();
                return Ok(new { Message = $"Task with ID {existingTask.TaskId} was successfully deleted.", 
                    Task = existingTask });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Error = "An error occurred while trying to delete the task.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateTask2")]
        public async Task<IActionResult> UpdateTask2([FromBody] TaskModel taskM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTask = await dbContext_.task.FindAsync(taskM.TaskId);
            if (existingTask == null)
            {
                return NotFound(new { Error = $"No task was found with the ID = {taskM.TaskId}." });
            }

            if (string.IsNullOrWhiteSpace(taskM.Title))
            {
                return BadRequest(new { Error = "The task title cannot be empty." });
            }

            if (taskM.DueDate == default)
            {
                return BadRequest(new { Error = "You must provide a valid expiration date." });
            }

            if (taskM.DueDate <= DateTime.UtcNow)
            {
                return BadRequest(new { Error = "The expiration date must be a future date." });
            }

            if (taskM.IsCompleted != 0 && taskM.IsCompleted != 1)
            {
                return BadRequest(new { Error = "The completion status must be 0 or 1." });
            }

            try
            {
                existingTask.Title = taskM.Title;
                existingTask.Description = taskM.Description;
                existingTask.DueDate = taskM.DueDate;
                existingTask.IsCompleted = taskM.IsCompleted;

                await dbContext_.SaveChangesAsync();
                return Ok(new { Message = "The task was updated successfully.", Task = existingTask });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Error = "Error trying to update task.", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Error = "An unexpected error occurred.", Details = ex.Message });
            }
        }
    }
}
