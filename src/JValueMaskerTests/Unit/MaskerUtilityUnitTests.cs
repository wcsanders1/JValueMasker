using JValueMasker.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;


namespace JValueMaskerTests.Unit
{
    [Trait("Category", "Unit")]
    public class MaskerUtilityUnitTests
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
            public void ReturnsNull_WhenProvidedNullJToken()
            {
                var result = MaskerUtility.Mask((JToken)null, new List<string>());

                Assert.Null(result);
            }

            [Fact]
            public void ReturnsJToken_WhenProvidedNullPropertiesToMask()
            {
                const string passwordVal = "badpassword123";

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = MaskerUtility.Mask(jProp, null);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(passwordVal, result.Value);
            }

            [Fact]
            public void ReturnsJToken_WhenProvidedEmptyPropertiesToMask()
            {
                const string passwordVal = "badpassword123";

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = MaskerUtility.Mask(jProp, new List<string>());

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(passwordVal, result.Value);
            }

            [Fact]
            public void ReturnsMaskedValue_WhenValueShouldBeMasked()
            {
                const string passwordVal = "badpassword123";

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = MaskerUtility.Mask(jProp, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(MaskerUtility.DefaultMask, result.Value);
            }

            [Fact]
            public void ReturnsOriginalValue_WhenValueShouldNotBeMasked()
            {
                const string nameProp = "name";
                const string nameVal = "Sam";

                var jProp = new JProperty(nameProp, nameVal);
                var result = MaskerUtility.Mask(jProp, PropsToMask);

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

                var result = MaskerUtility.Mask(jObj, PropsToMask);

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
                var result = MaskerUtility.Mask(jObj, PropsToMask);
                var maskedTeacher = result.ToObject<Teacher>();
                var student1 = maskedTeacher.Students.FirstOrDefault(s => s.Name == student1Name);
                var student2 = maskedTeacher.Students.FirstOrDefault(s => s.Name == student2Name);

                Assert.NotNull(result);
                Assert.IsType<JObject>(result);
                Assert.Equal(MaskerUtility.DefaultMask, maskedTeacher.Password);
                Assert.Equal(teacherName, maskedTeacher.Name);
                Assert.Equal(teacherAge, maskedTeacher.Age);
                Assert.Equal(teacherAddress, maskedTeacher.Address);
                Assert.Equal(students.Count, maskedTeacher.Students.Count);
                Assert.Equal(MaskerUtility.DefaultMask, student1.Password);
                Assert.Equal(student1Name, student1.Name);
                Assert.Equal(student1Address, student1.Address);
                Assert.Equal(student1Age, student1.Age);
                Assert.Equal(MaskerUtility.DefaultMask, student2.Password);
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
                var teacherResult = MaskerUtility.Mask(jObj, propsToMask);
                var studentsResult = teacherResult[nameof(Teacher.Students)].ToObject<JArray>();

                Assert.NotNull(teacherResult);
                Assert.IsType<JObject>(teacherResult);
                Assert.Equal(MaskerUtility.DefaultMask, teacherResult[nameof(Teacher.Age)].Value<string>());
                Assert.Equal(students.Count, studentsResult.Count);
                Assert.Equal(MaskerUtility.DefaultMask, studentsResult[0][nameof(Student.Age)]);
                Assert.Equal(MaskerUtility.DefaultMask, studentsResult[1][nameof(Student.Age)]);
                Assert.Equal(MaskerUtility.DefaultMask, studentsResult[0][nameof(Student.Password)]);
                Assert.Equal(MaskerUtility.DefaultMask, studentsResult[1][nameof(Student.Password)]);

                Debug.WriteLine(teacherResult.ToString());
            }

            [Fact]
            public void MaskedAllMaskedProperties_InComplexJObject()
            {
                var testJObj = new JObject(
                                    new JProperty("name", "Bob"),
                                        new JProperty("something-neat", null),
                                        new JProperty(
                                            new JProperty("title", "War 'n Peace")),
                                        new JProperty(
                                            new JProperty(
                                                new JProperty("password", "badPassword"))),
                                        new JProperty("accountings", 
                                            new JArray(
                                                new JValue("password"),
                                                    new JValue(654645),
                                                    new JValue("good times"),
                                                    new JObject(
                                                        new JProperty("read less", 444),
                                                        new JProperty("password", 78789798.787),
                                                        new JProperty("meanderings", 
                                                            new JObject(
                                                            new JProperty("planets", 
                                                                new JArray(
                                                                    new JValue(
                                                                        new JValue(
                                                                            new JValue(true)))),
                                                                    new JValue("Mars"),
                                                                    new JValue("Venus"),
                                                                    new JValue("Rectus 9"),
                                                        new JObject(
                                                        new JProperty("temperature", "mild"),
                                                        new JProperty("PASSWORD", "GREAT-PASSWORD!!")))))))));

                var result = MaskerUtility.Mask(testJObj, PropsToMask);

                Debug.WriteLine(testJObj.ToString());
                Debug.WriteLine(result.ToString());
            }

            [Fact]
            public void ReturnsJArray_WhenProvidedJArray()
            {
                var jArray = new JArray(
                                 new JValue("password"),
                                     new JValue(654645),
                                     new JValue("good times"),
                                     new JObject(
                                         new JProperty("read less", 444),
                                         new JProperty("password", 78789798.787),
                                         new JProperty("meanderings", 
                                             new JObject(
                                                 new JProperty("planets", 
                                                     new JArray(
                                                         new JValue(
                                                             new JValue(
                                                                 new JValue(true)))),
                                                         new JValue("Mars"),
                                                         new JValue("Venus"),
                                                         new JValue("Rectus 9"),
                                     new JObject(
                                     new JProperty("temperature", "mild"),
                                     new JProperty("PASSWORD", "GREAT-PASSWORD!!")))))));

                var result = MaskerUtility.Mask(jArray, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JArray>(result);

                Debug.WriteLine(result.ToString());
            }

            [Fact]
            public void ReturnsEmptyJArray_WhenProvidedEmptyJArray()
            {
                var jArray = new JArray();

                var result = MaskerUtility.Mask(jArray, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JArray>(result);
                Assert.Empty(result);
            }

            [Fact]
            public void ReturnsEmptyJObject_WhenProvidedEmptyJObject()
            {
                var jObject = new JObject();
                
                var result = MaskerUtility.Mask(jObject, PropsToMask);

                Assert.NotNull(result);
                Assert.IsType<JObject>(result);
                Assert.Empty(result);
            }

            [Fact]
            public void MasksWithAlternativeMask_WhenProvidedAlternativeMask()
            {
                const string passwordVal = "badpassword123";
                const string mask = "---";

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = MaskerUtility.Mask(jProp, PropsToMask, mask: mask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(mask, result.Value);
            }

            [Fact]
            public void MasksWithNull_WhenProvidedNullMask()
            {
                const string passwordVal = "badpassword123";
                const string mask = null;

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = MaskerUtility.Mask(jProp, PropsToMask, mask: mask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(mask, result.Value);

                Debug.WriteLine(result.ToString());
            }

            [Fact]
            public void MasksWithEmptyString_WhenProvidedEmptyStringMask()
            {
                const string passwordVal = "badpassword123";
                var mask = string.Empty;

                var jProp = new JProperty(PasswordProp, passwordVal);
                var result = MaskerUtility.Mask(jProp, PropsToMask, mask: mask);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(PasswordProp, result.Name);
                Assert.Equal(mask, result.Value);

                Debug.WriteLine(result.ToString());
            }

            [Fact]
            public void AccountsForCase_WhenStringComparisonCaseSensitive()
            {
                const string passwordVal = "badpassword123";
                var stringComparison = StringComparison.InvariantCulture;

                var jProp = new JProperty("Password", passwordVal);
                var result = MaskerUtility.Mask(jProp, PropsToMask, stringComparison);

                Assert.NotNull(result);
                Assert.IsType<JProperty>(result);
                Assert.Equal(passwordVal, result.Value);

                Debug.WriteLine(result.ToString());
            }
        }
    }
}
