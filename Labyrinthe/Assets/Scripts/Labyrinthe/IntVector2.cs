/** Structure permettant de définir un Vecteur à 2 dimensions en X et Z */
[System.Serializable]
public struct IntVector2 {
	
	public int x, z;
	
	public IntVector2 (int x, int z) {
		this.x = x;
		this.z = z;
	}

	public static IntVector2 operator + (IntVector2 a, IntVector2 b) {
		a.x += b.x;
		a.z += b.z;
		return a;
	}

	public override string ToString ()
	{
		return string.Format ("[IntVector2] x : " + x + " z : " + z);
	}
}