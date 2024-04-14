using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Workout
    {
        public int Id { get; set; }
        [Required]
        public required string Owner { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}}", ApplyFormatInEditMode = true)]
        public required DateTime Date { get; set; }
        [Required]
        public required List<Exercise> Exercises { get; set; }
    }
}
