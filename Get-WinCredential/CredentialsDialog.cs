using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using GetWinCredential.Pinvoke;

namespace GetWinCredential
{
    /// <summary>
    ///     Encapsulates dialog functionality from the Credential Management API.
    /// </summary>
    public sealed class CredentialsDialog
    {
        /// <summary>
        ///     Gets or sets if the dialog will be shown even if the credentials can be returned
        ///     from an existing credential in the credential manager.
        /// </summary>
        private readonly bool _alwaysDisplay;

        /// <summary>
        ///     Gets or sets if the dialog is populated with username/password only.
        /// </summary>
        private readonly bool _excludeCertificates;

        /// <summary>
        ///     Gets or sets if the username is read-only.
        /// </summary>
        private readonly bool _keepName;

        /// <summary>
        ///     Gets or sets if the credentials are to be persisted in the credential manager.
        /// </summary>
        private readonly bool _persist;

        /// <summary>
        ///     Gets or sets if the save checkbox is displayed.
        /// </summary>
        /// <remarks> This value only has effect if _persist is true. </remarks>
        private readonly bool _saveDisplayed;

        /// <summary>
        ///     Gets or sets the username of the target for the credentials, typically a server username.
        /// </summary>
        private readonly string _target;

        /// <summary>
        ///     Gets or sets if modern dialog is used or not.
        /// </summary>
        private readonly bool _useModernUI;

        private string _captionValue;

        private string _messageValue;

        private string _name = string.Empty;

        private SecureString _password = null;

        /// <summary>
        ///     Gets or sets if the save checkbox status.
        /// </summary>
        private bool _saveChecked;

        /// <summary>
        ///     Gets or sets the password for the credentials.
        /// </summary>
        public SecureString Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > CREDUI.MAX_PASSWORD_LENGTH)
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "The password has a maximum length of {0} characters.",
                            CREDUI.MAX_PASSWORD_LENGTH);
                        throw new ArgumentException(message, "Password");
                    }
                }
                // Convert to secure string here
                _password = value;
            }
        }

        /// <summary>
        ///     Gets or sets the username for the credentials.
        /// </summary>
        public string UserName
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > CREDUI.MAX_USERNAME_LENGTH)
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "The username has a maximum length of {0} characters.",
                            CREDUI.MAX_USERNAME_LENGTH);
                        throw new ArgumentException(message, "UserName");
                    }
                }
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the caption of the dialog.
        /// </summary>
        /// <remarks> A null value will cause a system default caption to be used. </remarks>
        private string Message
        {
            get
            {
                return _messageValue;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > CREDUI.MAX_MESSAGE_LENGTH)
                    {
                        var message = string.Format(
                            CultureInfo.InvariantCulture,
                            "The caption has a maximum length of {0} characters.",
                            CREDUI.MAX_MESSAGE_LENGTH);
                        throw new ArgumentException(message, "Message");
                    }
                }
                _messageValue = value;
            }
        }

        /// <summary>
        ///     Gets or sets the caption of the dialog.
        /// </summary>
        /// <remarks> A null value will cause a default caption to be used. </remarks>
        private string Caption
        {
            get
            {
                return _captionValue;
            }
            set
            {
                if (value != null)
                {
                    if (value.Length > CREDUI.MAX_CAPTION_LENGTH)
                    {
                        var caption = string.Format(
                            CultureInfo.InvariantCulture,
                            "The caption has a maximum length of {0} characters.",
                            CREDUI.MAX_CAPTION_LENGTH);
                        throw new ArgumentException(caption, "Caption");
                    }
                }
                _captionValue = value;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GetWinCredential.CredentialsDialog"
        ///     /> class with the specified caption.
        /// </summary>
        /// <param name="message">
        ///     The caption of the dialog (null will cause a system default caption to be used).
        /// </param>
        /// <param name="useModernUI"> Use Vista+ dialog </param>
        public CredentialsDialog(string caption="", string message = "", bool useModernUI = false)
        {
            _target = "PowerShell";

            if (string.IsNullOrEmpty(caption))
            {
                Caption = "Credentials";
            }
            else
            {
                Caption = caption;
            }

            if (string.IsNullOrEmpty(message))
            {
                Message = "Enter your credentials.";
            }
            else
            {
                Message = message;
            }
            _useModernUI = useModernUI;
            // Keep the default values
            _alwaysDisplay = true;
            _excludeCertificates = false;
            _persist = false;
            _keepName = false;
            _saveChecked = false;
            _saveDisplayed = false;
        }
        /// <summary>
        ///     Shows the credentials dialog with the specified owner, username, password and save
        ///     checkbox status.
        /// </summary>
        /// <param name="username"> The username for the credentials. </param>
        /// <returns> Returns a DialogResult indicating the user action. </returns>
        public DialogResult Show(string username = "")
        {
            if (string.IsNullOrEmpty(username))
            {
                username = "";
            }
            UserName = username;
            _saveChecked = false;

            // Get the owner
            var owner = new NativeWindow();
            owner.AssignHandle(Process.GetCurrentProcess().MainWindowHandle);

            return ShowDialog(owner);
        }

        private static SecureString ConvertToSecureString(string value)
        {
            var secureString = new SecureString();
            foreach (var c in value)
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }
        /// <summary>
        ///     Returns a DialogResult from the specified code.
        /// </summary>
        /// <param name="code"> The credential return code. </param>
        private static DialogResult GetDialogResult(CREDUI.ReturnCodes code)
        {
            switch (code)
            {
                case CREDUI.ReturnCodes.NO_ERROR:
                    return DialogResult.OK;

                case CREDUI.ReturnCodes.ERROR_CANCELLED:
                    return DialogResult.Cancel;

                case CREDUI.ReturnCodes.ERROR_NO_SUCH_LOGON_SESSION:
                    throw new ApplicationException("No such logon session.");
                case CREDUI.ReturnCodes.ERROR_NOT_FOUND:
                    throw new ApplicationException("Not found.");
                case CREDUI.ReturnCodes.ERROR_INVALID_ACCOUNT_NAME:
                    throw new ApplicationException("Invalid account username.");
                case CREDUI.ReturnCodes.ERROR_INSUFFICIENT_BUFFER:
                    throw new ApplicationException("Insufficient buffer.");
                case CREDUI.ReturnCodes.ERROR_INVALID_PARAMETER:
                    throw new ApplicationException("Invalid parameter.");
                case CREDUI.ReturnCodes.ERROR_INVALID_FLAGS:
                    throw new ApplicationException("Invalid flags.");
                default:
                    throw new ApplicationException("Unknown credential result encountered.");
            }
        }

        /// <summary>
        ///     Returns a DialogResult from the specified code.
        /// </summary>
        /// <param name="code"> The credential return code. </param>
        private static DialogResult GetDialogResultModernUI(CREDUI.ReturnCodesModernUI code)
        {
            switch (code)
            {
                case CREDUI.ReturnCodesModernUI.NO_ERROR:
                    return DialogResult.OK;

                case CREDUI.ReturnCodesModernUI.ERROR_CANCELLED:
                    return DialogResult.Cancel;

                case CREDUI.ReturnCodesModernUI.ERROR_NO_SUCH_LOGON_SESSION:
                    throw new ApplicationException("No such logon session.");
                case CREDUI.ReturnCodesModernUI.ERROR_NOT_FOUND:
                    throw new ApplicationException("Not found.");
                case CREDUI.ReturnCodesModernUI.ERROR_INVALID_ACCOUNT_NAME:
                    throw new ApplicationException("Invalid account username.");
                case CREDUI.ReturnCodesModernUI.ERROR_INSUFFICIENT_BUFFER:
                    throw new ApplicationException("Insufficient buffer.");
                case CREDUI.ReturnCodesModernUI.ERROR_INVALID_PARAMETER:
                    throw new ApplicationException("Invalid parameter.");
                case CREDUI.ReturnCodesModernUI.ERROR_INVALID_FLAGS:
                    throw new ApplicationException("Invalid flags.");
                default:
                    throw new ApplicationException("Unknown credential result encountered.");
            }
        }

        /// <summary>
        ///     Returns the flags for dialog display options.
        /// </summary>
        private CREDUI.FLAGS GetFlags()
        {
            var flags = CREDUI.FLAGS.GENERIC_CREDENTIALS;
            // grrrr... can't seem to get this to work... if (incorrectPassword) flags = flags | CredUI.CREDUI_FLAGS.INCORRECT_PASSWORD;
            if (_alwaysDisplay) flags |= CREDUI.FLAGS.ALWAYS_SHOW_UI;
            if (_excludeCertificates) flags |= CREDUI.FLAGS.EXCLUDE_CERTIFICATES;
            if (_persist)
            {
                flags |= CREDUI.FLAGS.EXPECT_CONFIRMATION;
                if (_saveDisplayed) flags |= CREDUI.FLAGS.SHOW_SAVE_CHECK_BOX;
            }
            else
            {
                flags |= CREDUI.FLAGS.DO_NOT_PERSIST;
            }
            if (_keepName) flags |= CREDUI.FLAGS.KEEP_USERNAME;
            return flags;
        }

        /// <summary>
        ///     Returns the flags for modern dialog display options.
        /// </summary>
        private CREDUI.FLAGS_MODERN_UI GetFlagsModernUI()
        {
            // It is possible to improve using the flags but for most use cases, using
            // GENERIC is more than enough. But we need to enumerate the domain, etc.
            return CREDUI.FLAGS_MODERN_UI.CREDUIWIN_AUTHPACKAGE_ONLY;
        }

        /// <summary>
        ///     Returns the info structure for dialog display settings.
        /// </summary>
        /// <param name="owner">
        ///     The System.Windows.Forms.IWin32Window the dialog will display in front of.
        /// </param>
        private CREDUI.INFO GetInfo(IWin32Window owner)
        {
            var info = new CREDUI.INFO();
            if (owner != null) info.hwndParent = owner.Handle;
            info.pszCaptionText = Caption;
            info.pszMessageText = Message;
            info.cbSize = Marshal.SizeOf(info);
            return info;
        }

        private void SetCredentials(StringBuilder n, StringBuilder pw, int save)
        {
            UserName = n.ToString();
            Password = ConvertToSecureString(pw.ToString());
            _saveChecked = Convert.ToBoolean(save);
        }

        private void SetCredentialsModern(StringBuilder n, StringBuilder pw)
        {
            UserName = n.ToString();
            Password = ConvertToSecureString(pw.ToString());
        }

        /// <summary>
        ///     Returns a DialogResult indicating the user action.
        /// </summary>
        /// <param name="owner">
        ///     The System.Windows.Forms.IWin32Window the dialog will display in front of.
        /// </param>
        /// <remarks>
        ///     Sets the username, password and SaveChecked accessors to the state of the dialog as
        ///     it was dismissed by the user.
        /// </remarks>
        private DialogResult ShowDialog(IWin32Window owner)
        {
            // set the API call parameters
            var name = new StringBuilder(CREDUI.MAX_USERNAME_LENGTH);
            name.Append(UserName);
            var password = new StringBuilder(CREDUI.MAX_PASSWORD_LENGTH);
            var info = GetInfo(owner);
            // make the API call
            if (_useModernUI)
            {
                uint authPackage = 0;
                var flags = GetFlagsModernUI();
                var code = CREDUI.CredUIPromptForWindowsCredentials(ref info,
                    0,
                    ref authPackage,
                    IntPtr.Zero,
                    0,
                    out var outCredBuffer,
                    out var outCredSize,
                    ref _saveChecked,
                    flags);

                if (code == CREDUI.ReturnCodesModernUI.NO_ERROR)
                {
                    var domainBuf = new StringBuilder(100);
                    var maxUserName = CREDUI.MAX_USERNAME_LENGTH;
                    var maxDomain = CREDUI.MAX_DOMAIN_TARGET_LENGTH;
                    var maxPassword = CREDUI.MAX_PASSWORD_LENGTH;
                    if (CREDUI.CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, name, ref maxUserName,
                            domainBuf, ref maxDomain, password, ref maxPassword))
                    {
                        //clear the memory allocated by CredUIPromptForWindowsCredentials
                        CREDUI.CoTaskMemFree(outCredBuffer);
                        SetCredentialsModern(name, password);
                    }
                }
                return GetDialogResultModernUI(code);
            }
            else
            {
                var flags = GetFlags();
                var saveChecked = Convert.ToInt32(_saveChecked);

                var code = CREDUI.PromptForCredentials(
                    ref info,
                    _target,
                    IntPtr.Zero,
                    0,
                    name,
                    CREDUI.MAX_USERNAME_LENGTH,
                    password,
                    CREDUI.MAX_PASSWORD_LENGTH,
                    ref saveChecked,
                    flags
                );
                // set the accessors from the API call parameters
                SetCredentials(name, password, saveChecked);
                return GetDialogResult(code);
            }
        }
    }
}