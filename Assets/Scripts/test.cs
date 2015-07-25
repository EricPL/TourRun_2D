using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	void Awake()
	{
		ResourceManager.Instance.LoadAssetAsync("CanvasTest",typeof(GameObject),new LoadCallBack(),true);
	}
	
	public void Log()
	{
		Debug.Log("testLoad Over!!!!");
	}
	
	
	public class LoadCallBack : ResourceManager.ILoadListening
	{
		#region ILoadCallBack implementation
		public void Succeed (Object asset)
		{
			//throw new System.NotImplementedException ();
			if(asset!=null)
			{
				GameObject target = GameObject.Instantiate(asset as GameObject) as GameObject;
				target.name= asset.name;
			}
			
			Debug.Log("suceess");
		}
		public void Failure ()
		{
			Debug.Log("failure");
			//throw new System.NotImplementedException ();
		}
		#endregion
	}
	
}
