properties {
	$filePath = $path
}

task default -depends Build

formatTaskName {
	param($taskName)
	write-host $taskName -foregroundcolor Green
}

task Build -depends Nuget-Restore, Clean { 
  msbuild /t:Build "src\Azure.Storage.sln"
}

task Test -depends Stop-AzureEmulator, Run-Tests, Start-AzureEmulator { 
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
  nuget restore src\Azure.Storage.sln
}

task Clean { 
  msbuild /t:Clean $filePath
}