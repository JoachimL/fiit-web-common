using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Fiit.Web.Common.Http.Filters
{
   public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                var errorMessage = BuildErrorMessage(modelState);
                actionContext.Response = actionContext.Request
                    .CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
            }
        }

        private string BuildErrorMessage(IEnumerable<KeyValuePair<string, ModelState>> modelState)
        {
            return string.Join(System.Environment.NewLine, modelState.SelectMany(GetErrorsFrom));
        }

        private static IEnumerable<string> GetErrorsFrom(KeyValuePair<string, ModelState> m)
        {
            return m.Value.Errors.Select(GetErrorMessageFrom);
        }

        private static string GetErrorMessageFrom(ModelError e)
        {
            return !string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.ErrorMessage : ((e.Exception != null ? e.Exception.Message : string.Empty));
        }
    }
}