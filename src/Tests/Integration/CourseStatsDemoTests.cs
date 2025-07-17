using FluentAssertions;
using LearnHub.Back.Application.DTOs;
using LearnHub.Back.Domain;
using LearnHub.Back.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using AutoMapper;
using LearnHub.Back.Application.Handlers.Course;
using System.Text.Json;

namespace LearnHub.Back.Tests.Integration;

[TestFixture]
public class CourseStatsDemoTests
{
    private DbContextOptions<ApplicationDbContext> _options;
    private string _databaseName;
    private IMapper _mapper;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _databaseName = $"CourseStatsDemoDb_{Guid.NewGuid()}";
        
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
    public async Task DemonstrateEndpointWithRealisticData()
    {
        // Arrange - Set up a realistic learning platform scenario
        using var context = new ApplicationDbContext(_options);
        
        var instructor1 = new Instructor 
        { 
            Id = Guid.NewGuid(),
            Name = "Dr. Sarah Johnson",
            Biography = "Senior Software Engineer with 15 years experience in web development"
        };

        var instructor2 = new Instructor 
        { 
            Id = Guid.NewGuid(),
            Name = "Prof. Michael Chen",
            Biography = "Data Science expert and AI researcher"
        };

        var courses = new[]
        {
            new Course
            {
                Id = Guid.NewGuid(),
                Title = "React & Next.js Masterclass",
                Description = "Learn modern React development with Next.js framework",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(67),
                Duration = 40,
                Price = 449.99m,
                Prerequisites = "Basic JavaScript knowledge",
                InstructorId = instructor1.Id,
                Modality = "Online",
                IncludedMaterials = "Video tutorials, Code samples, Project templates",
                Certification = "React Developer Certificate",
                AvailableSeats = 50,
                Location = "Online Platform",
                Category = "Web Development"
            },
            new Course
            {
                Id = Guid.NewGuid(),
                Title = "Machine Learning Fundamentals",
                Description = "Introduction to ML algorithms and practical applications",
                StartDate = DateTime.UtcNow.AddDays(14),
                EndDate = DateTime.UtcNow.AddDays(74),
                Duration = 60,
                Price = 699.99m,
                Prerequisites = "Python programming, Basic statistics",
                InstructorId = instructor2.Id,
                Modality = "Hybrid",
                IncludedMaterials = "Video lectures, Jupyter notebooks, Datasets",
                Certification = "ML Fundamentals Certificate",
                AvailableSeats = 25,
                Location = "Online + Lab Sessions",
                Category = "Data Science"
            },
            new Course
            {
                Id = Guid.NewGuid(),
                Title = "Python for Beginners",
                Description = "Complete Python programming course for absolute beginners",
                StartDate = DateTime.UtcNow.AddDays(21),
                EndDate = DateTime.UtcNow.AddDays(51),
                Duration = 30,
                Price = 299.99m,
                Prerequisites = "None",
                InstructorId = instructor1.Id,
                Modality = "Online",
                IncludedMaterials = "Interactive exercises, eBook, Practice projects",
                Certification = "Python Basics Certificate",
                AvailableSeats = 100,
                Location = "Online Platform",
                Category = "Programming"
            }
        };

        var students = new[]
        {
            new Student { Id = Guid.NewGuid(), FullName = "Alice Rodriguez", Email = "alice@email.com", PhoneNumber = "+1234567890", PostalAddress = "123 Tech St", EducationLevel = "Bachelor's", CurrentOccupation = "Junior Developer", PreviousExperience = "HTML/CSS" },
            new Student { Id = Guid.NewGuid(), FullName = "Bob Thompson", Email = "bob@email.com", PhoneNumber = "+1987654321", PostalAddress = "456 Code Ave", EducationLevel = "High School", CurrentOccupation = "Student", PreviousExperience = "None" },
            new Student { Id = Guid.NewGuid(), FullName = "Carol Martinez", Email = "carol@email.com", PhoneNumber = "+1555666777", PostalAddress = "789 Data Blvd", EducationLevel = "Master's", CurrentOccupation = "Data Analyst", PreviousExperience = "SQL, Excel" },
            new Student { Id = Guid.NewGuid(), FullName = "David Kim", Email = "david@email.com", PhoneNumber = "+1444555666", PostalAddress = "321 Learn Lane", EducationLevel = "Associate", CurrentOccupation = "Designer", PreviousExperience = "Basic programming" },
            new Student { Id = Guid.NewGuid(), FullName = "Eva Singh", Email = "eva@email.com", PhoneNumber = "+1777888999", PostalAddress = "654 Study Street", EducationLevel = "Bachelor's", CurrentOccupation = "Marketing", PreviousExperience = "None" }
        };

        // Create enrollments to make "React & Next.js Masterclass" the most demanded (4 enrollments)
        var enrollments = new List<Enrollment>
        {
            // React course - 4 enrollments (most demanded)
            new() { Id = Guid.NewGuid(), StudentId = students[0].Id, CourseId = courses[0].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-5), Status = "Approved", SchedulePreference = "Evening", PaymentId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), StudentId = students[1].Id, CourseId = courses[0].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-4), Status = "Approved", SchedulePreference = "Weekend", PaymentId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), StudentId = students[2].Id, CourseId = courses[0].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-3), Status = "Approved", SchedulePreference = "Evening", PaymentId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), StudentId = students[3].Id, CourseId = courses[0].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-2), Status = "Approved", SchedulePreference = "Morning", PaymentId = Guid.NewGuid() },
            
            // ML course - 2 enrollments
            new() { Id = Guid.NewGuid(), StudentId = students[2].Id, CourseId = courses[1].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-3), Status = "Approved", SchedulePreference = "Weekdays", PaymentId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), StudentId = students[4].Id, CourseId = courses[1].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-1), Status = "Approved", SchedulePreference = "Weekends", PaymentId = Guid.NewGuid() },
            
            // Python course - 1 enrollment
            new() { Id = Guid.NewGuid(), StudentId = students[1].Id, CourseId = courses[2].Id, EnrollmentDate = DateTime.UtcNow.AddDays(-1), Status = "Approved", SchedulePreference = "Flexible", PaymentId = Guid.NewGuid() }
        };

        // Note: Total unique students = 5 (Alice, Bob, Carol, David, Eva)
        // Some students are enrolled in multiple courses but count only once

        context.Instructors.AddRange(instructor1, instructor2);
        context.Courses.AddRange(courses);
        context.Students.AddRange(students);
        context.Enrollments.AddRange(enrollments);
        await context.SaveChangesAsync();

        // Act
        var handler = new GetCourseStatsQueryHandler(context, _mapper);
        var result = await handler.Handle(new GetCourseStatsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalEnrolledStudents.Should().Be(5, "because there are 5 unique students enrolled across all courses");
        result.MostDemandedCourse.Should().NotBeNull();
        result.MostDemandedCourse!.Title.Should().Be("React & Next.js Masterclass", "because it has 4 enrollments vs 2 for ML and 1 for Python");
        result.MostDemandedCourse.Price.Should().Be(449.99m);
        result.MostDemandedCourse.Category.Should().Be("Web Development");

        // Demonstrate the JSON output
        var jsonOptions = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonResult = JsonSerializer.Serialize(result, jsonOptions);
        
        TestContext.WriteLine("===== COURSE STATISTICS ENDPOINT DEMO =====");
        TestContext.WriteLine($"GET /api/course/stats");
        TestContext.WriteLine("Response (200 OK):");
        TestContext.WriteLine(jsonResult);
        TestContext.WriteLine("===========================================");
        
        // Verify the JSON structure is as expected
        jsonResult.Should().Contain("mostDemandedCourse");
        jsonResult.Should().Contain("totalEnrolledStudents");
    }
}