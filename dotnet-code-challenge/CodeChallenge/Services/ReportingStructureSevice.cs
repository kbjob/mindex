using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using System.Diagnostics.Metrics;

namespace CodeChallenge.Services
{
    //New ReportingStructureSevice class. I could have supported grabbing reporting structure data within the EmployeeService but this allows for future growth as a separate service
    public class ReportingStructureSevice : IReportingStructureSevice
    {
        private readonly ILogger<ReportingStructureSevice> _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureSevice(ILogger<ReportingStructureSevice> logger, IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        public ReportingStructure GetByEmployee(Employee employee)
        {
            int numOfReports = 0;
            
            //Due to limited number of reports for this challenge, I used recursion to count the number of all direct reports.
            // Otherwise I'd opt for a SQL to count direct reports for 
            numOfReports = RecursiveDirectReportCounter(employee, numOfReports);

            ReportingStructure reportingStructure = new ReportingStructure(){ 
                employee = employee,
                numberOfReports = numOfReports
            };
            return reportingStructure;
        }

        //Use recursion to loop through each Employee direct report
        private int RecursiveDirectReportCounter(Employee employee, int numOfReports)
        {
            if(employee.DirectReports != null && employee.DirectReports.Count > 0)
            {
                numOfReports += employee.DirectReports.Count;

                foreach (Employee report in employee.DirectReports)
                {
                    report.DirectReports = _employeeService.GetById(report.EmployeeId).DirectReports; //requery DB to grab DirectReports data
                    numOfReports = RecursiveDirectReportCounter(report, numOfReports);
                }
            }

            return numOfReports;
        }
    }
}
