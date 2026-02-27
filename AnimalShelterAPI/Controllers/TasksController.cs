using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnimalShelterAPI.Models;
using AnimalShelterAPI.Models.DTO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ApiContext _dbContext;
        private readonly UserManager<User> _userManager;

        public TasksController(ApiContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // ADMIN or manager assigns task to employee by id
        [HttpPost("assign/{employeeId}")]
        public async Task<IActionResult> AssignTaskToEmployee(string employeeId, [FromBody] TaskAssignmentDto dto)
        {
            var user = await _userManager.FindByIdAsync(employeeId);
            if (user == null) return NotFound("Zaposleni nije pronađen.");

            var task = new TaskAssignment
            {
                EmployeeId = employeeId,
                TaskDescription = dto.TaskDescription,
                AssignedDate = DateTime.UtcNow,
                DueDate = dto.DueDate
            };

            _dbContext.TaskAssignments.Add(task);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Zadatak dodeljen.", taskId = task.Id });
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeTasks(string employeeId)
        {
            var tasks = await _dbContext.TaskAssignments
                .Where(t => t.EmployeeId == employeeId)
                .Join(_dbContext.Users,
                      t => t.EmployeeId,
                      u => u.Id,
                      (t, u) => new TaskViewDto
                      {
                          Id = t.Id,
                          EmployeeId = t.EmployeeId,
                          TaskDescription = t.TaskDescription,
                          AssignedDate = t.AssignedDate,
                          DueDate = t.DueDate,
                          IsCompleted = t.IsCompleted,
                          EmployeeFirstName = u.FirstName,
                          EmployeeLastName = u.LastName
                      })
                .ToListAsync();

            return Ok(tasks); // Vrati samo tasks, ne objekat
        }



        // Toggle task completion (employee may toggle only their tasks, admin may toggle any)
        [HttpPut("{taskId}/toggle")]
       
        public async Task<IActionResult> ToggleTaskStatus(int taskId)
        {
            var task = await _dbContext.TaskAssignments.FindAsync(taskId);
            if (task == null) return NotFound("Zadatak nije pronađen.");

            // Privremeno bez provere autentifikacije
            task.IsCompleted = !task.IsCompleted;
            await _dbContext.SaveChangesAsync();

            return Ok(new { task.Id, task.IsCompleted });
        }

        // Admin može da vidi sve zadatke (sa info o zaposlenom)
        [HttpGet("all")]
       
        public IActionResult GetAllTasks()
        {
            var tasks = _dbContext.TaskAssignments
                        .Join(_dbContext.Users,
                              t => t.EmployeeId,
                              u => u.Id,
                              (t, u) => new TaskViewDto
                              {
                                  Id = t.Id,
                                  EmployeeId = t.EmployeeId,
                                  TaskDescription = t.TaskDescription,
                                  AssignedDate = t.AssignedDate,
                                  DueDate = t.DueDate,
                                  IsCompleted = t.IsCompleted,
                                  EmployeeFirstName = u.FirstName,
                                  EmployeeLastName = u.LastName
                              })
                        .ToList();

            return Ok(tasks);
        }
    }
}
