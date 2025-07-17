namespace LearnHub.Back.Application.DTOs
{
    /// <summary>
    /// Represents course statistics including the most demanded course and total enrollment count
    /// </summary>
    public class CourseStatsDto
    {
        /// <summary>
        /// The course with the highest number of enrollments
        /// </summary>
        public CourseDto? MostDemandedCourse { get; set; }

        /// <summary>
        /// Total number of enrolled students across all courses
        /// </summary>
        public int TotalEnrolledStudents { get; set; }
    }
}