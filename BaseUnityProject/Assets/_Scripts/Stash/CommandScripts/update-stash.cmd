:: Uses KDiff3 to update code here from the Stash folder in BaseUnityProject.

@echo OFF
setlocal
pushd "%~dp0"

:: Absolute directory to update the Stash folder from:
set SOURCE_DIRECTORY=%GAMEDEV_SOURCE%\Unity Projects\BaseUnityProject\BaseUnityProject\Assets\_Scripts\Stash

:: The relative directory of this Stash folder:
set DESTINATION_DIRECTORY=..\.

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
:: KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
call kdiff3 "%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%" --cs "CreateBakFiles=0"


popd