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

            // Get the course with the most enrollments without complex navigation properties
            var mostDemandedCourseId = await _context.Courses
                .Select(c => new { CourseId = c.Id, EnrollmentCount = c.Enrollments.Count })
                .OrderByDescending(x => x.EnrollmentCount)
                .Select(x => x.CourseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (mostDemandedCourseId != Guid.Empty)
            {
                var courseData = await _context.Courses
                    .Where(c => c.Id == mostDemandedCourseId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (courseData != null)
                {
                    result.MostDemandedCourse = new CourseDto
                    {
                        Id = courseData.Id,
                        Title = courseData.Title,
                        Description = courseData.Description,
                        StartDate = courseData.StartDate,
                        EndDate = courseData.EndDate,
                        Duration = courseData.Duration,
                        Price = courseData.Price,
                        Prerequisites = courseData.Prerequisites,
                        InstructorId = courseData.InstructorId,
                        Modality = courseData.Modality,
                        IncludedMaterials = courseData.IncludedMaterials,
                        Certification = courseData.Certification,
                        AvailableSeats = courseData.AvailableSeats,
                        Location = courseData.Location,
                        Category = courseData.Category
                    };
                }
            }

            return result;
        }
    }
}