/** \page pageQuickStart QuickStart
 * 
 * %Cubiquity for Unity3D is designed to be as easy and intuitive as possible, and so this quick start guide should be all you need to begin creating voxel worlds for your games. If you have any difficulties or wish to follow up some topics in more depth then you can see the appropriate sections of this user manual for more details.
 * 
 * \section secInstall Download and Install
 * 
 * There are currently two ways of obtaining Cubiquity for Unity3D. You can either download and install a '.unitypackage' file (available on the Asset Store or from our website), or get the latest development version from our git repository.
 *
 * \subsection secInstallFromPackage Installing a Package File
 *
 * When purchasing Cubiquity for Unity3D on the Asset Store you are immediately provided with an option to install. If you instead choose to save the package file for later use, or if you choose to download the non-commercial version of Cubiquity for Unity3D from our website, then you need to import the package file by going to the main menu in the Unity3D editor and selecting `Assets -> Import Package -> Custom Package..` and then locating your downloaded package file. The unity package importer will then present you with a list of files which you can choose to import - for now the best option is to make sure everything is selected and then press `Import`.
 *
 * The process we have described here is standard for all Unity3D packages, so if you have any difficulties you should consult the Unity3D documentation.
 *
 * \subsection secInstallFromGit Installing from Git
 *
 * Advanced users may be interested in using the latest development version of Cubiquity for Unity3D, rather than the stable releases. By doing this you may get access to functionality before it is officially released, but you can should expect that this is not yet fully tested. If you are interested in this then you can find our Git repository at https://bitbucket.org/volumesoffun/cubiquity-for-unity3d where further instructions are also provided.
 *
 * \section secFirstTerrainVolume Creating your first voxel terrain
 *
 * Cubiquity for Unity3D supports two types of voxel environments. We will begin by looking at the *Terrain Volume* which, as the name suggests, is intended for representing natural terrains. From a user point of view it is similar to Unity3D's built-in terrain, but additionally supports caves, overhangs, and flexible run-time editing.
 *
 * To create a Terrain Volume from within the unity3D editor you should go to the main menu and select `GameObject -> Create Other -> Terrain Volume`. The terrain volume will be created at the origin of your scene and should appear as shown in the image below:
 * 
 * Picture here...
 *
 * Your new terrain should be automatically selected, and if you move your mouse cursor over it you should see a light-blue *brush marker* following your cursor position. This indicates where any terrain modification operations will be performed. You can click with the left mouse button to begin adding material into the terrain, thereby creating hills and ridges.
 *
 * Then the terrain is selected you will see the editing controls displayed in the Unity3D inspector panel (see picture below). These control the tools which you can use to modify the shape of your terrain or to apply materials to it. You can choose an tool to apply (sculpt, smooth, etc) by selecting one of the buttons at the top of the inspector, and then choose your desired brush options and/or materials. Left-clicking on the terrain will then apply the tool.
 *
 * Picture of tools here...
 *
 * Take some time to experiment with the editing tools which are available. After you have created a simple terrain you can also try importing one of the standard Unity character controllers so that you can walk around your terrain in play mode. Be aware that it can take a few seconds for the terrain to generate after you press the play button, so for this reason you should set your character to start a hundred units or so above the terrain. This way the terrain will have time to load before the character reaches ground level.
 *
 */