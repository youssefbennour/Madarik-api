using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public class BadRequestException(string message) : AppException<BadRequestException>(message);