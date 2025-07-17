using AutoMapper;
using LearnHub.Back.Application.DTOs;
using LearnHub.Back.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Back.Application.Handlers.Course
{
    public class GetCourseStatsQueryHandler : IRequestHandler<GetCourseStatsQuery, CourseStatsDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetCourseStatsQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CourseStatsDto> Handle(GetCourseStatsQuery request, CancellationToken cancellationToken)
        {
            var result = new CourseStatsDto();

            // Get total number of enrolled students across all courses
            result.TotalEnrolledStudents = await _context.Enrollments
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync(cancellationToken);

            // Get the course with the most enrollments
            var mostDemandedCourse = await _context.Courses
                .Include(c => c.Instructor)
                .Select(c => new { Course = c, EnrollmentCount = c.Enrollments.Count })
                .OrderByDescending(x => x.EnrollmentCount)
                .FirstOrDefaultAsync(cancellationToken);

            if (mostDemandedCourse != null)
            {
                result.MostDemandedCourse = _mapper.Map<CourseDto>(mostDemandedCourse.Course);
            }

            return result;
        }
    }
}