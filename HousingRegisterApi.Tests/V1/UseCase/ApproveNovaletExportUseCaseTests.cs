using HousingRegisterApi.V1.Domain.FileExport;
using HousingRegisterApi.V1.Gateways;
using HousingRegisterApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.Tests.V1.UseCase
{
    public class ApproveNovaletExportUseCaseTests
    {
        private Mock<IFileGateway> _fileGatewayMock;
        private ApproveNovaletExportUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fileGatewayMock = new Mock<IFileGateway>();
            _classUnderTest = new ApproveNovaletExportUseCase(_fileGatewayMock.Object);        }

        [Test]
        public async Task AppovingANovaletExportSetsTheFileStateAsApproved()
        {
            // Act
            string fileName = "test.csv";
            var expectedMetadata = new Dictionary<string, string>()
            {
                { "ApprovedForExport", "true"}
            };

            await _classUnderTest.Execute(fileName).ConfigureAwait(false);

            _fileGatewayMock.Verify(x => x.UpdateMetadata(fileName, expectedMetadata));
        }
    }
}
