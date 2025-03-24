namespace MPCalcHub.Domain.Constants;

public class AppConstants
{
    public static class Policies
    {
        public const string SuperUser = "Admin";
        public const string User = "User";
        public const string SuperOrModerator = "SuperOrModerator";
        public const string SuperOrGuest = "SuperOrGuest";
        public const string Guest = "Guest";
        public const string Banned = "Banned";
    }

    public static class Routes
    {
        public static class RabbitMQ
        {
            public const string ContactInsert = "contact.insert";
            public const string ContactUpdate = "contact.update";
        }
    }
}