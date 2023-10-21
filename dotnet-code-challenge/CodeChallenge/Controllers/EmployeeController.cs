using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IReportingStructureSevice _reportingStructureSevice;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService, IReportingStructureSevice reportingStructureSevice)
        {
            _logger = logger;
            _employeeService = employeeService;
            _reportingStructureSevice = reportingStructureSevice;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        //Create new GET Endpoint within existing EmployeeController to return the ReportingStructure for a given EmployeeID
        [HttpGet("getReportingStructurByEmployeeID/{id}")]
        public IActionResult GetReportingStructurByEmployeeID(String id)
        {
            _logger.LogDebug($"Received GetReportingStructurByEmployeeID get request for '{id}'");

            //Grab employee if they exist
            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            //Create a new ReportingStructure object and populate it
            ReportingStructure reportingStructure = _reportingStructureSevice.GetByEmployee(employee);

            return Ok(reportingStructure);
        }

        //New GET endpoint to grab compensation data by employeeID
        [HttpGet("getCompensationById/{id}")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Recieved GetCompensationById update request for '{id}'");

            Compensation compensation = _employeeService.GetByIdWithCompensation(id);
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }

        //New POST endpoint to add Compensation for an employeeID
        [HttpPost("addNewCompensation/{id}")]
        public IActionResult AddNewCompensation(String id, [FromForm]Compensation compensation)
        {
            _logger.LogDebug($"Recieved AddNewCompensation post request for '{id}'");

            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            compensation.employee = employee;
            var returnComp = _employeeService.AddNewCompensation(compensation);

            return Ok(returnComp);
        }
    }
}
