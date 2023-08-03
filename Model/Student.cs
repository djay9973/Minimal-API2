using System.ComponentModel.DataAnnotations;

namespace Minimal_API2.Model
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
