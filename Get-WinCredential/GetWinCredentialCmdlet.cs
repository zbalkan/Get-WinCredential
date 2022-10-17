using System;
using System.Management.Automation;
using System.Reflection.Metadata;

namespace PowerShellModuleDemo
{
    /// <summary>
    /// <para type="synopsis">Gets a credential object based on a user name and password.
    /// It uses Windows native dialogs even on PowerShell 7.x, instead of terminal.</para>
    /// <para type="description">The `Get-WinCredential` cmdlet creates a credential object for a specified user name and password.
    /// You can use the credential object in security operations.</para>
    /// <para type="description">Beginning in PowerShell 3.0, you can use the Message parameter to specify a customized message
    /// on the dialog box that prompts the user for their name and password.</para>
    /// <para type="description">The `Get-WinCredential` cmdlet prompts the user for a password or a user name and password.
    /// By default, an authentication dialog box appears to prompt the user. However, in some host programs, such as
    /// the PowerShell console, you can prompt the user at the command line by changing a registry entry.
    /// For more information about this registry entry, see the notes and examples.</para>
    /// <para type="description">This cmdlet tries to be a drop-in alternative to `Get-Credential`. Therefore, parameters and output is exactly the same.
    /// Except for the new `UseModernDialog` switch which can invoke Vista+ credential dialog.</para>
    /// <para type="notes">You can use the PSCredential object that `Get-Credential` creates in cmdlets that request user authentication, such as those with a
    /// Credential parameter.
    ///
    /// By default, the authentication prompt appears in a dialog box.To display the authentication prompt at the command line, add the
    ///
    ///    ConsolePrompting registry entry (`HKLM:\SOFTWARE\Microsoft\PowerShell\1\ShellIds\ConsolePrompting`) and set its value to True.If the
    /// ConsolePrompting registry entry does not exist or if its value is False, the authentication prompt appears in a dialog box.For
    /// instructions, see the examples.
    /// The ConsolePrompting registry entry works in the PowerShell console, but it does not work in all host programs.
    ///
    /// For example, it has no effect in the PowerShell Integrated Scripting Environment (ISE). For information about the effect of the
    /// ConsolePrompting registry entry, see the help topics for the host program.
    ///
    /// The Credential parameter is not supported by all providers that are installed with PowerShell. Beginning in PowerShell 3.0, it is
    ///
    /// supported on select cmdlets, such as the `Get-Content` and `New-PSDrive` cmdlets.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "WinCredential", DefaultParameterSetName = GetWinCredentialCmdlet.credentialSet, HelpUri = "https://go.microsoft.com/fwlink/?LinkID=2096824")]
    [OutputType(typeof(PSCredential), ParameterSetName = new string[] { GetWinCredentialCmdlet.credentialSet, GetWinCredentialCmdlet.messageSet })]

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
        [ValidateNotNullOrEmpty]
        public string Message { get; set; }

        /// <summary>
        /// Gets and sets the user supplied username to be used while creating the PSCredential.
        /// </summary>
        [Parameter(Position = 0, Mandatory = false, ParameterSetName = messageSet)]
        [ValidateNotNullOrEmpty()]
        public string UserName { get; set; }

        [Parameter(ParameterSetName = "MessageSet")]
        [Parameter(ParameterSetName = "CredentialSet")]
        public bool UseModernDialog { get; set; }

        /// <summary>
        /// The Credential parameter set name.
        /// </summary>
        private const string credentialSet = "CredentialSet";

        /// <summary>
        /// The Message parameter set name.
        /// </summary>
        private const string messageSet = "MessageSet";

        /// <summary>
        /// Initializes a new instance of the GetWinCredentialCmdlet
        /// class.
        /// </summary>
        public GetWinCredentialCmdlet() : base()
        {
        }

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
                // Original code:
                // Credential = this.Host.UI.PromptForCredential(_title, _message, _userName, string.Empty);

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
