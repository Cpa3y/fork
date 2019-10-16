param (
    [string]$Solution,
	[string]$Configuration = 'Release',
	[switch]$Publish,
	[switch]$Testing = $true,
    [switch]$Package,	
	[switch]$Log
)

$WorkDir = 	Split-Path -Parent $PSCommandPath
$Solutions = if ($Solution -eq '') { Get-Item ("{0}\*.sln" -f $WorkDir) | %{ $_.FullName } } else { $Solution }
$NugetPath = Join-Path $WorkDir '.nuget\nuget.exe'

$MSBuildExePath = ls "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\Bin\MSBuild.exe" -ErrorAction Ignore | %{ $_.FullName }  | Select-Object -Last 1
if (!$MSBuildExePath)
{
  $MSBuildToolsPath = (Resolve-Path HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\* | 
	Get-ItemProperty -Name MSBuildToolsPath |
	Sort-Object { [double]::Parse($_.PSChildName, [System.Globalization.CultureInfo]::InvariantCulture) } -Descending |
	Select-Object -First 1).MSBuildToolsPath
  $MSBuildExePath = Join-Path $MSBuildToolsPath 'msbuild.exe'
}

#
#$MSBuildToolsPath = (Resolve-Path HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\* | build.ps1
#	Get-ItemProperty -Name MSBuildToolsPath |
#	Sort-Object { [double]::Parse($_.PSChildName, [System.Globalization.CultureInfo]::InvariantCulture) } -Descending |
#	Select-Object -First 1).MSBuildToolsPath
#$MSBuildExePath = Join-Path $MSBuildToolsPath 'msbuild.exe'


$PackagePath = $WorkDir #Join-Path $WorkDir 'artifacts\packages'


# download nuget.exe

if (!(Test-Path $NugetPath))
{
	Write-Host 'Downloading latest version of NuGet.exe...'
	$ProgressPreference = 'SilentlyContinue'
	Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/v4.1.0/nuget.exe' -OutFile $NugetPath
}

# restore packages
$Solutions | %{ (Start-Process -PassThru -NoNewWindow -Wait $NugetPath ('restore',$_) ).ExitCode } | ?{ $_ -ne 0 } | %{ exit $_ }

# build solution
foreach ($solution in $Solutions)
{
	$buildArgs = @($solution, ('/p:Configuration={0}' -f $Configuration))
	if ($Publish)
	{
		#$buildArgs += '/t:DeployISCC'

		$buildArgs += '/p:DeployOnBuild=true'
#		$buildArgs += '/p:PublishProfile={0}' -f $Publish
	}
	if ($Log)
	{
		$buildArgs += '/l:FileLogger,Microsoft.Build.Engine;logfile="{0}".log;append=true;verbosity=normal;encoding=utf-8' -f $solution
	}

	& $MSBuildExePath $buildArgs

	if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

# testing
#if ($Testing)
#{
#	$NUnitPath = (Get-Item ("{0}\packages\NUnit.Console*\tools\nunit*-console.exe" -f $WorkDir) | Select-Object -First 1).FullName
#	$TestDlls = Get-Item ("{0}\tests\*.Test*" -f $WorkDir) | 
#					%{ '{0}\bin\Release\{1}.dll' -f $_.FullName,$_.Name } |
#					?{ Test-Path $_ }
#
#	& $NUnitPath '--noresult' $TestDlls
#	if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
#}

# build packages

function Create-Package($Nuspec, $OutDir)
{
	#  
	$ProjectDir = Split-Path $Nuspec -Parent

	#   
	$ProjectFile = [IO.Path]::ChangeExtension($Nuspec, 'csproj')

    if (!(Test-Path $ProjectFile))
    {
        $ProjectFile = Get-Item([IO.Path]::GetDirectoryName($Nuspec) + '\*.csproj') | %{ $_.FullName } | Select-Object -First 1
    }

	#    
	$VersionFiles = Select-Xml -Path $ProjectFile -Namespace @{Project="http://schemas.microsoft.com/developer/msbuild/2003"} -XPath '//Project:Compile' |
		%{ Join-Path $ProjectDir $_.Node.Include } #  

	$Versions = @()
	$Versions += $VersionFiles | %{ Select-String -Path $_ -Pattern '(?<=^\s*\[\s*assembly:\s*AssemblyInformationalVersion\s*\(\s*"\s*).+?(?=(\.\*)?")' }
	$Versions += $VersionFiles | %{ Select-String -Path $_ -Pattern '(?<=^\s*\[\s*assembly:\s*AssemblyVersion\s*\(\s*"\s*).+?(?=(\.\*)?")' }
	$Version = $Versions | %{ $_.Matches.Value } | Select-Object -First 1 #  

	#    
	$Packages = @{}
	Get-ChildItem '.\packages' |
		%{ [regex]::Matches($_.Name, '^(.+?)\.([0-9]+[A-Za-z0-9\.\-]*)$') } |
		%{ $Packages[$_.Groups[1].Value] = $_.Groups[2].Value }
	$PackageDependencies = ($Packages.GetEnumerator() | %{ 'version_{0}={1}' -f ($_.Key.Replace('.', '_')), $_.Value }) -join ';'

    New-Item -ItemType Directory $OutDir -ErrorAction Ignore | Out-Null
	& $NugetPath pack $Nuspec -NonInteractive -Prop $PackageDependencies -Prop version=$Version -OutputDirectory $OutDir -Prop ('Configuration=' + $Configuration)
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

if ($Package)
{
    Get-ChildItem -Path (Join-Path $WorkDir 'src') -Filter '*.nuspec' -Recurse | %{ $_.FullName } | %{ Create-Package $_ $PackagePath }
}



