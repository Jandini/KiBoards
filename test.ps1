$env:KIB_ELASTICSEARCH_HOST='http://127.0.0.1:9200'
dotnet build -c:Release src\KiBoards.sln
dotnet test -c:Release --no-build src\TestFramework 
dotnet test -c:Release --no-build src\TestObject
dotnet test -c:Release --no-build src\TestStartup
 