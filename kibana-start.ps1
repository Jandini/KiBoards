# Ensure docker desktop is running
& .\docker-start.ps1

# Start docker compose up in separate window. Compose Elasticsearch and Kibana services only. 
Start-Process "docker" -WindowStyle Minimized -ArgumentList "compose -f kibana-compose.yaml up --force-recreate"
