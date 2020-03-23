using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AsicServer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsicServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public ExtensionSettings extensionSettings { get; }

        public ClaimsPrincipal currentUser => extensionSettings.httpContextAccessor.HttpContext.User;

        public string CurrentUserId => currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

        public string CurrentUsername => currentUser.FindFirstValue(ClaimTypes.Name);


        public BaseController(ExtensionSettings extensionSettings)
        {
            this.extensionSettings = extensionSettings;
            string name = currentUser.Identity.Name;
            List<Claim> claims = currentUser.Claims.ToList();
            Claim claim = currentUser.Claims.FirstOrDefault(c => c.Value == ClaimTypes.NameIdentifier);
        }

        protected BaseResponse<T> ExecuteInMonitoring<T>(Func<T> function)
        {
            try
            {
                dynamic result = function();
                return BaseResponse<T>.GetSuccessResponse(result);
            }
            catch (BaseException ex)
            {
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.Message } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
            catch (Exception ex)
            {
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.ToString() } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
        }

        protected async Task<BaseResponse<T>> ExecuteInMonitoring<T>(Func<Task<T>> function)
        {
            dynamic result = null;
            try
            {
                result = await function();
            }
            catch (BaseException ex)
            {
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.Message } }
                };
                return BaseResponse<T>.GetErrorResponse(err, ex.StatusCode, result);
            }
            catch (Exception ex)
            {
                var err = new Dictionary<string, IEnumerable<string>>
                {
                    { "General", new List<string> { ex.ToString() } }
                };
                return BaseResponse<T>.GetErrorResponse(err);
            }
            return BaseResponse<T>.GetSuccessResponse(result);
        }

    }
}