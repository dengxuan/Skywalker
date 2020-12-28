namespace Skywalker
{
    public class UserFriendlyException : SkywalkerException, IHasErrorCode
    {
        public int Code { get; set; }

        public UserFriendlyException() { }

        public UserFriendlyException(int code)
        {
            Code = code;
        }
    }
}
