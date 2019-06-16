:: Uses KDiff3 to commit the Core.Unity.Rewired namespace to the base unity project

@echo OFF
setlocal
pushd "%~dp0"

:: Directory of Core.Unity.Rewired in this Unity project.
set SOURCE_DIRECTORY=..\..\Libs\Core.Unity.RewiredExtensions

:: Directory of Core.Unity.Rewired in BaseUnityProject:
set DESTINATION_DIRECTORY=%GAMEDEV_SOURCE_UNITY%\BaseUnityProject\BaseUnityProject\Assets\_Game\Libs\Core.Unity.RewiredExtensions

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
:: KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
call kdiff3 "%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%" --cs "CreateBakFiles=0"


popd