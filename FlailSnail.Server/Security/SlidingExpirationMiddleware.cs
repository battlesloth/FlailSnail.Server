using System.IdentityModel.Tokens.Jwt;

namespace FlailSnail.Server.Security
{
    public class SlidingExpirationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<SlidingExpirationMiddleware> logger;

        public SlidingExpirationMiddleware(RequestDelegate next, ILogger<SlidingExpirationMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService jwtTokenService)
        {
            try
            {
                string authorization = context.Request.Headers["Authorization"];

                JwtSecurityToken token = null;
                if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer"))
                    token = new JwtSecurityTokenHandler()
                        .ReadJwtToken(authorization[7..]); // trim 'Bearer ' from the start

                if (token != null && token.ValidTo > DateTime.UtcNow)
                {
                    var timeElapsed = DateTime.UtcNow.Subtract(token.ValidFrom);
                    var timeRemaining = token.ValidTo.Subtract(DateTime.UtcNow);

                    if (timeRemaining < timeElapsed)
                    {
                        context.Response.Headers.Add("Set-Authorization", jwtTokenService.ReissueToken(authorization));
                    }
                }

            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }

            await next(context);
        }
    }
}