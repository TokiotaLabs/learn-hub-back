using AutoMapper;
using FluentAssertions;
using LearnHub.Back.Application.DTOs;
using LearnHub.Back.Application.Handlers.Course;
using LearnHub.Back.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LearnHub.Back.Tests.Application.Handlers.Course
{
    [TestFixture]
    public class CourseHandlerTests
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;
        private DbContextOptions<ApplicationDbContext> _options;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<CreateCourseCommand, Domain.Course>();
                cfg.CreateMap<UpdateCourseCommand, Domain.Course>();
                cfg.CreateMap<Domain.Course, CourseDto>();
                cfg.CreateMap<Domain.Instructor, Domain.Instructor>();
                cfg.CreateMap<Domain.Enrollment, EnrollmentDto>();
                cfg.CreateMap<Domain.Student, StudentDto>();
            });
            
            _mapper = config.CreateMapper();
        }

        [Test]
        public async Task CreateCourse_WithValidData_ShouldCreateAndReturnDto()
        {
            // Arrange
            var command = new CreateCourseCommand
            {
                Title = "Test Course",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Duration = 5,
                Price = 99.99m,
                Prerequisites = "None",
                InstructorId = Guid.NewGuid(),
                Modality = "Online",
                IncludedMaterials = "None",
                Certification = "Certificate of Completion",
                AvailableSeats = 20,
                Location = "Online",
                Category = "Technology"
            };
            
            var handler = new CreateCourseCommandHandler(_mapper, _context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(command.Title);
            result.Description.Should().Be(command.Description);
            result.Price.Should().Be(command.Price);
            result.Duration.Should().Be(command.Duration);
        }

        [Test]
        public async Task UpdateCourse_WithValidData_ShouldUpdateAndReturnUnit()
        {
            // Arrange
            var course = new Domain.Course
            {
                Title = "Original Title",
                Description = "Original Description",
                Price = 50m,
                Duration = 4,
                Category = "Sample Category",
                Certification = "Sample Certification",
                IncludedMaterials = "Sample Materials",
                Location = "Sample Location",
                Modality = "Sample Modality",
                Prerequisites = "Sample Prerequisites",
                InstructorId = Guid.NewGuid() // Assuming you have a valid InstructorId
            };
            
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var command = new UpdateCourseCommand
            {
                Id = course.Id,
                Title = "Updated Title",
                Description = "Updated Description",
                Price = 75m,
                Duration = 2
            };

            var handler = new UpdateCourseCommandHandler(_mapper, _context);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var updatedCourse = await _context.Courses.FindAsync(course.Id);
            updatedCourse.Should().NotBeNull();
            updatedCourse.Title.Should().Be(command.Title);
            updatedCourse.Description.Should().Be(command.Description);
            updatedCourse.Price.Should().Be(command.Price);
            updatedCourse.Duration.Should().Be(command.Duration);
        }

        [Test]
        public async Task DeleteCourse_WithExistingId_ShouldRemoveAndSaveChanges()
        {
            // Arrange
            var course = new Domain.Course
            {
                Title = "Test Course",
                Description = "Test Description",
                Price = 99.99m,
                Duration = 2,
                Prerequisites = "None",
                Modality = "Online",
                IncludedMaterials = "None",
                Certification = "None",
                Location = "Test Location",
                Category = "Test Category"
            };
            
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var handler = new DeleteCourseCommandHandler(_context);
            var command = new DeleteCourseCommand { Id = course.Id };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var deletedCourse = await _context.Courses.FindAsync(course.Id);
            deletedCourse.Should().BeNull();
        }

        [Test]
        public async Task DeleteCourse_WithNonExistingId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var handler = new DeleteCourseCommandHandler(_context);
            var command = new DeleteCourseCommand { Id = Guid.NewGuid() };

            // Act & Assert
            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<KeyNotFoundException>();
        }

        [Test]
        public async Task GetCourseStats_WithData_ShouldReturnStatsCorrectly()
        {
            // Arrange
            var instructor = new Domain.Instructor 
            { 
                Id = Guid.NewGuid(),
                Name = "Test Instructor",
                Biography = "Test Biography"
            };
            
            var course1 = new Domain.Course
            {
                Id = Guid.NewGuid(),
                Title = "Course 1",
                Description = "Description 1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Duration = 5,
                Price = 99.99m,
                Prerequisites = "None",
                InstructorId = instructor.Id,
                Modality = "Online",
                IncludedMaterials = "Materials",
                Certification = "Certificate",
                AvailableSeats = 20,
                Location = "Online",
                Category = "Programming"
            };

            var course2 = new Domain.Course
            {
                Id = Guid.NewGuid(),
                Title = "Course 2",
                Description = "Description 2",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Duration = 10,
                Price = 199.99m,
                Prerequisites = "Basic",
                InstructorId = instructor.Id,
                Modality = "Online",
                IncludedMaterials = "Materials",
                Certification = "Certificate",
                AvailableSeats = 15,
                Location = "Online",
                Category = "Programming"
            };

            var student1 = new Domain.Student
            {
                Id = Guid.NewGuid(),
                FullName = "Student 1",
                Email = "student1@test.com",
                PhoneNumber = "123456789",
                PostalAddress = "Address 1",
                EducationLevel = "Bachelor",
                CurrentOccupation = "Developer",
                PreviousExperience = "None"
            };

            var student2 = new Domain.Student
            {
                Id = Guid.NewGuid(),
                FullName = "Student 2",
                Email = "student2@test.com",
                PhoneNumber = "987654321",
                PostalAddress = "Address 2",
                EducationLevel = "Master",
                CurrentOccupation = "Analyst",
                PreviousExperience = "Some"
            };

            // Course 2 has 2 enrollments (most demanded)
            var enrollment1 = new Domain.Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student1.Id,
                CourseId = course2.Id,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Approved",
                SchedulePreference = "Evening",
                PaymentId = Guid.NewGuid()
            };

            var enrollment2 = new Domain.Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student2.Id,
                CourseId = course2.Id,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Approved",
                SchedulePreference = "Morning",
                PaymentId = Guid.NewGuid()
            };

            // Course 1 has 1 enrollment
            var enrollment3 = new Domain.Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = student1.Id,
                CourseId = course1.Id,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Approved",
                SchedulePreference = "Evening",
                PaymentId = Guid.NewGuid()
            };

            await _context.Instructors.AddAsync(instructor);
            await _context.Courses.AddRangeAsync(course1, course2);
            await _context.Students.AddRangeAsync(student1, student2);
            await _context.Enrollments.AddRangeAsync(enrollment1, enrollment2, enrollment3);
            await _context.SaveChangesAsync();

            // Use a simpler configuration that avoids nested mapping issues
            var simpleConfig = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<Domain.Course, CourseDto>()
                    .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                    .ForMember(dest => dest.Enrollments, opt => opt.Ignore());
            });
            var simpleMapper = simpleConfig.CreateMapper();

            var handler = new GetCourseStatsQueryHandler(_context, simpleMapper);
            var query = new GetCourseStatsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalEnrolledStudents.Should().Be(2); // 2 unique students
            result.MostDemandedCourse.Should().NotBeNull();
            result.MostDemandedCourse!.Id.Should().Be(course2.Id); // Course 2 has more enrollments
            result.MostDemandedCourse.Title.Should().Be("Course 2");
        }

        [Test]
        public async Task GetCourseStats_WithNoData_ShouldReturnEmptyStats()
        {
            // Arrange
            var simpleConfig = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<Domain.Course, CourseDto>()
                    .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                    .ForMember(dest => dest.Enrollments, opt => opt.Ignore());
            });
            var simpleMapper = simpleConfig.CreateMapper();
            
            var handler = new GetCourseStatsQueryHandler(_context, simpleMapper);
            var query = new GetCourseStatsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalEnrolledStudents.Should().Be(0);
            result.MostDemandedCourse.Should().BeNull();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}