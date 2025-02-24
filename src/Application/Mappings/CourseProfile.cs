using AutoMapper;
using LearnHub.Back.Domain;
using LearnHub.Back.Application.DTOs;

namespace LearnHub.Back.Application.Mappings;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
            .ReverseMap()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Instructor, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());
    }
}