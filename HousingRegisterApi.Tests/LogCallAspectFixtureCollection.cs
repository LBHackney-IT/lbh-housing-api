using Hackney.Core.Testing.Shared;
using Xunit;

namespace TenureInformationApi.Tests
{
    [CollectionDefinition("LogCall collection")]
    public class LogCallAspectFixtureCollection : ICollectionFixture<LogCallAspectFixture>
    { }
}