:: Sets up the base essentials of a Unity project.
:: Uses KDiff3 to copy files over from BaseUnityProject to the current directory.
:: Note that this must be called from the directory it's contained in.

@echo OFF
setlocal

:: Absolute directory containing the BaseUnityProject:
set SOURCE_DIRECTORY=D:\Mark\Gamedev\Projects\Unity Projects\BaseUnityProject

:: Name of the source's Unity project (the folder that gets opened in Unity):
set SOURCE_UNITY_PROJECT_NAME=BaseUnityProject

:: The relative directory of this folder:
set DESTINATION_DIRECTORY=.

:: Directories to ignore:
set DIRECTORY_ANTI_PATTERNS=Testing;Sandbox;CVS;.deps;.svn;.hg;.git;.vs;Library;library;Temp;temp;Obj;obj;ActionLogs;actionLogs

:: Files to ignore:
set FILE_ANTI_PATTERNS=Testing.meta;Sandbox.meta;README.txt;README.txt.meta;*.sln;*.csproj;*.unityproj

:: Name of this Unity project (taken from parent of the directory that called this command script):
for %%I in (.) do set UNITY_PROJECT_NAME=%%~nI%%~xI


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
call kdiff3 "%SOURCE_DIRECTORY%" "%~dp0%DESTINATION_DIRECTORY%" -m --cs "FilePattern=*.*" --cs "DirAntiPattern=%DIRECTORY_ANTI_PATTERNS%" --cs "FileAntiPattern=%FILE_ANTI_PATTERNS%" --cs "CreateBakFiles=0"


:: Rename unity project
ren "%SOURCE_UNITY_PROJECT_NAME%" "%UNITY_PROJECT_NAME%"


:: Create empty directories that should be there (empty folders aren't saved in vcs)
if not exist ".\Builds" mkdir ".\Builds"
if not exist ".\Presentation" mkdir ".\Presentation"
if not exist ".\Workbench" mkdir ".\Workbench"
if not exist ".\Ideas" mkdir ".\Ideas"

:: Create empty directories in Assets that should be there (empty folders aren't saved in vcs)
if not exist "%UNITY_PROJECT_NAME%\Assets\_Fonts" mkdir "%UNITY_PROJECT_NAME%\Assets\_Fonts"
if not exist "%UNITY_PROJECT_NAME%\Assets\_Materials" mkdir "%UNITY_PROJECT_NAME%\Assets\_Materials"
if not exist "%UNITY_PROJECT_NAME%\Assets\_Prefabs" mkdir "%UNITY_PROJECT_NAME%\Assets\_Prefabs"
if not exist "%UNITY_PROJECT_NAME%\Assets\_Sprites" mkdir "%UNITY_PROJECT_NAME%\Assets\_Sprites"



:: more info on cmd scripts:
:: - http://steve-jansen.github.io/guides/windows-batch-scripting/index.html
:: - call /?
:: - use %~dp0 to get the directory of this file.
:: - KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
