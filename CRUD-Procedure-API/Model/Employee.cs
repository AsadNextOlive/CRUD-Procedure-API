using System.ComponentModel.DataAnnotations;

namespace CRUD_Procedure_API.Model
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string EmployeeEmail { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Address { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }
}
