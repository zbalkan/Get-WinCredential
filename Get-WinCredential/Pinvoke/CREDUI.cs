using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GetWinCredential.Pinvoke
{
    internal static partial class CREDUI
    {
        /// <summary>
        ///     http://msdn.microsoft.com/library/default.asp?url=/library/en-us/secauthn/security/authentication_constants.asp
        /// </summary>
        public const int MAX_MESSAGE_LENGTH = 100;

        public const int MAX_CAPTION_LENGTH = 100;
        public const int MAX_GENERIC_TARGET_LENGTH = 100;
        public const int MAX_DOMAIN_TARGET_LENGTH = 100;
        public const int MAX_USERNAME_LENGTH = 100;
        public const int MAX_PASSWORD_LENGTH = 100;

        /// <summary>
        ///     http://www.pinvoke.net/default.aspx/credui.CredUIPromptForCredentialsW http://msdn.microsoft.com/library/default.asp?url=/library/en-us/secauthn/security/creduipromptforcredentials.asp
        /// </summary>
        [DllImport("credui", EntryPoint = "CredUIPromptForCredentialsW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern ReturnCodes PromptForCredentials(
            ref INFO creditUR,
            string targetName,
            IntPtr reserved1,
            int iError,
            StringBuilder userName,
            int maxUserName,
            StringBuilder password,
            int maxPassword,
            ref int iSave,
            FLAGS flags
        );

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ReturnCodesModernUI CredUIPromptForWindowsCredentials(ref CREDUI.INFO notUsedHere,
            int authError,
            ref uint authPackage,
            IntPtr InAuthBuffer,
            uint InAuthBufferSize,
            out IntPtr refOutAuthBuffer,
            out uint refOutAuthBufferSize,
            ref bool fSave,
            FLAGS_MODERN_UI flags);

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool CredUnPackAuthenticationBuffer(int dwFlags,
            IntPtr pAuthBuffer,
            uint cbAuthBuffer,
            StringBuilder pszUserName,
            ref int pcchMaxUserName,
            StringBuilder pszDomainName,
            ref int pcchMaxDomainame,
            StringBuilder pszPassword,
            ref int pcchMaxPassword);

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CoTaskMemFree(IntPtr ptr);
    }
}