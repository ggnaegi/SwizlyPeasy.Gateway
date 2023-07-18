using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Test.UnitTest.Exceptions;

public class Rfc7807ExceptionTest
{
    [Theory]
    [ClassData(typeof(ExceptionTestData))]
    public void Rfc7807_WhenPassingException_ReturnsStatusCorrespondingToException(DomainException exception,
        int status)
    {
        var result = Rfc7807.ExceptionFactory(exception, "http://localhost:8080/test");

        Assert.Equal(status, result.Status);
        Assert.Equal(exception.Message, result.Detail);
    }
}