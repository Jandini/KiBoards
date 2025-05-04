@echo off
set KIB_ELASTICSEARCH_HOST=http://localhost:9200
set KIB_KIBANA_HOST=http://localhost:5601
set KIB_DARK_MODE=1
set KIB_B64_HELLO_WORLD=SGVsbG8KV29ybGQ=
dotnet build -c:Release src\KiBoards.sln
dotnet test -c:Release --no-build src\TestFramework
dotnet test -c:Release --no-build src\TestObject
dotnet test -c:Release --no-build src\TestStartup
