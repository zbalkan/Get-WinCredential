# Get-WinCredential

Gets a credential object based on a user name and password. It uses Windows native dialogs even on PowerShell 7.x, instead of terminal.

This cmdlet aims to be a drop-in alternative to `Get-Credential`. Therefore, parameters and output is exactly the same.
Except for the new `UseModernDialog` switch which can invoke Vista+ credential dialog.

The help documentation is in `Get-WinCredential.md` file. Refer to `Get-Credential` documentation for advanced usages.

## Usage

```powershell
    # Install module from PowerShell Gallery
    Install-Module Get-WinCredential
```

```powershell
    # Use legacy credential dialog, the same with Get-Credential
    $creds = Get-WinCredential
```

![Legacy dialog](/assets/legacy.png)

```powershell
    # Use modern creential dialog, came after Vista+
    $creds = Get-WinCredential -UseModernDialog
```

![Modern dialog](/assets/modern.png)

## Acknowledgement

The idea of this POC stemmed from [a StackOverflow question](https://stackoverflow.com/q/70570097/5910839) by [BubblesTheTurtle](https://stackoverflow.com/users/6211486/bubblestheturtle)

The code is based on the [Credential Management API examples by Alan Dean](https://www.developerfusion.com/code/4693/using-the-credential-management-api/).
