# BaseUnityProject

Anything in the Testing folder will NOT be copied over to a new project.  Files here are intended to be used for testing the Stash code.

-------------------------------------------------------------------

How to create a new Unity Project:


## PART 1: Git Setup

* Open Github Desktop.  File -> New Repository
	* Make sure projects folder makes sense.
	* Use a default .gitignore if applicable (e.g. Unity, Visual Studio, etc.)

* Click 'Publish repository'
	* Optional: check 'Keep this code private'

Note: If having connections issues, try this: https://github.com/desktop/desktop/issues/4817#issuecomment-393141777


## PART 2: Copying the Base Unity Project

* Make sure KDiff3 is installed and can be accessed through the command line.

* Copy create-base-unity-project.cmd (located at the root of BaseUnityProject) into the root project folder.

* Run create-base-unity-project.cmd.  It will use KDiff3 to copy the files from BaseUnityProject to the new Unity project.

* create-base-unity-project.cmd can then be deleted from the new Unity project.
