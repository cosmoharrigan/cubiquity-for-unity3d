/** \page pageTroubleShooting Trouble Shooting
 *
 * We hope that %Cubiquity for Unity3D runs smoothly for you, but if you have any problems then you should check this page for potential solutions.
 * 
 * \section secCheckConsistency CheckConsistency: Transform child can't be loaded
 *
 * This error message is caused by what appears to be undesirable behavior with regards to the way Unity handles the 'DontSave' flag. Specifically, if the 'DontSave' flag is set on a GameObject in the scene hierarchy then Unity still serializes *references* to that GameObject even though it does not serialize the GameObject itself. This behavior is problematic for various parts of %Cubiquity as we rely heavily of generating GameObjects at runtime (including in edit mode) and naturally we don't want them serialized to disk.
 *
 * We have taken various steps to try an overcome this problem, but your own code can still cause such an error by setting the 'DontSave' flag on any object to which you have serializable references. Setting it on GameOjects at the root of the hierarchy appears to be ok (as it has no parent serializing a reference to it). Notice how our procedural generation examples do not create a new game object and components at runtime, but instead simply create and attach data to an existing game object and components. You should probably follow a similar pattern if implementing your own procedural generation.
 *
 * More information on this issue: http://answers.unity3d.com/questions/609621/hideflagsdontsave-causes-checkconsistency-transfor.html
 *
 */