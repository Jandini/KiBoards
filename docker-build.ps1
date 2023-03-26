$EntryPoint = "KiBoards.dll"
$PublishDir = "src/KiBoards/bin/Release/net7.0/linux-x64/publish"
$ImageName = "kiboards"
$ImageTag = $(dotnet gitversion /showvariable SemVer || "1.0.0")

if (!(Test-Path $PublishDir)) {
    # Publish solution in src directory as non self contained, linux-x64 runtime for docker container
    dotnet publish -nologo --configuration Release --runtime linux-x64 --no-self-contained src
}

# Suppress message `Use 'docker scan' to run Snyk tests against images to find vulnerabilities and learn how to fix them`
$env:DOCKER_SCAN_SUGGEST="false"

# Build docker image 
docker build --build-arg PUBLISH_DIR=${PublishDir} --build-arg ENTRYPOINT_DLL=${EntryPoint} -t ${ImageName}/${ImageTag} .
