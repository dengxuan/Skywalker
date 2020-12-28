namespace System
{

    public class UserFriendlyException : Exception, IHasErrorCode
    {
        public int Code { get; set; }

        public UserFriendlyException(int code) : this(code, string.Empty)
        {
        }

        public UserFriendlyException(int code, string message) : base(message)
        {
            Code = code;
        }
    }
}
