
using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public class InternalServerException() : AppException<InternalServerException>(string.Empty);
