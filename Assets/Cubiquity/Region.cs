using UnityEngine;
using System.Collections;

[System.Serializable]
public class Region
{
	public IntVector3 lowerCorner;
	public IntVector3 upperCorner;
	
	public Region(int lowerX, int lowerY, int lowerZ, int upperX, int upperY, int upperZ)
	{
		lowerCorner = new IntVector3(lowerX, lowerY, lowerZ);
		upperCorner = new IntVector3(upperX, upperY, upperZ);
	}
	
	public Region(IntVector3 lowerCorner, IntVector3 upperCorner)
	{
		this.lowerCorner = lowerCorner;
		this.upperCorner = upperCorner;
	}
}
