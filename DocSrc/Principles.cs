/** \page pagePrinciples Principles of Cubiquity for Unity3D
 *
 * We have worked hard to make both the visual and the programmatic interface to Cubiquity for Unity3D as simple and intuitive as possible. However, there is no getting around the fact that internally it is a powerful and flexible system, and with this comes a certain amount of complexity. Having a high-level understanding of how the system is working internally will help you to make more effective use of it, particularly when using it from code.
 *
 * Therefore we use this part of the user manual to provide all the information which we feel may be relevant to people to push the system as far as possible, or even just those who like some understanding of what is happening behind-the-scenes. We will talk at a high level about the structure of the system, the algorithms which underpin it, and the design decisions we have made for its implementation.
 *
 * Before proceeding to read about these principles we would recommend that you at least work through the Quick Start [LINK] guide to get a feeling for what the system can do. This will provide you with some useful context when reading about the underlying details.
 *
 * We begin by ... and then ...
 *
 * \section Voxel Engine Concepts
 *
 * \section secCubiquity The Cubiquity Voxel Engine
 *
 * One of the first points to note is the difference between *Cubiquity* and *Cubiquity for Unity3D*. The first of these, *Cubiquity*, is a native code (i.e. C/C++) library for storing, editing, and rendering voxel worlds. It is not tied to any particular game engine or platform, and can be used from a variety of languages. On the other hand, *Cubiquity for Unity3D* is a set of C# scripts which connect Cubiquity to the Unity3D. These scripts allow Unity3D games to create, edit and display Cubiquity volumes. As a user of Cubiquity for Unity3D you do not have access to the underlying C/C++ code of Cubiquity, but you do have a compiled version of the Cubiquity and the source code to the C# integration scripts.
 *
 * The interaction between the underlying Cubiquity voxel engine and the Cubiquity for Unity3D integration layer is shown in the diagram below. Further information is provided after the diagram.
 *
 * Diagram here...
 *
 * \subsection secNativeCode The Cubiquity Native-Code Library
 *
 * Functions defined in the native-code library can be called from Unity3D scripts using some magic known as P/Invoke [LINK]. The file 'CubiquityDLL.cs' uses this P/Invoke technology to provide thin .NET wrappers around each function which is available in the Cubiquity engine. The rest of the Cubiquity for Unity3D scripts are then implemented in terms of these wrapper functions. As a user of Cubiquity for Unity3D you are unlikely to come across these implementation details unless you deliberately go exploring the code (which of course you are welcome to do).
 *
 * Using native-code allows for some significant performance and memory optimizations compared to systems implemented using just Unity3D scripts. However, it also imposes some limitations, in particular that it cannot be used with the Unity3D web-player because this does not support native code. The native-code library also has to be compiled separately for each platform on which it needs to run (Windows, OSX, Linux, etc), but this is an issue which we as developers have to deal with rather than you as a user.
 * 
 * \subsection secVoxelDatabase The Cubiquity Voxel Database Format
 *
 * Voxel environments can get very large (potentially containing billions of voxels) so efficient storage of such worlds is of utmost importance. The Cubiquity voxel engine stores a volume as a *Voxel Database*, which is a single file containing all the voxels in the volume. Internally it is actually an SQLite [LINK] database and so can be opened with an SQLite viewer such as .... Such as tool will let you view certain properties of the volume such as its dimensions, but you won't be able to gain any meaningful insight into the voxel data itself as it is stored in an efficient compressed format.
 *
 * Cubiquity for Unity3D wraps these voxel databases with a class called VoxelData, and more specifically with its subclasses called TerrainVolumeData and ColoredCubesVolumeData. These classes provides functions to retrieve and modify individual voxel values, and can also be saved to disk as a Unity3D asset. Full details of using these classes to interact with voxel databases are presented later in section XXX and in the 'Working with volumes from code' [LINK] section of this user manual.
 * 
 */