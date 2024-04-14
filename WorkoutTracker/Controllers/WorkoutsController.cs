using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Route("api/[controller]")]
    public class WorkoutsController : AuthController
    {
        private readonly AppDbContext _db_context;
        public WorkoutsController(AppDbContext db_context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _db_context = db_context;
        }

        // GET: api/Workouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workout>>> GetWorkouts()
        {
            return await _db_context.Workouts.Where(x => x.Owner == GetCurrentUser()).Include(workout => workout.Exercises).ToListAsync();
        }

        // GET: api/Workouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Workout>> GetWorkout(int id)
        {
            var workout = await _db_context.Workouts.Include(workout => workout.Exercises)
                .FirstOrDefaultAsync(workout => workout.Id == id);

            if (workout == null)
            {
                return NotFound();
            }
            if (workout.Owner != GetCurrentUser())
            {
                return BadRequest();
            }

            return workout;
        }

        // PUT: api/Workouts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkout(int id, Workout workout)
        {
            if (id != workout.Id)
            {
                return BadRequest();
            }
            if (workout.Owner != GetCurrentUser())
            {
                return BadRequest();
            }
            // Validate exercise type.
            foreach (Exercise exercise in workout.Exercises)
            {
                ExerciseType? foundExercise = await _db_context.ExerciseTypes
                    .FirstOrDefaultAsync(x => x.Name == exercise.Name);
                if (foundExercise == null)
                {
                    return BadRequest($"Exercise {exercise.Name} is not a valid ExerciseType!");
                }
            }

            _db_context.Entry(workout).State = EntityState.Modified;

            try
            {
                await _db_context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkoutExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Workouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Workout>> PostWorkout(Workout workout)
        {
            string? owner = GetCurrentUser();
            if (owner == null)
            {
                return BadRequest();
            }
            // Validate exercise type.
            foreach (Exercise exercise in workout.Exercises)
            {
                ExerciseType? foundExercise = await _db_context.ExerciseTypes
                    .FirstOrDefaultAsync(x => x.Name == exercise.Name);
                if (foundExercise == null)
                {
                    return BadRequest($"Exercise {exercise.Name} is not a valid ExerciseType!");
                }
            }
            workout.Owner = owner;
            _db_context.Workouts.Add(workout);
            await _db_context.SaveChangesAsync();

            return CreatedAtAction("GetWorkout", new { id = workout.Id }, workout);
        }

        // DELETE: api/Workouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var workout = await _db_context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            if (workout.Owner != GetCurrentUser())
            {
                return BadRequest();
            }

            _db_context.Workouts.Remove(workout);
            await _db_context.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkoutExists(int id)
        {
            return _db_context.Workouts.Any(e => e.Id == id);
        }
    }
}
