using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseTypesController : AuthController
    {
        private readonly AppDbContext _context;

        public ExerciseTypesController(AppDbContext context, UserManager<IdentityUser> userManager) : base(userManager)
        {
            _context = context;
        }

        // GET: api/ExerciseTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseType>>> GetExerciseTypes()
        {
            return await _context.ExerciseTypes
                .Where(exerciseType => exerciseType.Owner == GetCurrentUser()).ToListAsync();
        }

        // GET: api/ExerciseTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseType>> GetExerciseType(int id)
        {
            var exerciseType = await _context.ExerciseTypes.FindAsync(id);

            if (exerciseType == null)
            {
                return NotFound();
            }
            string? owner = GetCurrentUser();
            if (exerciseType.Owner != owner)
            {
                return BadRequest();
            }

            return exerciseType;
        }

        // PUT: api/ExerciseTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExerciseType(int id, ExerciseType exerciseType)
        {
            if (id != exerciseType.Id)
            {
                return BadRequest();
            }
            if (exerciseType.Owner != GetCurrentUser())
            {
                return BadRequest();
            }

            _context.Entry(exerciseType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExerciseTypeExists(id))
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

        // POST: api/ExerciseTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ExerciseType>> PostExerciseType(ExerciseType exerciseType)
        {
            string? owner = GetCurrentUser();
            if (owner == null)
            {
                return BadRequest();
            }
            exerciseType.Owner = owner;
            _context.ExerciseTypes.Add(exerciseType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExerciseType", new { id = exerciseType.Id }, exerciseType);
        }

        // DELETE: api/ExerciseTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExerciseType(int id)
        {
            var exerciseType = await _context.ExerciseTypes.FindAsync(id);
            if (exerciseType == null)
            {
                return NotFound();
            }
            if (exerciseType.Owner != GetCurrentUser())
            {
                return BadRequest();
            }

            _context.ExerciseTypes.Remove(exerciseType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExerciseTypeExists(int id)
        {
            return _context.ExerciseTypes.Any(e => e.Id == id);
        }
    }
}
