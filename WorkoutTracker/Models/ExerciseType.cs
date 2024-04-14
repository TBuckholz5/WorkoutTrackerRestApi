using System.ComponentModel.DataAnnotations;


namespace WorkoutTracker.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }
        [Required]
        public required string Owner { get; set; }
        [Required]
        public required string Name { get; set; }
    }
}
