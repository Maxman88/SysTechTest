using System;
using System.ComponentModel.DataAnnotations;

namespace SysTechTest.dal
{
    public class Group
    {
        [Key]
        public Int64 Id { get; set; }
        public string GroupName { get; set; }
    }
}
