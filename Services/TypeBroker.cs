namespace Platform.Services
{
    public static class TypeBroker
    {
        private static IResponseFormatter responseFormatter = new TextResponseFormatter();
        public static IResponseFormatter Formatter => responseFormatter;
    }
}
