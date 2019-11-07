using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem.Data
{
    public class SeedStudentData
    {

        public void Seed(StudentSystemContext context)
        {
            context.Students.Add(new Student
            {
                Name = "Petko Boshnakov",
                PhoneNumber = "0882025779",
                Birthday = DateTime.Parse("15.01.1997"),
                RegisteredOn = DateTime.UtcNow

            });

            context.Students.Add(new Student
            {
                Name = "Svetla Spirdonova",
                PhoneNumber = "0894234213",
                Birthday = DateTime.Parse("12.12.200"),
                RegisteredOn = DateTime.UtcNow

            });

            context.Students.Add(new Student
            {
                Name = "Stoqn Boshnakov",
                PhoneNumber = "0899310316",
                Birthday = DateTime.Parse("15.07.1967"),
                RegisteredOn = DateTime.UtcNow

            });

            context.SaveChanges();

            context
                .Courses.Add(new Course
                {
                    Name = "C# Db",
                    Description = "Learn Databases",
                    Price = 440m,
                    StartDate = DateTime.Parse("10.10.2019"),
                    EndDate = DateTime.Parse("12.12.2019")
                });

            context
                .Courses.Add(new Course
                {
                    Name = "C# Framework",
                    Description = "Learn Framework",
                    Price = 480m,
                    StartDate = DateTime.Parse("10.09.2019"),
                    EndDate = DateTime.Parse("12.11.2019")
                });

                        context
                .Courses.Add(new Course
                {
                    Name = "C# OOP",
                    Description = "Learn OOP",
                    Price = 440m,
                    StartDate = DateTime.Parse("20.11.2019"),
                    EndDate = DateTime.Parse("12.01.2020")
                });

            context.SaveChanges();

            context
                .Resources
                .Add(new Resource
                {
                    CourseId = 1,
                    Name = "Resource",
                    Url = "https:...",
                    ResourceType = ResourceType.Document
                });

                        context
                .Resources
                .Add(new Resource
                {
                    CourseId = 3,
                    Name = "Database",
                    Url = "https:...",
                    ResourceType = ResourceType.Other
                });

                        context
                .Resources
                .Add(new Resource
                {
                    CourseId = 2,
                    Name = "Resource",
                    Url = "https:...",
                    ResourceType = ResourceType.Video
                });

            context.SaveChanges();

            context
                .HomeworkSubmissions
                .Add(new Homework
                {
                    StudentId = 1,
                    CourseId = 2,
                    ContentType = ContentType.Pdf,
                    Content = "Root"

                });

            context
                .HomeworkSubmissions
                .Add(new Homework
                {
                    StudentId = 2,
                    CourseId = 1,
                    ContentType = ContentType.Application,
                    Content = "Root"

                });

                        context
                .HomeworkSubmissions
                .Add(new Homework
                {
                    StudentId = 3,
                    CourseId = 2,
                    ContentType = ContentType.Zip,
                    Content = "Root"

                });

            context.SaveChanges();

            context
                .StudentCourses
                .Add(new StudentCourse
                {
                    CourseId = 1,
                    StudentId = 3
                });

                        context
                .StudentCourses
                .Add(new StudentCourse
                {
                    CourseId = 1,
                    StudentId = 2
                });

                        context
                .StudentCourses
                .Add(new StudentCourse
                {
                    CourseId = 1,
                    StudentId = 1
                });

                        context
                .StudentCourses
                .Add(new StudentCourse
                {
                    CourseId = 2,
                    StudentId = 1
                });

                        context
                .StudentCourses
                .Add(new StudentCourse
                {
                    CourseId = 2,
                    StudentId = 3
                });

            context.SaveChanges();
        }
    }
}
