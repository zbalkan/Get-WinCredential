$apiKey = Get-Content -Path.\.apikey -Raw
$module = '.\Get-WinCredential.psd1'

Publish-Module -Name $module -NuGetApiKey $apiKey