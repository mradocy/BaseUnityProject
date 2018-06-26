:: Opens SourceTree for a given repository on the status screen.
:: (If it doesn't open on the status screen, it means there's nothing to commit.)

@echo OFF
setlocal

:: Absolute location of the sourcetree program
set SOURCETREE_PROGRAM="%LOCALAPPDATA%\SourceTree\SourceTree.exe"

:: Absolute directory of the repository to open in SourceTree
set REPOSITORY="D:\Mark\Gamedev\Projects\UnityCodeStash"


start "" %SOURCETREE_PROGRAM% -f %REPOSITORY% status



:: more info on cmd scripts:
:: - http://steve-jansen.github.io/guides/windows-batch-scripting/index.html
:: - call /?
:: - use %~dp0 to get the directory of this file.
:: - KDiff3 command line: http://kdiff3.sourceforge.net/doc/documentation.html