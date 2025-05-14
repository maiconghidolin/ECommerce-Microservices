using WireMock.Server;

namespace OrderService.Application.Unit.Tests.Fixtures;

public class WireMockFixture : IDisposable
{
    public WireMockServer Server { get; }

    public WireMockFixture()
    {
        Server = WireMockServer.Start(9091);
    }

    public void Dispose()
    {
        Server.Stop();
        Server.Dispose();
    }
}