# Docker image name
$ImageName = "jandini/kiboards"
# Docker image tag from gitversion or 1.0.0 
$ImageTag = $(dotnet gitversion /showvariable SemVer); if (!$ImageName) { $ImageName = "1.0.0" }


# Publish solution in src directory as non self contained, linux-x64 runtime for docker container
dotnet publish -nologo --configuration Release --runtime linux-x64 --no-self-contained src
# Suppress message `Use 'docker scan' to run Snyk tests against images to find vulnerabilities and learn how to fix them`
$env:DOCKER_SCAN_SUGGEST="false"
# Build docker image 
docker build -t ${ImageName}:${ImageTag} .
