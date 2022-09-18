using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ElgatoWaveSDK.Tests;

public class TransactionTrackerTests
{
    private ITestOutputHelper _output;

    public TransactionTrackerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ValidTracker()
    {
        var subject = new TransactionTracker(10, 10);

        var value = subject.NextTransactionId();
        value.Should().Be(0100000011);
    }

    [Fact]
    public void CreateRandom()
    {
        var subject = new TransactionTracker(0, 10);

        var value = subject.NextTransactionId();
        _output.WriteLine("Value: " + value);
        value.Should().NotBe(0000000011);
    }

    [Fact]
    public void HitTransactionMax()
    {
        var subject = new TransactionTracker(0, 5752195);

        var value = subject.NextTransactionId();
        value.Should().NotBe(0110000000);
    }

    [Fact]
    public void HitBoxMax()
    {
        var subject = new TransactionTracker(122, 5752195);

        var value = subject.NextTransactionId();
        value.Should().Be(000000000);
    }
}
