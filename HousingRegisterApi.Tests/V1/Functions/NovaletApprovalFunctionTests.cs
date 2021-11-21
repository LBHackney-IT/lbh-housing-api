using Amazon.Lambda.Core;
using FluentAssertions;
using HousingRegisterApi.V1.Functions;
using Moq;
using NUnit.Framework;
using System;

namespace HousingRegisterApi.Tests
{
    public class NovaletApprovalFunctionTests : DynamoDbIntegrationTests<Startup>
    {
        [Test]
        public void NovaletApprovalFunction()
        {
            // Arrange
            var lamdaContextMock = new Mock<ILambdaContext>();
            lamdaContextMock.Setup(x => x.Logger.Log(It.IsAny<string>()));

            var lamdaFunction = new NovaletApprovalFunction();

            Action function = () => lamdaFunction.Handle(lamdaContextMock.Object);

            function.Should().NotThrow();
        }
    }
}
