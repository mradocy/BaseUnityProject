:: Uses KDiff3 to commit code in the Stash folder to BaseUnityProject.

@echo OFF
setlocal
pushd "%~dp0"

:: The relative directory of this Stash folder:
set SOURCE_DIRECTORY=..\.

:: Absolute directory to merge the Stash folder into into:
set DESTINATION_DIRECTORY=%GAMEDEV_PROJECTS%\Unity Projects\BaseUnityProject\BaseUnityProject\Assets\_Scripts\Stash

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
:: KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
call kdiff3 "%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%" --cs "CreateBakFiles=0"


popd