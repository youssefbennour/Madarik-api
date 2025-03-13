namespace Madarik.Common.BusinessRulesEngine;

public interface IBusinessRule
{
    bool IsMet();
    string ErrorKey { get; }
    string Error { get; }
}