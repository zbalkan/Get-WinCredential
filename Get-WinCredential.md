---
Module Name: Get-WinCredential
Module Guid: bde7dbcf-3866-4a8c-bb88-2c259bbb73ac
Download Help Link:
  {
    {
      https://raw.githubusercontent.com/zbalkan/Get-WinCredential/master/Get-WinCredential.md,
    },
  }
Help Version: { { 1.0.1.0 } }
Locale: en-US
---

# Get-WinCredential Module

## Synopsis

{{ Gets a credential object based on a user name and password. It uses Windows native dialogs even on PowerShell 7.x, instead of terminal.}}

## Description

{{
The `Get-WinCredential` cmdlet creates a credential object for a specified user name and password. You can use the
credential object in security operations.

Beginning in PowerShell 3.0, you can use the Message parameter to specify a customized message on the dialog box
that prompts the user for their name and password.

The `Get-WinCredential` cmdlet prompts the user for a password or a user name and password. By default, an
authentication dialog box appears to prompt the user. However, in some host programs, such as the PowerShell
console, you can prompt the user at the command line by changing a registry entry. For more information about this
registry entry, see the notes and examples.

This cmdlet aims to be a drop-in alternative to `Get-Credential`. Therefore, parameters and output is exactly the same.
Except for the new `UseModernDialog` switch which can invoke Vista+ credential dialog.

Refer to `Get-Credential` documentation for advanced usages.

The code is based on the [Credential Management API examples by Alan Dean](https://www.developerfusion.com/code/4693/using-the-credential-management-api/).
}}

## Example

```powershell
    $creds = Get-WinCredential
```

This command gets a credential object and saves it in the `$creds` variable.

## Example

```powershell
    Get-WinCredential -Message "Type your credentials" -Username "test"
```

## Example

```powershell
    Get-WinCredential -Message "Type your credentials" -UseModernDialog
```

Unlike `Get-Credential`, you can trigger the modern credentials dialog.

## Example

```powershell
    $creds | Get-WinCredential
```

Exports $creds as is.
