# BaseUnityProject

Anything in the Testing folder will NOT be copied over to a new project.  Files here are intended to be used for testing the Stash code.

-------------------------------------------------------------------

How to create a new Unity Project:

## PART 1: Git Setup

* Go to Github.com and create a new repository.

* (Optional) Make the repository private
* Use the default Unity .gitignore.

* Copy the repository path (using HTTPS).  It should look like https://github.com/[username]/[project_name].git

* Open SourceTree as an administrator.

* Open a new tab and go to Remote repositories.

* If there's a problem with getting repositories, do the following:
* Right-click [username] and select Edit Account	
	* Ensure the following values are used in the Edit Hosting Account window:
		* Hosting Service: GitHub
		* Preferred Protocol: HTTPS
		* Authentication: OAuth
		* (try pressing Refresh OAuth Token if there's a problem with authentication)

* Clone the repo that was just created in Github.

* Set the destination directory to somewhere in the Projects folder.
	* e.g. if it's a unity project, set the destination to D:\Mark\Gamedev\Projects\Unity Projects\ [project_name].
	* The folder must exist and be empty.

## PART 2: Copying the Base Unity Project

* Make sure KDiff3 is installed and can be accessed through the command line.

* Copy create-base-unity-project.cmd (located at the root of BaseUnityProject) into the root project folder.

* Run create-base-unity-project.cmd.  It will use KDiff3 to copy the files from BaseUnityProject to the new Unity project.

* create-base-unity-project.cmd can then be deleted from the new Unity project.
