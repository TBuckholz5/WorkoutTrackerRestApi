using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public WorkoutsController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: api/Workouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workout>>> GetWorkouts()
        {
            return await _dbContext.Workouts.Where(x => x.Owner == _userManager.GetUserId(User)).Include(workout => workout.Exercises).ToListAsync();
        }

        // GET: api/Workouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Workout>> GetWorkout(int id)
        {
            var workout = await _dbContext.Workouts.Include(workout => workout.Exercises)
                .FirstOrDefaultAsync(workout => workout.Id == id);

            if (workout == null)
            {
                return NotFound();
            }
            if (workout.Owner != _userManager.GetUserId(User))
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
            if (workout.Owner != _userManager.GetUserId(User))
            {
                return BadRequest();
            }
            // Validate exercise type.
            foreach (Exercise exercise in workout.Exercises)
            {
                ExerciseType? foundExercise = await _dbContext.ExerciseTypes
                    .FirstOrDefaultAsync(x => x.Name == exercise.Name);
                if (foundExercise == null)
                {
                    return BadRequest($"Exercise {exercise.Name} is not a valid ExerciseType!");
                }
            }

            _dbContext.Entry(workout).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
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
            string? owner = _userManager.GetUserId(User);
            if (owner == null)
            {
                return BadRequest();
            }
            // Validate exercise type.
            foreach (Exercise exercise in workout.Exercises)
            {
                ExerciseType? foundExercise = await _dbContext.ExerciseTypes
                    .FirstOrDefaultAsync(x => x.Name == exercise.Name);
                if (foundExercise == null)
                {
                    return BadRequest($"Exercise {exercise.Name} is not a valid ExerciseType!");
                }
            }
            workout.Owner = owner;
            _dbContext.Workouts.Add(workout);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetWorkout", new { id = workout.Id }, workout);
        }

        // DELETE: api/Workouts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var workout = await _dbContext.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            if (workout.Owner != _userManager.GetUserId(User))
            {
                return BadRequest();
            }

            _dbContext.Workouts.Remove(workout);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool WorkoutExists(int id)
        {
            return _dbContext.Workouts.Any(e => e.Id == id);
        }
    }
}
