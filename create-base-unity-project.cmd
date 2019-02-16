:: Sets up the base essentials of a Unity project.
:: Uses KDiff3 to copy files over from BaseUnityProject to the current directory.
:: Note that this must be called from the directory it's contained in.

@echo OFF
setlocal

:: Confirm GAMEDEV_PROJECTS is defined
if not defined GAMEDEV_PROJECTS (
	echo User environment variable GAMEDEV_PROJECTS not defined.  Try running Tools\setup.cmd.
	pause
	goto END
)

:: Name of the source's Unity project (the folder that gets opened in Unity):
set SOURCE_UNITY_PROJECT_NAME=BaseUnityProject

:: Absolute directory containing the BaseUnityProject:
set SOURCE_DIRECTORY=%GAMEDEV_PROJECTS%\Unity Projects\%SOURCE_UNITY_PROJECT_NAME%

:: The relative directory of this folder:
set DESTINATION_DIRECTORY=%~dp0.

:: ensure that destination directory only contains 1 file (this command script)
set numFiles=0
for /f "delims=" %%a in ('dir /b "%DESTINATION_DIRECTORY%"') do set /a numFiles+=1
if %numFiles% GTR 1 (
	echo This command script should only run in an empty folder.
	pause
	goto END
)

:: Directories to ignore:
set DIRECTORY_ANTI_PATTERNS=Testing;Sandbox;CVS;.deps;.svn;.hg;.git;.vs;Library;library;Temp;temp;Obj;obj;ActionLogs;actionLogs

:: Files to ignore:
set FILE_ANTI_PATTERNS=Testing.meta;Sandbox.meta;README.txt;README.txt.meta;*.sln;*.csproj;*.unityproj

:: Name of this Unity project (taken from parent of the directory that called this command script):
for %%I in (.) do set UNITY_PROJECT_NAME=%%~nI%%~xI


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
:: KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
call KDiff3 "%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=*.*" --cs "DirAntiPattern=%DIRECTORY_ANTI_PATTERNS%" --cs "FileAntiPattern=%FILE_ANTI_PATTERNS%" --cs "CreateBakFiles=0"

:: Error handling KDiff3
if %ERRORLEVEL% NEQ 0 (
	echo Could not run KDiff3.  Has it been installed and added to the system path?
	pause
	goto END
)
if not exist "%SOURCE_UNITY_PROJECT_NAME%" (
	echo "%SOURCE_UNITY_PROJECT_NAME%" not copied over.  KDiff3 execution may have been terminated.
	pause
	goto END
)

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
if not exist "%UNITY_PROJECT_NAME%\Assets\_Testing" mkdir "%UNITY_PROJECT_NAME%\Assets\_Testing"

:: Delete this command script
(goto) 2>nul & del "%~f0"


:END