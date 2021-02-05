using System;
using System.Threading.Tasks;

namespace MobileSecurityPOC
{
    public interface IBiometricAuthenticateService
    {
        String GetAuthenticationType();
        Task<bool> AuthenticateUserIDWithTouchID();
        bool fingerprintEnabled();
        void Authendicate();
    }
}
