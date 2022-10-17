# Get-WinCredential

Gets a credential object based on a user name and password. It uses Windows native dialogs even on PowerShell 7.x, instead of terminal.

This cmdlet aims to be a drop-in alternative to `Get-Credential`. Therefore, parameters and output is exactly the same.
Except for the new `UseModernDialog` switch which can invoke Vista+ credential dialog.

The help documentation is in `Get-WinCredential.md` file. Refer to `Get-Credential` documentation for advanced usages.

## Usage

```
	//TODO
```

## Acknowledgement

The code is based on the [Credential Management API examples by Alan Dean](https://www.developerfusion.com/code/4693/using-the-credential-management-api/).