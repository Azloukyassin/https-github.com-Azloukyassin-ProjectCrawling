$imgName = "projektor"
$exportPath = "."
$exportFileName = $imgName
$exportFileExtension = "tar"
$exportUri = $exportPath + "/" + $exportFileName + "." + $exportFileExtension
dotnet publish -c Release
docker build -t $imgName -f Dockerfile .
docker image save $imgName -o $exportUri