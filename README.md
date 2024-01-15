# Get-WinCredential

Gets a credential object based on a user name and password. It uses Windows native dialogs even on PowerShell 7.x, instead of terminal.

This cmdlet aims to be a drop-in alternative to `Get-Credential`. Therefore, output is exactly the same. New parameters are included as features.
For instance, the new `UseModernDialog` switch added which can invoke Vista+ credential dialog. However, you cannot pass `Username` parameter as 
CREDUI API does not allow it. Another feature is the `Title` parameter that enables the user to update the captin. It can be helpful with 
password management tools like KeePass which matches window title to the password.

The help documentation is in `Get-WinCredential.md` file. Refer to `Get-Credential` documentation for advanced usages.

## Usage

```powershell
    # Install module from PowerShell Gallery
    # Package URL: https://www.powershellgallery.com/packages/Get-WinCredential
    Install-Module Get-WinCredential
```

```powershell
    # Use legacy credential dialog, the same with Get-Credential
    $creds = Get-WinCredential
```

![Legacy dialog](/assets/legacy.png)

```powershell
    # Use modern credential dialog, came after Vista+ without a Title defined 
    $creds = Get-WinCredential -UseModernDialog
```

![Modern dialog](/assets/modern.png)

## Acknowledgement

The idea of this POC stemmed from [a StackOverflow question](https://stackoverflow.com/q/70570097/5910839) by [BubblesTheTurtle](https://stackoverflow.com/users/6211486/bubblestheturtle)

The code is based on the [Credential Management API examples by Alan Dean](https://www.developerfusion.com/code/4693/using-the-credential-management-api/).
