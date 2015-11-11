properties {
	$filePath = $path
}

task default -depends Build

formatTaskName {
	param($taskName)
	write-host $taskName -foregroundcolor Green
}

task Build -depends Clean, Nuget-Restore { 
  msbuild /t:Build $filePath
}

task Test -depends Start-AzureEmulator, Run-Tests, Stop-AzureEmulator { 
  # no op
}

task Stop-AzureEmulator { 
  invoke-expression ".\tools\StorageEmulator\AzureStorageEmulator.exe stop"
}

task Run-Tests { 
  xunit.console.clr4 $filePath /appveyor
}

task Start-AzureEmulator { 
  invoke-expression ".\tools\StorageEmulator\AzureStorageEmulator.exe start"
  sleep -Seconds 2
  write-output "emulator started"
}

task Nuget-Restore { 
  nuget restore $filePath
}

task Clean { 
  msbuild /t:Clean $filePath
}