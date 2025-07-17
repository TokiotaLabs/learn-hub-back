using MediatR;
using LearnHub.Back.Application.DTOs;
using System.Collections.Generic;

namespace LearnHub.Back.Application.Handlers.Course
{
    public class GetTopDemandCoursesQuery : IRequest<List<CourseDemandDto>>
    {
        public int TopCount { get; set; } = 10;
    }
}
