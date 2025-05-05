@echo off
set KIB_ELASTICSEARCH_HOST=http://localhost:9200
set KIB_KIBANA_HOST=http://localhost:5601
set KIB_DEFAULT_ROUTE=/app/dashboards#/view/3fd54b90-7a8b-11ee-84d9-b3a2378dedfb
set KIB_DARK_MODE=1
set KIB_B64_HELLO_WORLD=SGVsbG8KV29ybGQ=
set KIB_SPACE_INITIALS=IT
set KIB_SPACE_NAME="Integration Tests"
set KIB_SPACE_DESCRIPTION="Integration test dashboards"
dotnet build -c:Release src\KiBoards.sln
dotnet test --logger "console;verbosity=normal" -c:Release --no-build src\TestFramework
dotnet test --logger "console;verbosity=normal" -c:Release --no-build src\TestObject
dotnet test --logger "console;verbosity=normal" -c:Release --no-build src\TestStartup


