$ImageName = "icehub"
$ImageTag = $(dotnet gitversion /showvariable SemVer || "1.0.0")

Start-Process "http://localhost:8088/swagger/index.html" -ErrorAction SilentlyContinue
docker run --network kiboards-network -p 8088:80 -it -e ELASTICSEARCH_URI=http://kiboards-elastic:9200 -e ASPNETCORE_ENVIRONMENT=Development --rm ${ImageName}/${ImageTag}

 