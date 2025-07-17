using AutoMapper;
using LearnHub.Back.Application.DTOs;
using LearnHub.Back.Application.Handlers.Course;
using LearnHub.Back.Domain;

namespace LearnHub.Back.Application.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>();

        CreateMap<Course, CourseDemandDto>()
            .ForMember(dest => dest.EnrollmentCount, opt => opt.MapFrom(src => src.Enrollments.Count));

        CreateMap<CreateCourseCommand, Course>();
        CreateMap<UpdateCourseCommand, Course>();
    }
}