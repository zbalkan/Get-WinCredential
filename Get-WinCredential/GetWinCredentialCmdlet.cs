using System;
using System.Management.Automation;
using System.Reflection.Metadata;
using System.Windows.Forms;

namespace GetWinCredential
{
    [Cmdlet(VerbsCommon.Get, "WinCredential", DefaultParameterSetName = credentialSet, HelpUri = "https://github.com/zbalkan/Get-WinCredential/blob/master/Get-WinCredential.md")]
    [OutputType(typeof(PSCredential), ParameterSetName = new string[] { credentialSet, messageSet })]

    public class GetWinCredentialCmdlet : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the underlying PSCredential of
        /// the instance.
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = credentialSet)]
        [ValidateNotNull]
        [Credential()]
        public PSCredential Credential { get; set; }

        /// <summary>
        /// Gets and sets the user supplied message providing description about which script/function is
        /// requesting the PSCredential from the user.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = messageSet)]
        [Parameter(Mandatory = false, ParameterSetName = messageSetModern)]
        [ValidateNotNullOrEmpty]
        public string Message { get; set; }

        /// <summary>
        /// Gets and sets the user supplied title providing description about which script/function is
        /// requesting the PSCredential from the user.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = messageSet)]
        [Parameter(Mandatory = false, ParameterSetName = messageSetModern)]
        [ValidateNotNullOrEmpty]
        public string Title { get; set; }

        /// <summary>
        /// Gets and sets the user supplied username to be used while creating the PSCredential.
        /// </summary>
        [Parameter(Position = 0, Mandatory = false, ParameterSetName = messageSet)]
        [ValidateNotNullOrEmpty()]
        public string UserName { get; set; }

        [Parameter(ParameterSetName = messageSetModern)]
        public SwitchParameter UseModernDialog
        {
            get { return _useModernDialog; }
            set { _useModernDialog = value; }
        }

        private bool _useModernDialog;

        /// <summary>
        /// The Credential parameter set name.
        /// </summary>
        private const string credentialSet = "CredentialSet";

        /// <summary>
        /// The Message parameter set name.
        /// </summary>
        private const string messageSet = "MessageSet";

        /// <summary>
        /// The Modern Message parameter set name.
        /// </summary>
        private const string messageSetModern = "MessageSetModern";

        /// <summary>
        /// The command outputs the stored PSCredential.
        /// </summary>
        protected override void BeginProcessing()
        {
            if (Credential != null)
            {
                WriteObject(Credential);
                return;
            }

            try
            {
                var dialog = new CredentialsDialog(caption: Title, message: Message, useModernUI: _useModernDialog);
                var dialogResult = dialog.Show(UserName);
                if (dialogResult == DialogResult.OK)
                {
                    Credential = new PSCredential(dialog.UserName, dialog.Password);
                }
            }
            catch (ArgumentException exception)
            {
                ErrorRecord errorRecord = new(
                    exception,
                    "CouldNotPromptForCredential",
                    ErrorCategory.InvalidOperation,
                    targetObject: null);
                WriteError(errorRecord);
            }

            if (Credential != null)
            {
                WriteObject(Credential);
            }
        }
    }
}
