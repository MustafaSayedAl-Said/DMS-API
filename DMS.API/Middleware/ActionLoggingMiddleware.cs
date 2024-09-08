using DMS.Core.Entities;
using DMS.Services.Interfaces;
using System.Security.Claims;

namespace DMS.API.Middleware
{
    public class ActionLoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IRabbitMQService _rabbitMQService;


        public ActionLoggingMiddleware(RequestDelegate next,
            IRabbitMQService rabbitMQService)
        {
            _next = next;
            _rabbitMQService = rabbitMQService;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var userId = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var actionType = GetActionType(context);

            if (actionType != null && userId != null)
            {
                var logEntry = new ActionLog
                {
                    UserId = int.Parse(userId),
                    ActionType = (ActionTypeEnum) actionType,
                    CreationDate = DateTime.UtcNow,
                    UserName = email,
                    DocumentId = GetDocumentIdFromRequest(context),
                    DocumentName = GetDocumentNameFromRequest(context),
                    //Reviewers = await userService.GetAllAdminIDs()
                };

                // Send log entry to RabbitMQ
                _rabbitMQService.SendMessage(logEntry);
            }
            await _next(context);
        }

        private ActionTypeEnum? GetActionType(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            var method = context.Request.Method;

            if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) && path.Contains("/download")) return ActionTypeEnum.Download;
            if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) && path.Contains("/preview")) return ActionTypeEnum.Preview;
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) && path.Contains("/documents")) return ActionTypeEnum.Upload;

            return null;
        }

        private int? GetDocumentIdFromRequest(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            var method = context.Request.Method;
            var query = context.Request.Query;
            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) && path.Contains("/documents"))
            {
                return null;
            }
            if ((method.Equals("GET", StringComparison.OrdinalIgnoreCase) && path.Contains("/preview")) ||
                (method.Equals("GET", StringComparison.OrdinalIgnoreCase) && path.Contains("/download")))
            {
                var segments = path.Split('/');

                if (segments.Length > 1 && int.TryParse(segments.Last(), out var documentId))
                {
                    return documentId;
                }
            }
            if (context.Request.Query.ContainsKey("id"))
            {
                if (int.TryParse(context.Request.Query["id"], out var documentId))
                {
                    return documentId;
                }
            }

            return null;

        }

        private string GetDocumentNameFromRequest(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            var method = context.Request.Method;
            var query = context.Request.Query;

            if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) && path.Contains("/documents"))
            {
                if (context.Request.HasFormContentType)
                {
                    var form = context.Request.Form;

                    // Check if a file is uploaded in the form
                    if (form.Files.Count > 0)
                    {
                        var file = form.Files[0]; // Assuming the first file, but handle multiple files if needed
                        return file.FileName; // Return the file name
                    }
                }
            }

            return "";
        }
    }
}
