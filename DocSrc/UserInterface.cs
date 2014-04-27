/** \page pageUserInterface User Interface
 *
  * %Cubiquity for Unity3D is tightly coupled with the Unity3D editor and strives to follow similar design and usage approaches to those used in other parts of Unity. This means that concepts such as component-based design, assets, inspectors, transform gizmos, and materials should all work in similar way to what you are used to, and the interface to the TerrainVolume also takes significant cues from Unity's standard terrain.
 *
 * We hope that you will therefore find the system intuitive, but none-the-less we use this section of the manual to outline the key user interface elements and how they fit together.
 *
 * \section secAddVolumes Adding Volumes To The Scene
 *
 * A new terrain volume or colored cubes volume can be added to the scene via the main menu.
 *
 * Picture of menu here...
 *
 * This adds a new game object to the scene and automatically attaches Volume, Volume Renderer, and VolumeCollider components. It also creates a small volume data with just the floor filled in so that you have something from which to start your editing. Note that you are not given any options during the creation process (it is intended to be a simple one-click-and-you're-done' interface), but you can later create bigger/different volume data as discussed in the section on \ref secNewVolData.
 *
 * \section secTransVolumes Transforming Volumes
 *
 * Because a %Cubiquity volume is just a GameObject with some specific components attached, it can be transformed much like any other object in Unity. Try selecting a volume and clicking one of the following buttons on the toolbar:
 *
 * Picture of toolbar
 *
 * You should see the corresponding transform gizmo appear on the volume and you can use this to manipulate it. As you drag the mouse you will see the transform changing in the inspector, and you can also also enter the desired transform directly.
 *
 * Picture of transformed volume with gizmo and inspector highlighted.
 *
 * \section secEditVolumes Editing Volumes
 *
 * We provide tools for editing the volumes from within the Unity editor (you can also edit the volumes in play mode but we don't provide tools for this - just use the API to implement your own game-specific tools).
 *
 * \subsection secEditTerrainVolumes Editing Terrain Volumes
 *
 * When a terrain volume is selected you should see the 'Terrain Volume (Script)' component in the inspector.
 *
 * Picture here
 *
 * You can cycle through the tabs to access the various functionality which is available:
 *
 * \subsubsection secSculptMode Sculpt Mode
 *
 * Picture here
 *
 * This mode is used to make most changes to the shape of the terrain. You can choose the desired brush (gentle vs. sharp falloff) and set the brush's radius. The 'Opacity' setting controls how quickly the changes are applied, so having the slider to the left will allow finer control than having to the right.
 *
 * Changes are applied by left-clicking on the desired part of the terrain. Note that changes are only applied when the mouse is moved, so you can't simply hold down the mouse on the same spot but must instead move it slightly. You can also hold down the Shift key when sculpting to remove material from the terrain instead of adding it.
 *
 * \subsubsection secSmoothMode Smooth Mode
 *
 * Picture here
 *
 * The smooth mode is used to eliminate jagged edges and sharp features on the terrain, as well as softening the boundary between different textures. It does this by averaging together a voxel with it's neighbours - a  kind of 3D equivalent to the 'blur' operation found in many 2D image editors. It is again possible to change the falloff, radius, and opacity of the brush performing the smoothing.
 *
 * \subsubsection secPaintMode Paint Mode
 *
 * Textures can be applied to the terrain by using Cubiquity's paint mode.
 *
 * Picture here
 *
 * Note that the range of available textures is defined by the currently applied material, and this can be changed via \ref secRendererComp as discussed later. The textures are not simply painted onto the mesh but are instead painted into the volume - i.e. the brush is not a circle but a sphere which also changes the texture of underground voxels. You can see the effect of this if you choose a large brush, paint part of the terrain, and then dig into the part which you have painted.
 *
 * \subsubsection secSettingsMode Settings Mode
 *
 * \subsection secEditColCubesVolumes Editing Colored Cubes Volumes
 * \section secOtherComps Other Volume Components
 * \subsection secRendererComp The Volume Renderer
 * \subsection secColliderComp The Volume Collider
 * \section secNewVolData Creating New Volume Data And Assets
 *
 */