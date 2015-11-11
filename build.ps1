Param(
    [string]$taskName='',
    [Parameter(Mandatory=$true)][string]$path
)

Import-Module .\tools\PSake\psake.psm1
Invoke-psake .\default.ps1 $taskName -parameters @{'path' = $path}