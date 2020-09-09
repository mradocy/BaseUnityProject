# BaseUnityProject

My personal starter code and directory structure for new Unity projects.  I like keeping the repositories for my projects private, but I suppose there's no harm in sharing the base project.

Most notable is game code contained in `BaseUnityProject/BaseUnityProject/Assets/_Game/Libs/`.  Code contained in `Core.Unity` can be used in any Unity project, while `Core.Unity.TextMeshPro` and `Core.Unity.RewiredExtensions` have dependencies on their respective Unity extensions.

Code can be updated from or committed to their library in BaseUnityProject using command scripts in `BaseUnityProject/BaseUnityProject/Assets/_Game/CommandScripts/`.  These scripts use external tool KDiff3 to copy files from one directory to another, while notifying the user of changes and allowing them to be diffed if desired.  This is a relatively simple way to update the shared code in the projects without the extra 'baggage' of Git submodules.

Anything in the Testing folder will NOT be copied over to a new project.  Files here are intended to be used for testing the Core.Unity code.

## Creating a new project

* Copy create-base-unity-project.cmd (located at the root of BaseUnityProject) into the root project folder.

* Run create-base-unity-project.cmd.  It will use robocopy to copy the files from BaseUnityProject to the new Unity project.  This file will then delete itself.