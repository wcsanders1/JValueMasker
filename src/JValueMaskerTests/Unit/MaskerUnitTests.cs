﻿using JValueMasker.Utilities;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;


namespace JValueMaskerTests.Unit
{
    [Trait("Category", "Unit")]
    public class MaskerUnitTests
    {
        public class Mask
        {
            private const string PasswordProp = "password";
            private static readonly List<string> PropsToMask = new List<string>
            {
                PasswordProp
            };

            private class Teacher
            {
                public string Name { get; set; }
                public string Address { get; set; }
                public int Age { get; set; }
                public string Password { get; set; }
                public List<Student> Students { get; set; }
            }

            private class Student
            {
                public string Name { get; set; }
                public string Address { get; set; }
                public int Age { get; set; }
                public string Password { get; set; }
            }



            [Fact]
            public void ReturnsMaskedValue_WhenValueShouldBeMasked()
            {
                const string passwordVal = "badpassword123";

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = Masker.Mask(jProp, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(Masker.DefaultMask, result.Value);
            }

            [Fact]
            public void ReturnsOriginalValue_WhenValueShouldNotBeMasked()
            {
                const string nameProp = "name";
                const string nameVal = "Sam";

                var jProp = new JProperty(nameProp, nameVal);
                var result = Masker.Mask(jProp, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(nameProp, result.Name);
                Assert.Equal(nameVal, result.Value);
            }

            [Fact]
            public void ReturnsSameObject_WhenObjectHasNoMaskedProperties()
            {
                const string nameProp = "name";
                const string nameVal = "Sam";
                const string ageProp = "age";
                const int ageVal = 80;

                var jObj = new JObject(new JProperty(nameProp, nameVal),
                                       new JProperty(ageProp, ageVal));

                var result = Masker.Mask(jObj, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JObject>(result);
                Assert.True(result[nameProp].Value<string>() == nameVal);
                Assert.True(result[ageProp].Value<int>() == ageVal);
            }

            [Fact]
            public void MasksAllMaskedProperties_InNestedObjects()
            {
                const string password = "badPassword";
                const string teacherName = "Skinner";
                const string teacherAddress = "123 Street Rd.";
                const int teacherAge = 55;
                const string student1Name = "Bob";
                const string student2Name = "Sue";
                const int student1Age = 7;
                const int student2Age = 8;
                const string student1Address = "555 Town Blvd.";
                const string student2Address = "434 Rebel Yell Rd.";

                var students = new List<Student>
                {
                    new Student
                    {
                        Name = student1Name,
                        Age = student1Age,
                        Password = password,
                        Address = student1Address
                    },
                    new Student
                    {
                        Name = student2Name,
                        Age = student2Age,
                        Password = null,
                        Address = student2Address
                    }
                };

                var teacher = new Teacher
                {
                    Name = teacherName,
                    Age = teacherAge,
                    Password = password,
                    Address = teacherAddress,
                    Students = students
                };

                var jObj = JObject.FromObject(teacher);
                var result = Masker.Mask(jObj, PropsToMask);
                var maskedTeacher = result.ToObject<Teacher>();
                var student1 = maskedTeacher.Students.FirstOrDefault(s => s.Name == student1Name);
                var student2 = maskedTeacher.Students.FirstOrDefault(s => s.Name == student2Name);

                Assert.NotNull(result);
                Assert.IsType<JObject>(result);
                Assert.Equal(Masker.DefaultMask, maskedTeacher.Password);
                Assert.Equal(teacherName, maskedTeacher.Name);
                Assert.Equal(teacherAge, maskedTeacher.Age);
                Assert.Equal(teacherAddress, maskedTeacher.Address);
                Assert.Equal(students.Count, maskedTeacher.Students.Count);
                Assert.Equal(Masker.DefaultMask, student1.Password);
                Assert.Equal(student1Name, student1.Name);
                Assert.Equal(student1Address, student1.Address);
                Assert.Equal(student1Age, student1.Age);
                Assert.Equal(Masker.DefaultMask, student2.Password);
                Assert.Equal(student2Name, student2.Name);
                Assert.Equal(student2Address, student2.Address);
                Assert.Equal(student2Age, student2.Age);

                Debug.WriteLine(result.ToString());
            }

            [Fact]
            public void ProperlyMasksNonStringValues_WithString()
            {
                const string password = "badPassword";
                const string teacherName = "Skinner";
                const string teacherAddress = "123 Street Rd.";
                const int teacherAge = 55;
                const string student1Name = "Bob";
                const string student2Name = "Sue";
                const int student1Age = 7;
                const int student2Age = 8;
                const string student1Address = "555 Town Blvd.";
                const string student2Address = "434 Rebel Yell Rd.";

                var students = new List<Student>
                {
                    new Student
                    {
                        Name = student1Name,
                        Age = student1Age,
                        Password = password,
                        Address = student1Address
                    },
                    new Student
                    {
                        Name = student2Name,
                        Age = student2Age,
                        Password = null,
                        Address = student2Address
                    }
                };

                var teacher = new Teacher
                {
                    Name = teacherName,
                    Age = teacherAge,
                    Password = password,
                    Address = teacherAddress,
                    Students = students
                };

                var propsToMask = new List<string>
                {
                    PasswordProp,
                    "Age"
                };

                var jObj = JObject.FromObject(teacher);
                var teacherResult = Masker.Mask(jObj, propsToMask);
                var studentsResult = teacherResult[nameof(Teacher.Students)].ToObject<JArray>();

                Assert.NotNull(teacherResult);
                Assert.IsType<JObject>(teacherResult);
                Assert.Equal(Masker.DefaultMask, teacherResult[nameof(Teacher.Age)].Value<string>());
                Assert.Equal(students.Count, studentsResult.Count);
                Assert.Equal(Masker.DefaultMask, studentsResult[0][nameof(Student.Age)]);
                Assert.Equal(Masker.DefaultMask, studentsResult[1][nameof(Student.Age)]);
                Assert.Equal(Masker.DefaultMask, studentsResult[0][nameof(Student.Password)]);
                Assert.Equal(Masker.DefaultMask, studentsResult[1][nameof(Student.Password)]);

                Debug.WriteLine(teacherResult.ToString());
            }

            [Fact]
            public void MaskedAllMaskedProperties_InComplexJObject()
            {
                var testJObj = new JObject(new JProperty("name", "Bob"),
                                           new JProperty("something-neat", null),
                                           new JProperty(new JProperty("title", "War 'n Peace")),
                                           new JProperty(new JProperty(new JProperty("password", "badPassword"))),
                                           new JProperty("accountings", 
                                                new JArray(new JValue("password"),
                                                           new JValue(654645),
                                                           new JValue("good times"),
                                                           new JObject(
                                                               new JProperty("read less", 444),
                                                               new JProperty("password", 78789798.787),
                                                               new JProperty("meanderings", new JObject(
                                                                   new JProperty("planets", new JArray(
                                                                       new JValue(new JValue(new JValue(true)))),
                                                                       new JValue("Mars"),
                                                                       new JValue("Venus"),
                                                                       new JValue("Rectus 9"),
                                                           new JObject(
                                                           new JProperty("temperature", "mild"),
                                                           new JProperty("PASSWORD", "GREAT-PASSWORD!!")))))))));

                var result = Masker.Mask(testJObj, PropsToMask);

                Debug.WriteLine(testJObj.ToString());
                Debug.WriteLine(result.ToString());
            }
        }
    }
}
