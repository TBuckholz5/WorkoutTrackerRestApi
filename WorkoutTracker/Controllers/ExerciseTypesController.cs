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
    public class ExerciseTypesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ExerciseTypesController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // GET: api/ExerciseTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseType>>> GetExerciseTypes()
        {
            return await _dbContext.ExerciseTypes
                .Where(exerciseType => exerciseType.Owner == _userManager.GetUserId(User)).ToListAsync();
        }

        // GET: api/ExerciseTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseType>> GetExerciseType(int id)
        {
            var exerciseType = await _dbContext.ExerciseTypes.FindAsync(id);

            if (exerciseType == null)
            {
                return NotFound();
            }
            string? owner = _userManager.GetUserId(User);
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
            if (exerciseType.Owner != _userManager.GetUserId(User))
            {
                return BadRequest();
            }

            _dbContext.Entry(exerciseType).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
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
            string? owner = _userManager.GetUserId(User);
            if (owner == null)
            {
                return BadRequest();
            }
            exerciseType.Owner = owner;
            _dbContext.ExerciseTypes.Add(exerciseType);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetExerciseType", new { id = exerciseType.Id }, exerciseType);
        }

        // DELETE: api/ExerciseTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExerciseType(int id)
        {
            var exerciseType = await _dbContext.ExerciseTypes.FindAsync(id);
            if (exerciseType == null)
            {
                return NotFound();
            }
            if (exerciseType.Owner != _userManager.GetUserId(User))
            {
                return BadRequest();
            }

            _dbContext.ExerciseTypes.Remove(exerciseType);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool ExerciseTypeExists(int id)
        {
            return _dbContext.ExerciseTypes.Any(e => e.Id == id);
        }
    }
}
