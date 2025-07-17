using AutoMapper;
using LearnHub.Back.Application.DTOs;
using LearnHub.Back.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Back.Application.Handlers.Course
{
    public class GetTopDemandCoursesQueryHandler : IRequestHandler<GetTopDemandCoursesQuery, List<CourseDemandDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTopDemandCoursesQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CourseDemandDto>> Handle(GetTopDemandCoursesQuery request, CancellationToken cancellationToken)
        {
            var topCourses = await _context.Courses
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(request.TopCount)
                .ProjectTo<CourseDemandDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return topCourses;
        }
    }
}
