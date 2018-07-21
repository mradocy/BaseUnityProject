:: Sets up the base essentials of a Unity project.
:: Uses KDiff3 to copy files over from BaseUnityProject to the current directory.

@echo OFF
setlocal

:: Creating empty directories that should be there, despite not being backed up in version control
if not exist "%~dp0Builds" mkdir "%~dp0Builds"



:: Absolute directory containing the BaseUnityProject:
set SOURCE_DIRECTORY=D:\Mark\Gamedev\Projects\Unity Projects\BaseUnityProject

:: The relative directory of this folder:
set DESTINATION_DIRECTORY=.

:: Directories to ignore:
set DIRECTORY_ANTI_PATTERNS=Testing;CVS;.deps;.svn;.hg;.git;.vs;Library;library;Temp;temp;Obj;obj;ActionLogs;actionLogs

:: Files to ignore:
set FILE_ANTI_PATTERNS=Testing.meta;README.txt;README.txt.meta;*.sln;*.csproj;*.unityproj

:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
call kdiff3 "%SOURCE_DIRECTORY%" "%~dp0%DESTINATION_DIRECTORY%" -m --cs "FilePattern=*.*" --cs "DirAntiPattern=%DIRECTORY_ANTI_PATTERNS%" --cs "FileAntiPattern=%FILE_ANTI_PATTERNS%" --cs "CreateBakFiles=0"



:: more info on cmd scripts:
:: - http://steve-jansen.github.io/guides/windows-batch-scripting/index.html
:: - call /?
:: - use %~dp0 to get the directory of this file.
:: - KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
