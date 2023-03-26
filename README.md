# KiBoards

[![Docker Image CI](https://github.com/Jandini/KiBoards/actions/workflows/docker-image.yml/badge.svg)](https://github.com/Jandini/KiBoards/actions/workflows/docker-image.yml)

Elasticsearch logging and dashboards service for Kibana 8.6.2


## Quick Start

Publish KiBoards and build docker image with `docker-build.ps1` script or use following commands.

```sh
dotnet publish -nologo --configuration Release --runtime linux-x64 --no-self-contained src
docker build -t jandini/kiboards:latest .
```

Run KiBoards, Elasticsearch and Kibana with docker compose command.
```sh
docker compose up
```

You can run KiBoards service separately and keep Elasticsearch and Kibana running in the background. 
Start Elasticsearch and Kibana with `kibana-start.ps1` script or run following command.

```sh
docker compose -f kibana-compose.yaml up --force-recreate
```

Run KiBoards with `docker-run.ps1` script or use following command.
```sh
docker run --network kiboards-network -p 8089:80 -it -e KIBANA__URI=http://kiboards-kibana:5601 -e ELASTICSEARCH_URI=http://kiboards-elastic:9200 -e ASPNETCORE_ENVIRONMENT=Development --rm jandini/kiboards:latest
```



## Scripts and configuration files

The repository consist of number of files 

| File Name             | Description                                                  |
| --------------------- | ------------------------------------------------------------ |
| `docker-build.ps1`    | Publish application and build docker image.                  |
| `docker-compose.yaml` | Docker compose file for Kibana 8.6.2, Elasticsearch 8.6.2 and latest KiBoards service. |
| `docker-run.ps1`      | Run KiBoards docker container.                               |
| `docker-start.ps1`    | Starts Docker Desktop services if not already running.       |
| `Dockerfile`          | Docker file to build docker image.                           |
| `GitVersion.yml`      | GitVersion configuration file.                               |
| `kibana-compose.yaml` | Docker compose file for Kibana 8.6.2, Elasticsearch 8.6.2 only. |
| `kibana-start.ps1`    | Starts Elasticsearch and Kibana in minimized window using `kibana-compose.yaml` file. |



## Troubleshooting

#### Docker daemon throws error 

Docker Desktop returns `open //./pipe/docker_engine: The system cannot find the file specified.` error or running `docker` command results with:

```
error during connect: In the default daemon configuration on Windows, the docker client must be run with elevated privileges to connect.: Post "http://%2F%2F.%2Fpipe%2Fdocker_engine/v1.24/build": open //./pipe/docker_engine: The system cannot find the file specified.
```

Check if Docker Desktop is running and if your are running `docker` command with administrator privileges. 
Use `docker-start.ps1` to start docker desktop.  Ensure you have privileges to run `docker` command. 





---
Created from [JandaBox](https://github.com/Jandini/JandaBox)
