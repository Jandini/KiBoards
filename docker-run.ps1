$ImageName = "jandini/kiboards"
$ImageTag = $(dotnet gitversion /showvariable SemVer); if (!$ImageTag) { $ImageName = "1.0.0" }

Start-Process "http://localhost:8089/swagger/index.html" -ErrorAction SilentlyContinue
docker run --network kiboards-network -p 8089:80 -it -e KIBANA__URI=http://kiboards-kibana:5601 -e ELASTICSEARCH_URI=http://kiboards-elastic:9200 -e ASPNETCORE_ENVIRONMENT=Development --rm ${ImageName}:${ImageTag}

 