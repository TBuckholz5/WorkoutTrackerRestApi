using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.DTO;
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
        private readonly IMapper _mapper;

        public ExerciseTypesController(AppDbContext dbContext, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: api/ExerciseTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseTypeDto>>> GetExerciseTypes()
        {
            return await _dbContext.ExerciseTypes
                .Where(exerciseType => exerciseType.Owner == _userManager.GetUserId(User))
                .ProjectTo<ExerciseTypeDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // GET: api/ExerciseTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseTypeDto>> GetExerciseType(int id)
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

            return _mapper.Map<ExerciseTypeDto>(exerciseType);
        }

        // PUT: api/ExerciseTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExerciseType(int id, ExerciseTypeDto exerciseTypeDto)
        {
            ExerciseType? exerciseType = await _dbContext.ExerciseTypes.FindAsync(id);
            if (exerciseType == null)
            {
                return NotFound();
            }
            if (exerciseType.Owner != _userManager.GetUserId(User))
            {
                return BadRequest();
            }

            ExerciseType newExerciseType = _mapper.Map<ExerciseType>(exerciseTypeDto);
            newExerciseType.Id = id;
            newExerciseType.Owner = exerciseType.Owner;

            _dbContext.Entry(newExerciseType).State = EntityState.Modified;

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
        public async Task<ActionResult<ExerciseTypeDto>> PostExerciseType(ExerciseTypeDto postExerciseTypeDto)
        {
            string? owner = _userManager.GetUserId(User);
            if (owner == null)
            {
                return BadRequest();
            }
            ExerciseType exerciseType = _mapper.Map<ExerciseType>(postExerciseTypeDto);
            exerciseType.Owner = owner;
            _dbContext.ExerciseTypes.Add(exerciseType);
            await _dbContext.SaveChangesAsync();

            postExerciseTypeDto.Id = exerciseType.Id;

            return CreatedAtAction("GetExerciseType", new { id = exerciseType.Id }, postExerciseTypeDto);
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
