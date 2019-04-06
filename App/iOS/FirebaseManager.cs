using Firebase.Auth;

namespace Timecard.iOS
{
    public class FirebaseManager
    {
        public static Auth Auth;

        public static void Configure()
        {
            Firebase.Core.App.Configure();
            Auth = Auth.DefaultInstance;
        }
    }
}
