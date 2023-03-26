& .\docker-start.ps1

Start-Process "docker" -WindowStyle Minimized -ArgumentList "compose up --force-recreate"


