using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public class ForbiddenException(string message) : AppException<ForbiddenException>(message);
