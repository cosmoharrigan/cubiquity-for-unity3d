/** \page pageObtainingVolDat Obtaining Volume Data
 *
 * Although %Cubiquity for Unity3D comes with built in editor tools, they a currently quite limited (particularly for the colored cubes volume) and it is possible you may wish to obtain your volume data through other sources. Alternatively you may want your volume data to be generated procedurally at runtime rather than designed in advance. This section discusses both these possibilities.
 *
 * \section secImporting Importing From External Sources
 *
 * We have already seen how %Cubiquity for Unity3D can import existing voxel databases (see \ref secFromVoxelDatabase) but where did these .vdb files come from? For colored cubes volumes there are a couple of options discussed below.
 *
 * \subsection secMagica
 *
 * You may wish to model your voxel geometry in an external application such as Magica Voxel, Voxel Shop, or Quibicle Constructor. Such applications will typically allow more comprehensive editing options than we will ever provide in %Cubiquity, and %Cubiquity alrealy provides the option to import Magica Voxel files (others will follow in the future). This is done by the use of the command-line `ConvertToVDB` tool.
 *
 * To use this tool you should open a command prompt and change to the `StreamingAssets\Cubiquity\SDK` directory. From here you can run:
 *
 * `ConvertToVDB.exe -i input.vox -o output.vdb`
 *
 * You can then copy the resulting .vdb into the `StreamingAssets\Cubiquity\VoxelDatabases` folder before followint the instruction in \ref secFromVoxelDatabase to import it into Unity.
 *
 * \subsection secImageSlices
 *
 * Note that both Magica and image slices are only appropriate for importing colored cubes volumes. Currently there are no methods for creating terrain volumes *outside* of %Cubiquity for Unity3D, but you can still create them procedually as discussed later.
 *
 * \section secGenerating Generating Volume Data Through Scripts
 *
 * %Cubiquity for Unity3D provides a very simple but powerful API for generating volumes through code. Each volume is essentially just a 3D grid of voxel values, and the API gives you direct access to these through the VolumeData's GetVoxel(...) and SetVoxel(...) methods. You can then choose any method you wish to decide which values should be written to which voxels. Common approaches include:
 *
 * Using a noise function: Evalutaing a 3D noise function (such as Perlin noise or Simplex noise) at each point on the grid can generate both natural and surreal environments. Multiple octaves of noise can be combined to add additional detail. Please see the 'Procedural Generation' example in the examples folder.
 *
 * Reading an input image: The 'Maze' example (see the examples folder) reads a 2D image of a maze and sets the height of voxel columns based of whether the corresponding pixel is black or white. The same principle can be applied to generating a terrain from a heightmap.
 *
 * Building planets from spheres: You can create spheres by computing the distance function from a point, and a cube map can then be used to apply a texture. The 'Solar Sytem' example shows how this can be used to create planets.
 *
 * Overall there is a lot of potential for using your imagination here. If you can think of a way to generate a particular type of volume data then Cubiquity provides a way for you to bring it to life.
 */
 