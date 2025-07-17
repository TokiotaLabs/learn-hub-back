using LearnHub.Back.Domain;
using System.ComponentModel.DataAnnotations;

namespace LearnHub.Back.Application.DTOs
{
    /// <summary>
    /// Represents a course with demand information
    /// </summary>
    public class CourseDemandDto
    {
        /// <summary>
        /// Unique identifier of the course
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the course
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Detailed description of the course
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Start date of the course
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the course
        /// </summary>
        public DateTime EndDate { get; set; }

        public int Duration { get; set; }
        public decimal Price { get; set; }
        public string? Prerequisites { get; set; }
        public Guid InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
        public string? Modality { get; set; }
        public string? IncludedMaterials { get; set; }
        public string? Certification { get; set; }
        public int AvailableSeats { get; set; }
        public string? Location { get; set; }
        public string? Category { get; set; }

        /// <summary>
        /// Number of enrollments for this course (demand indicator)
        /// </summary>
        public int EnrollmentCount { get; set; }
    }
}
