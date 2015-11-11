$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = "AzureStorageEmulator.exe"
$pinfo.Arguments = "status"
$pinfo.UseShellExecute = $false
$pinfo.CreateNoWindow = $true
$pinfo.RedirectStandardOutput = $truecd
$pinfo.RedirectStandardError = $true

$process = New-Object System.Diagnostics.Process
$process.StartInfo = $pinfo

# Start the process
$process.Start() | Out-Null

# Wait a while for the process to do something
sleep -Seconds 2

# If the process is still active kill it
if (!$process.HasExited) {
    $process.Kill()
}

# get output from stdout and stderr
$stdout = $process.StandardOutput.ReadToEnd()
$stderr = $process.StandardError.ReadToEnd()

# check output for success information, you may want to check stderr if stdout if empty
if ($stdout.Contains("IsRunning: False")) {
    # Start emulator
    write-output "starting emulator"
}