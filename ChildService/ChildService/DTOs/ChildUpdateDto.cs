using System.ComponentModel.DataAnnotations;

namespace ChildService.DTOs
{
    public class ChildUpdateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Range(1, 18, ErrorMessage = "Age must be between 1 and 18")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Group is required")]
        [StringLength(50, ErrorMessage = "Group name cannot be longer than 50 characters")]
        public string Group { get; set; }
    }
}
