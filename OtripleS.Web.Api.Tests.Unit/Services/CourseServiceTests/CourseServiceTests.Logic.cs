﻿﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE AS LONG AS SOFTWARE FUNDS ARE DONATED TO THE POOR
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using OtripleS.Web.Api.Models.Courses;
using Xunit;

namespace OtripleS.Web.Api.Tests.Unit.Services.CourseServiceTests
{
    public partial class CourseServiceTests
    {
        [Fact]
        public async Task ShouldModifyCourseAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            int randomDays = randomNumber;
            DateTimeOffset randomDate = GetRandomDateTime();
            Course randomCourse = CreateRandomCourse(dateTime: randomDate);
            Course inputCourse = randomCourse;
            Course beforeUpdateStorageCourse = randomCourse.DeepClone();
            inputCourse.UpdatedDate = beforeUpdateStorageCourse.UpdatedDate.AddDays(days: randomDays);
            Course afterUpdateStorageCourse = inputCourse;
            Course expectedCourse = afterUpdateStorageCourse;
            Guid courseId = inputCourse.Id;

            // when
            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTime())
                .Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCourseByIdAsync(courseId))
                .ReturnsAsync(beforeUpdateStorageCourse);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCourseAsync(inputCourse))
                .ReturnsAsync(afterUpdateStorageCourse);

            Course actualCourse = await this.courseService.ModifyCourseAsync(inputCourse);

            // then
            actualCourse.Should().BeEquivalentTo(expectedCourse);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTime(),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCourseByIdAsync(courseId),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCourseAsync(inputCourse),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldDeleteCourseAsync()
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Course randomCourse = CreateRandomCourse(dateTime);
            Guid inputCourseId = randomCourse.Id;
            Course inputCourse = randomCourse;
            Course storageCourse = randomCourse;
            Course expectedCourse = randomCourse;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCourseByIdAsync(inputCourseId))
                    .ReturnsAsync(inputCourse);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteCourseAsync(inputCourse))
                    .ReturnsAsync(storageCourse);

            // when
            Course actualCourse =
                await this.courseService.DeleteCourseAsync(inputCourseId);

            // then
            actualCourse.Should().BeEquivalentTo(expectedCourse);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCourseByIdAsync(inputCourseId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCourseAsync(inputCourse),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}