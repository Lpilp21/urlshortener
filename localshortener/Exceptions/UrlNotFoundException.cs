namespace localshortener.api.Exceptions
{
    public class UrlNotFoundException : BaseException
    {
        public UrlNotFoundException() : base(404, "URL_NOT_FOUND")
        {

        }
    }
}
