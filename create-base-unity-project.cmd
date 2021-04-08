:: Sets up the base essentials of a Unity project.
:: Uses robocopy to copy files over from BaseUnityProject to the current directory.
:: Note that this must be called from the directory it's contained in.

@echo OFF
setlocal

:: Confirm GAMEDEV_SOURCE_UNITY is defined
if not defined GAMEDEV_SOURCE_UNITY (
	echo User environment variable GAMEDEV_SOURCE_UNITY not defined.  Try running Gamedev\setup.cmd.
	pause
	goto END
)

:: Name of the source's Unity project (the folder that gets opened in Unity):
set SOURCE_UNITY_PROJECT_NAME=BaseUnityProject

:: Absolute directory containing the BaseUnityProject:
set SOURCE_DIRECTORY="%GAMEDEV_SOURCE_UNITY%\%SOURCE_UNITY_PROJECT_NAME%"

:: The relative directory of this folder:
set DESTINATION_DIRECTORY="%~dp0."

:: ensure that destination directory only contains 1 file (this command script)
set numFiles=0
for /f "delims=" %%a in ('dir /b %DESTINATION_DIRECTORY%') do set /a numFiles+=1
if %numFiles% GTR 1 (
	echo To run, this command script should be the only file in the folder.
	pause
	goto END
)

:: Directories to ignore:
set DIR_IGNORE=Testing Sandbox CVS .deps .svn .hg .git .vs Library library Temp temp Obj obj Logs logs ActionLogs actionLogs

:: Files to ignore:
set FILE_IGNORE=Testing.meta Sandbox.meta README.txt README.txt.meta *.sln *.csproj *.unityproj


:: Name of this Unity project (taken from parent of the directory that called this command script):
for %%I in (.) do set UNITY_PROJECT_NAME=%%~nI%%~xI

:: Calling robocopy (documentation: https://ss64.com/nt/robocopy.html)
robocopy %SOURCE_DIRECTORY% %DESTINATION_DIRECTORY% /xf %FILE_IGNORE% /xd %DIR_IGNORE% /e /mir

:: Error handling robocopy
if %ERRORLEVEL% NEQ 1 (
	echo Error encountered: %ERRORLEVEL%
	pause
	goto END
)
if not exist "%SOURCE_UNITY_PROJECT_NAME%" (
	echo "%SOURCE_UNITY_PROJECT_NAME%" not copied over.  Something went wrong with robocopy.
	pause
	goto END
)

:: Rename unity project
ren "%SOURCE_UNITY_PROJECT_NAME%" "%UNITY_PROJECT_NAME%"

:: Success
echo Unity project successfully created.
pause

:: Delete this command script
(goto) 2>nul & del "%~f0"

:END