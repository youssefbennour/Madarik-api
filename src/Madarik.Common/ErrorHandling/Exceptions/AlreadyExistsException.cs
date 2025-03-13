using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public sealed class AlreadyExistsException(string message) : AppException<AlreadyExistsException>(message);