$env:KIB_ELASTICSEARCH_HOST='http://localhost:9200'
$env:KIB_KIBANA_HOST='http://localhost:5601'
$env:KIB_DARK_MODE=1
dotnet build -c:Release src\KiBoards.sln
dotnet test -c:Release --no-build src\TestFramework --logger="console"
dotnet test -c:Release --no-build src\TestObject
dotnet test -c:Release --no-build src\TestStartup
 