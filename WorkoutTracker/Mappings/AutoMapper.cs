using AutoMapper;
using WorkoutTracker.DTO;
using WorkoutTracker.Models;

namespace WorkoutTracker.Mappings
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<ExerciseTypeDto, ExerciseType>().ReverseMap();
        }
    }
}
