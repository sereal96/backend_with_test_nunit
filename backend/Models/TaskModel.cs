using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "The title cannot be longer than 45 characters.")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "The description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The expiration date is mandatory.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "IsCompleted must be 0 (incomplete) or 1 (completed).")]
        public byte IsCompleted { get; set; }
    }
}
