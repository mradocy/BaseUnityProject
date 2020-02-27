:: Uses KDiff3 to commit code to the CutScript project.

@echo OFF
setlocal
pushd "%~dp0"

:: Directory of CutScript in this Unity project.
set SOURCE_DIRECTORY=..\..\Libs\Core.CutScript

:: Directory of the CutScript project:
set DESTINATION_DIRECTORY=%GAMEDEV_SOURCE_TOOLS%\CutScript\Core.CutScript

:: The (only) types of files to merge.
set FILE_PATTERNS=*.cs;*.cmd;*.bat;*.exe;*.txt;*.shader;*.md;

:: Directories to ignore:
set DIR_IGNORE=Obj;obj;Bin;bin


:: Calling KDiff3 (make sure KDiff3 is installed and contained in %PATH%)
:: KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html
call kdiff3 "%SOURCE_DIRECTORY%" "%DESTINATION_DIRECTORY%" -m --cs "FilePattern=%FILE_PATTERNS%" --cs "DirAntiPattern=%DIR_IGNORE%" --cs "CreateBakFiles=0"


popd