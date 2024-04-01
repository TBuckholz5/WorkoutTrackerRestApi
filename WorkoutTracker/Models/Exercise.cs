using System.ComponentModel.DataAnnotations;

namespace WorkoutTracker.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required List<int> reps { get; set; }
        [Required]
        public required List<float> weights { get; set; }
    }
}
