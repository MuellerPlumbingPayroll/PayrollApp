using System.Threading.Tasks;
using Timecard.Models;
using Timecard.Services;

namespace Timecard.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        public string FirebaseNotAuthorizedErrorMessage { get; }
        private readonly GoogleService _googleService;

        public AuthViewModel()
        {
            Title = "Sign In";
            FirebaseNotAuthorizedErrorMessage = "You are not authorized to access this application. " + 
                "If you believe this is an error, please contact your administrator. " +
            	"Once your administrator has fixed this problem, please restart the app.";

            _googleService = new GoogleService();
        }

        public async Task<FirebaseUserInfo> AuthenticateUserWithFirebaseAsync(GoogleUserInfo googleUserInfo)
        {
            return await DataStore.AuthenticateUser(googleUserInfo);
        }

        public async Task<GoogleUserInfo> RetrieveGoogleUserInfoAsync(string tokenType, string accessToken)
        {
            return await _googleService.GetUserInfoAsync(tokenType, accessToken);
        }

        public async Task<bool> RevokeTokenAsync(string tokenType, string accessToken)
        {
            return await _googleService.RevokeTokenAsync(tokenType, accessToken);
        }
    }
}
