using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        
        public Employee GetById(string id)
        {
            //Updated GetById to include the DirectReports as they were always returning "NULL" due to missing include
            return _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        //When grabbing the Compensation, include Employee and Direct Reports info and order by descending Compensation EffectiveDate to grab the latest salary via FirstOrDefault()
        public Compensation GetByIdWithCompensation(string id)
        {
            return _employeeContext.Compensation.Include(x => x.employee).Include(d => d.employee.DirectReports).OrderByDescending(c => c.effectiveDate).FirstOrDefault(e => e.employee.EmployeeId == id);
        }
        public Compensation AddNewCompensation(Compensation compensation)
        {
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }
    }
}
