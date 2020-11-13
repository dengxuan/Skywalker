using System;
using System.Net;
using System.Text;

namespace Skywalker.AspNetCore.Mvc.Models
{
    //TODO@Gordon: Localizer later
    internal class DefaultErrorConverter : IExceptionToErrorConverter
    {
        private readonly SkywalkerResponseWrapperOptions _options;

        public IExceptionToErrorConverter? Next { set; private get; }

        private bool SendAllExceptionsToClients
        {
            get
            {
                return _options.SendAllExceptionsToClients;
            }
        }

        public DefaultErrorConverter(SkywalkerResponseWrapperOptions options)
        {
            _options = options;
        }

        public Error Convert(Exception exception)
        {
            var errorInfo = CreateErrorInfoWithoutCode(exception);

            return errorInfo;
        }

        private Error CreateErrorInfoWithoutCode(Exception exception)
        {
            if (SendAllExceptionsToClients)
            {
                return CreateDetailedErrorInfoFromException(exception);
            }

            return new Error((int)HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        private Error CreateDetailedErrorInfoFromException(Exception exception)
        {
            var detailBuilder = new StringBuilder();

            AddExceptionToDetails(exception, detailBuilder);

            var errorInfo = new Error((int)HttpStatusCode.InternalServerError, exception.Message, detailBuilder.ToString());

            return errorInfo;
        }

        private void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder)
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
}
