using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public class UnauthorizedException(string message) : AppException<UnauthorizedException>(message);