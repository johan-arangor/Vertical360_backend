using System.Security.Claims;

namespace Vertical360_back.Services
{
    public class ServiceUser : IServiceUser
    {
        private readonly HttpContext _httContest;

        public ServiceUser(IHttpContextAccessor httpContextAccessor)
        {
            _httContest = httpContextAccessor.HttpContext!;
        }

        public string GetUserId()
        {
            if(_httContest.User.Identity!.IsAuthenticated)
            {
                var idClaim = _httContest.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                if (idClaim is null)
                {
                    throw new ApplicationException("User ID claim not found.");
                }

                return idClaim.Value;
            }
            else
            {
                throw new ApplicationException("User is not authenticated.");
            }
        }
    }
}
