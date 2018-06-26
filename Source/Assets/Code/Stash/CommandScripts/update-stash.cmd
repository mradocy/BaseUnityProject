:: Uses KDiff3 to update code here from the Stash folder in BaseUnityProject.

@echo OFF
setlocal

:: Absolute directory to update the Stash folder from:
set SOURCE_DIRECTORY=D:\Mark\Gamedev\Projects\Unity Projects\BaseUnityProject\Source\Assets\Code\Stash

:: The relative directory of this Stash folder:
set DESTINATION_DIRECTORY=../.

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
call kdiff3 "%SOURCE_DIRECTORY%" "%~dp0%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%"



:: more info on cmd scripts:
:: - http://steve-jansen.github.io/guides/windows-batch-scripting/index.html
:: - call /?
:: - use %~dp0 to get the directory of this file.
:: - KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
