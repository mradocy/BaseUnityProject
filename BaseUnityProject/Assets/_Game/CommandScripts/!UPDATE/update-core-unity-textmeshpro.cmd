:: Uses KDiff3 to update code for the Core.Unity.TextMeshPro namespace.

@echo OFF
setlocal
pushd "%~dp0"

:: Directory to update Core.Unity.TextMeshPro from:
set SOURCE_DIRECTORY=%GAMEDEV_SOURCE_UNITY%\BaseUnityProject\BaseUnityProject\Assets\_Game\Libs\Core.Unity.TextMeshPro

:: Directory of this Core.Unity.TextMeshPro folder:
set DESTINATION_DIRECTORY=..\..\Libs\Core.Unity.TextMeshPro

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;*.cginc;

:: Directories to ignore:
set DIR_IGNORE=.svn;.hg;.git;.vs;Library;library;Temp;temp;Obj;obj;Bin;bin

:: Files to ignore:
set FILE_IGNORE=*.sln;*.csproj;*.unityproj


:: Calling KDiff3 (documentation: http://kdiff3.sourceforge.net/doc/documentation.html)
call kdiff3 "%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%" --cs "DirAntiPattern=%DIR_IGNORE%" "" --cs "CreateBakFiles=0"


popd