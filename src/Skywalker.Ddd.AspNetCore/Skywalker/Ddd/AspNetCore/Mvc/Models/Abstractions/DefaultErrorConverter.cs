using System.Net;
using System.Text;
using Skywalker.Ddd.Exceptions;

namespace Skywalker.Ddd.AspNetCore.Mvc.Models.Abstractions;

//TODO@Gordon: Localizer later
internal class DefaultErrorConverter(ResponseWrapperOptions options) : IExceptionToErrorConverter
{
    public IExceptionToErrorConverter? Next { set; private get; }

    public Error Convert(Exception exception)
    {
        var errorInfo = CreateErrorInfoWithoutCode(exception);

        return errorInfo;
    }

    private Error CreateErrorInfoWithoutCode(Exception exception)
    {
        if (options.SendAllExceptionsToClients)
        {
            return CreateDetailedErrorInfoFromException(exception);
        }
        if (exception is UserFriendlyException friendlyException)
        {
            return new Error(friendlyException.Code ?? HttpStatusCode.InternalServerError.ToString(), friendlyException.Message);
        }
        return new Error(HttpStatusCode.InternalServerError.ToString(), "Internal Server Error");
    }

    private static Error CreateDetailedErrorInfoFromException(Exception exception)
    {
        var detailBuilder = new StringBuilder();

        AddExceptionToDetails(exception, detailBuilder);
        var code = HttpStatusCode.InternalServerError.ToString();
        if (exception is UserFriendlyException friendlyException)
        {
            code = friendlyException.Code ?? HttpStatusCode.InternalServerError.ToString();
        }
        var errorInfo = new Error(code!, exception.Message, detailBuilder.ToString());

        return errorInfo;
    }

    private static void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder)
    {
        //Exception Message
        detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

        //Exception StackTrace
        if (!string.IsNullOrEmpty(exception.StackTrace))
        {
            detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);
        }

        //Inner exception
        if (exception.InnerException != null)
        {
            AddExceptionToDetails(exception.InnerException, detailBuilder);
        }
    }
}
