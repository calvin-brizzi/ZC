using UnityEngine;
using System.Collections;

public class VarMan : Singleton<VarMan> {
    protected VarMan () {}

    public int pNum;
    public int humans = 0;
	public int lava = 1000;

	public int housing=20;
    

}
