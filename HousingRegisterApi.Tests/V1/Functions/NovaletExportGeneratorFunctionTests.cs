using Amazon.Lambda.Core;
using FluentAssertions;
using HousingRegisterApi.V1.Functions;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests
{
    public class NovaletExportGeneratorFunctionTests : DynamoDbIntegrationTests<Startup>
    {
        [Test]
        public void NovaletExportGeneratorFunctionShouldNotThrowAnException()
        {
            // Arrange
            var lamdaContextMock = new Mock<ILambdaContext>();
            lamdaContextMock.Setup(x => x.Logger.Log(It.IsAny<string>()));

            var lamdaFunction = new NovaletExportGeneratorFunction();

            Action function = () => lamdaFunction.Handle(lamdaContextMock.Object);

            function.Should().NotThrow();
        }
    }
}
