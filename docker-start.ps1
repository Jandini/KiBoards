# Start docker desktop 

$desktop = "${env:ProgramFiles}\Docker\Docker\Docker Desktop.exe"
$docker = "${env:ProgramFiles}\Docker\Docker\resources\bin\docker.exe"

if ((Test-Path $desktop -PathType Leaf) -and (Test-Path $docker -PathType Leaf) -and (Get-Service -Name com.docker.service -ErrorAction SilentlyContinue)) {

    & $docker version *> $null 

    if ($LASTEXITCODE -ne 0) {

        $service = { return (Get-Service -Name com.docker.service -ErrorAction SilentlyContinue).Status }
        $status = & $service

        if ($status -ne "Running") {
            
            Write-Warning "Docker service is not running."
            Write-Host "Starting com.docker.service..."
            Start-Process "sc.exe" -ArgumentList "start com.docker.service" -Verb RunAs -WorkingDirectory $env:windir -Wait -WindowStyle Hidden
            
            Write-Host -NoNewline "Waiting docker service..."
            while ($status -ne "Running") {
                Write-Host -NoNewline .
                Start-Sleep 2
                $status = & $service
            }
            Write-Host .
        }

        if (!(Get-Process 'com.docker.proxy' -ErrorAction SilentlyContinue)) {
            Write-Warning "Docker desktop is not running."

            # Disable "Open Docker Dashboard at startup"
            $settingsJson = "$env:APPDATA\Docker\settings.json"
            $settings = Get-Content $settingsJson | ConvertFrom-Json 
            
            if ($settings.openUIOnStartupDisabled -eq $false) {
                Write-Host "Disabling `"Open Docker Dashboard at startup`" option..."
                if (!(Test-Path "$settingsJson.bak" -PathType Leaf)) {
                    # Backup settings file only once
                    Copy-Item $settingsJson "$settingsJson.bak"
                }
                $settings.openUIOnStartupDisabled = $true
                $settings | ConvertTo-Json | Set-Content $settingsJson
            }
                        
            Write-Host "Starting docker desktop..."        
            & $desktop -Autostart
            
            Write-Host -NoNewline "Waiting for com.docker.proxy..."
            while (!(Get-Process 'com.docker.proxy' -ErrorAction SilentlyContinue)) {
                Write-Host -NoNewline .
                Start-Sleep 1
            }

            Write-Host .
            Write-Host -NoNewline "Waiting for docker..."

            $start = { return Start-Process $docker -WindowStyle Hidden -ArgumentList "container ls" -PassThru -Wait }
            $process = & $start

            while ($process.ExitCode -ne 0) {
                Write-Host -NoNewline .
                Start-Sleep 2
                $process = & $start
            }

            Write-Host .
            Exit 0
        }        
    }
}
else {
    Write-Warning "Docker desktop is not installed."
    Write-Host "Run 'choco install docker-desktop' or download docker desktop from https://www.docker.com/products/docker-desktop/"    
    Exit 1
}

$env:DOCKER_SCAN_SUGGEST="false"
