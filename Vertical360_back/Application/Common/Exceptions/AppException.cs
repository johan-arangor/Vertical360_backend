namespace Vertical360_back.Application.Common.Exceptions
{
    public class AppException : Exception
    {
        public string Code { get; }

        public AppException(string code, string? details = null)
            : base(details)
        {
            Code = code;
        }
    }

    public class WeakPasswordException : AppException
    {
        public WeakPasswordException(string? details = null)
            : base("USER_WEAK_PASSWORD", details)
        { }
    }

    public class CompanyCreationException : AppException
    {
        public CompanyCreationException(string? details = null)
            : base("COMPANY_CREATION_FAILED", details)
        { }
    }
}
