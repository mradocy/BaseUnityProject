:: Uses KDiff3 to commit code in the Stash folder to BaseUnityProject.

@echo OFF
setlocal

:: The relative directory of this Stash folder:
set SOURCE_DIRECTORY=../.

:: Absolute directory to merge the Stash folder into into:
set DESTINATION_DIRECTORY=D:\Mark\Gamedev\Projects\Unity Projects\BaseUnityProject\Source\Assets\_Scripts\Stash

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
call kdiff3 "%~dp0%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%" --cs "CreateBakFiles=0"



:: more info on cmd scripts:
:: - http://steve-jansen.github.io/guides/windows-batch-scripting/index.html
:: - call /?
:: - use %~dp0 to get the directory of this file.
:: - KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
