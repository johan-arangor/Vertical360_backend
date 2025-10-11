namespace Vertical360_back.Application.Common.Exceptions
{
    public class UserCreationException : AppException
    {
        public UserCreationException(string? details = null)
            : base("USER_CREATION_FAILED", details)
        { }
    }
}
