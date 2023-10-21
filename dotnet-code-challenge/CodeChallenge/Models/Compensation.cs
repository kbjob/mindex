using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public int compensationId { get; set; } //added ID to be able to save to DB
        public Employee employee { get; set; }
        public float salary { get; set; }
        public DateTime effectiveDate { get; set; }
    }
}
