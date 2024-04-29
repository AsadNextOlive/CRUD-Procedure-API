﻿using CRUD_Procedure_API.Data;
using CRUD_Procedure_API.Model;
using CRUD_Procedure_API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CRUD_Procedure_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ErrorResponseService _errorResponseService;

        public EmployeeController(ApplicationDbContext context, ErrorResponseService errorResponseService)
        {
            _context = context;
            _errorResponseService = errorResponseService;
        }

        //GET: List Employee
        [HttpGet("index")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            try
            {
                //Stored Procedure
                var employeeList = await _context.Employee.FromSqlRaw("EXEC GetEmployeeList").ToListAsync();

                if (employeeList == null || employeeList.Count == 0)
                {
                    var errorResponse = _errorResponseService.CreateErrorResponse(404, "No employee found");
                    return BadRequest(errorResponse);
                }

                var response = new
                {
                    Status = 200,
                    Message = "Employee list loaded successfully",
                    Data = employeeList
                };
                return Created("", response);

            }
            catch (Exception)
            {
                var errorResponse = _errorResponseService.CreateErrorResponse(500, "Internal Server Error");
                return StatusCode(500, errorResponse);
            }
        }

        //POST: Create Employee
        [HttpPost("create")]
        public async Task<ActionResult<Employee>> EmployeeRegister(Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existEmployee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeEmail == employee.EmployeeEmail);
                    if (existEmployee != null)
                    {
                        var errorResponse = _errorResponseService.CreateErrorResponse(400, "Email already registered");
                        return BadRequest(errorResponse);
                    }

                    // Using Stored Procedure to string data
                    var result = await _context.Database.ExecuteSqlRawAsync(
                        "EXEC CreateEmployee @p0, @p1, @p2, @p3, @p4",
                        employee.EmployeeName,
                        employee.EmployeeEmail,
                        employee.Password,
                        employee.Address,
                        employee.LastUpdate);

                    var response = new
                    {
                        Status = 200,
                        Message = "Employee registered successfully",
                        Data = employee
                    };
                    return Created("", response);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                var errorResponse = _errorResponseService.CreateErrorResponse(500, "Internal Server Error");
                return StatusCode(500, errorResponse);
            }
        }

    }
}
