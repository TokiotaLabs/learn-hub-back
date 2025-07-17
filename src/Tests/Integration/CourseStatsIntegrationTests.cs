using FluentAssertions;
using LearnHub.Back.Application.DTOs;
using LearnHub.Back.Domain;
using LearnHub.Back.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using AutoMapper;
using LearnHub.Back.Application.Handlers.Course;

namespace LearnHub.Back.Tests.Integration;

[TestFixture]
public class CourseStatsIntegrationTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private string _databaseName;
    private IMapper _mapper;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _databaseName = $"CourseStatsTestDb_{Guid.NewGuid()}";
        
        var config = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile<LearnHub.Back.Application.Mappings.CourseProfile>();
            cfg.AddProfile<LearnHub.Back.Application.Mappings.EnrollmentProfile>();
            cfg.AddProfile<LearnHub.Back.Application.Mappings.StudentProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;
    }

    [TearDown]
    public async Task TearDown()
    {
        using var context = new ApplicationDbContext(_options);
        await context.Database.EnsureDeletedAsync();
    }

    [Test]
    public async Task GetCourseStats_WithCompleteScenario_ShouldReturnCorrectStats()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        
        var instructor = new Instructor 
        { 
            Id = Guid.NewGuid(),
            Name = "Dr. Jane Smith",
            Biography = "Experienced instructor with 10 years of teaching"
        };

        var course1 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Introduction to Programming",
            Description = "Learn the basics of programming",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(40),
            Duration = 30,
            Price = 299.99m,
            Prerequisites = "None",
            InstructorId = instructor.Id,
            Modality = "Online",
            IncludedMaterials = "Videos, PDFs, Exercises",
            Certification = "Certificate of Completion",
            AvailableSeats = 50,
            Location = "Online Platform",
            Category = "Programming"
        };

        var course2 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Advanced Web Development",
            Description = "Master modern web development techniques",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddDays(75),
            Duration = 60,
            Price = 599.99m,
            Prerequisites = "Basic programming knowledge",
            InstructorId = instructor.Id,
            Modality = "Hybrid",
            IncludedMaterials = "Videos, Live Sessions, Projects",
            Certification = "Professional Certificate",
            AvailableSeats = 30,
            Location = "Online + Lab Sessions",
            Category = "Web Development"
        };

        var student1 = new Student
        {
            Id = Guid.NewGuid(),
            FullName = "Alice Johnson",
            Email = "alice@example.com",
            PhoneNumber = "+1234567890",
            PostalAddress = "123 Tech Street",
            EducationLevel = "Bachelor's Degree",
            CurrentOccupation = "Junior Developer",
            PreviousExperience = "Basic HTML/CSS"
        };

        var student2 = new Student
        {
            Id = Guid.NewGuid(),
            FullName = "Bob Wilson",
            Email = "bob@example.com",
            PhoneNumber = "+1987654321",
            PostalAddress = "456 Code Avenue",
            EducationLevel = "High School",
            CurrentOccupation = "Student",
            PreviousExperience = "None"
        };

        var student3 = new Student
        {
            Id = Guid.NewGuid(),
            FullName = "Carol Davis",
            Email = "carol@example.com",
            PhoneNumber = "+1555666777",
            PostalAddress = "789 Programming Lane",
            EducationLevel = "Associate Degree",
            CurrentOccupation = "Designer",
            PreviousExperience = "Basic JavaScript"
        };

        // Create enrollments - course2 will be the most demanded with 3 enrollments
        var enrollments = new[]
        {
            new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student1.Id,
                CourseId = course2.Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-5),
                Status = "Approved",
                SchedulePreference = "Evening",
                PaymentId = Guid.NewGuid()
            },
            new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student2.Id,
                CourseId = course2.Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-3),
                Status = "Approved",
                SchedulePreference = "Morning",
                PaymentId = Guid.NewGuid()
            },
            new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student3.Id,
                CourseId = course2.Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-1),
                Status = "Approved",
                SchedulePreference = "Afternoon",
                PaymentId = Guid.NewGuid()
            },
            // Course1 has 1 enrollment
            new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student1.Id,
                CourseId = course1.Id,
                EnrollmentDate = DateTime.UtcNow.AddDays(-2),
                Status = "Approved",
                SchedulePreference = "Weekend",
                PaymentId = Guid.NewGuid()
            }
        };

        context.Instructors.Add(instructor);
        context.Courses.AddRange(course1, course2);
        context.Students.AddRange(student1, student2, student3);
        context.Enrollments.AddRange(enrollments);
        await context.SaveChangesAsync();

        // Act
        var handler = new GetCourseStatsQueryHandler(context, _mapper);
        var result = await handler.Handle(new GetCourseStatsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalEnrolledStudents.Should().Be(3, "because there are 3 unique students enrolled across all courses");
        result.MostDemandedCourse.Should().NotBeNull();
        result.MostDemandedCourse!.Id.Should().Be(course2.Id, "because course2 has 3 enrollments vs course1's 1 enrollment");
        result.MostDemandedCourse.Title.Should().Be("Advanced Web Development");
        result.MostDemandedCourse.Price.Should().Be(599.99m);
        result.MostDemandedCourse.InstructorId.Should().Be(instructor.Id);
    }

    [Test]
    public async Task GetCourseStats_WithNoCourses_ShouldReturnEmptyStats()
    {
        // Arrange
        using var context = new ApplicationDbContext(_options);
        
        // Act
        var handler = new GetCourseStatsQueryHandler(context, _mapper);
        var result = await handler.Handle(new GetCourseStatsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalEnrolledStudents.Should().Be(0);
        result.MostDemandedCourse.Should().BeNull();
    }
}