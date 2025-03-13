using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public class NotFoundException() : AppException<NotFoundException>(string.Empty) { };