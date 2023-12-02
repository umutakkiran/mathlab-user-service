using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.common
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string name { get; set; }
        public bool status { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updatedTime { get; set; }
    }
}
