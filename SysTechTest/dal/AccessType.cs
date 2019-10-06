using System;
using System.ComponentModel.DataAnnotations;


namespace SysTechTest.dal
{
    public class AccessType
    {
        [Key]
        public Int64 Id { get; set; }
        public string TypeName { get; set; }
    }
}
