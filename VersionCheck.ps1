$content = Invoke-RestMethod -Uri "https://api.nuget.org/v3-flatcontainer/elgatowavesdk/index.json"
$nugetVersionString = $content.versions[$content.versions.length - 1]
if($nugetVersionString.Contains("-")){
    $nugetVersion = New-Object System.Version($nugetVersionString.Substring(0, $nugetVersionString.IndexOf("-"))) 
}else{
    $nugetVersion = New-Object System.Version($nugetVersionString) 
}

$projNode = Select-Xml -Path .\ElgatoWaveAPI\ElgatoWaveSDK.csproj -XPath '/Project/PropertyGroup/Version'
$projVersionString = $projNode.Node.InnerText
$projVersion = New-Object System.Version($projVersionString);

if($projVersion -eq $nugetVersion){
    Write-Output 'Project version is the same as NuGet release, please update version'
    exit 1
}
if($projVersion -lt $nugetVersion){
    Write-Output 'Project version is behind NuGet release, please update version'
    exit 1
}
exit 0