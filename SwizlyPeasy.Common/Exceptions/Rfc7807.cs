using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Common.Exceptions
{
    public static class Rfc7807
    {
        /// <summary>
        /// This class is used as factory according to the
        /// exception class type passed as method argument.
        /// </summary>
        /// <param name="appException"></param>
        /// <param name="httpRequestPath"></param>
        /// <returns></returns>
        public static Dtos.Rfc7807 ExceptionFactory(Exception appException, string httpRequestPath)
        {
            var currentException = new Dtos.Rfc7807
            {
                Title = "Internal Server Error",
                Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
                Detail = appException.Message,
                Status = 500,
                Instance = httpRequestPath,
                Context = (appException as DomainException)?.Context
            };

            switch (appException)
            {
                case InternalDomainException:
                    currentException.Title = "Internal Domain Exception";
                    break;
                case BadRequestDomainException:
                    currentException.Title = "Bad Request Domain Exception";
                    currentException.Status = 400;
                    currentException.Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400";
                    break;
                case NotFoundDomainException:
                    currentException.Title = "Not Found Domain Exception";
                    currentException.Status = 404;
                    currentException.Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404";
                    break;
                case UnprocessableEntityDomainException:
                    currentException.Title = "Unprocessable Entity Domain Exception";
                    currentException.Status = 422;
                    currentException.Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422";
                    break;
                case ConflictDomainException:
                    currentException.Title = "Conflict Domain Exception";
                    currentException.Status = 409;
                    currentException.Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/409";
                    break;
                case UnAuthorizedDomainException:
                    currentException.Title = "Unauthorized Domain Exception";
                    currentException.Status = 401;
                    currentException.Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401";
                    break;
                case ForbiddenDomainException:
                    currentException.Title = "Forbidden Domain Exception";
                    currentException.Status = 403;
                    currentException.Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/403";
                    break;
            }

            return currentException;
        }
    }
}