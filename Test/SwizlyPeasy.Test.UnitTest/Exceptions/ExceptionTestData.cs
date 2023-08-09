using System.Collections;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Test.UnitTest.Exceptions;

public class ExceptionTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new BadRequestDomainException("test", null), 400 };
        yield return new object[] { new ConflictDomainException("test", null), 409 };
        yield return new object[] { new ForbiddenDomainException("test", null), 403 };
        yield return new object[] { new InternalDomainException("test", null), 500 };
        yield return new object[] { new NotFoundDomainException("test", null), 404 };
        yield return new object[] { new UnAuthorizedDomainException("test", null), 401 };
        yield return new object[] { new UnprocessableEntityDomainException("test", null), 422 };
        yield return new object[] { new TooManyRequestsException("test", null), 429 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}