namespace contact_manager.Models
{
    public class ContactDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DateOfBirth { get; set; } 
        public bool? Married { get; set; }
        public string? Phone { get; set; }
        public decimal Salary { get; set; }
    }
}
