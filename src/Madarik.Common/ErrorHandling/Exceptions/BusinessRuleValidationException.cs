using Madarik.Common.ErrorHandling.Exceptions.Abstractions;

namespace Madarik.Common.ErrorHandling.Exceptions;

public class BusinessRuleValidationException(string message = "One or more errors have occured")
    : AppException<BusinessRuleValidationException>(message);