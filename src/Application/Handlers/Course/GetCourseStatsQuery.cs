using MediatR;
using LearnHub.Back.Application.DTOs;

namespace LearnHub.Back.Application.Handlers.Course
{
    public class GetCourseStatsQuery : IRequest<CourseStatsDto>
    {
    }
}