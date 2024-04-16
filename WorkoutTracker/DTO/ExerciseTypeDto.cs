using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.DTO
{
    public class ExerciseTypeDto
    {
        public int? Id { get; set; }
        [Required]
        public required string Name { get; set; }
    }
}
