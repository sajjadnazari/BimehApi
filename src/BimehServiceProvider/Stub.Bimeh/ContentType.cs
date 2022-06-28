namespace Stub.Bimeh
{
    public class ContentType
    {
        private readonly HttpContentType _httpContentType;

        public ContentType(HttpContentType httpContentType)
        {
            _httpContentType = httpContentType;
        }

        public override string ToString()
        {
            return "application/json";
        }
    }
}
