using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour {

	/// <summary>
	/// Resource Singleton
	/// </summary>
	private static ResourceManager _instance;
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static ResourceManager Instance
	{
		get{
			if(_instance==null)
			{
				GameObject obj= new GameObject("ResourceManager");
				_instance = obj.AddComponent<ResourceManager>();	
				mProcessorCount=SystemInfo.processorCount;
				mProcessorCount=mProcessorCount< 1 ? 1: mProcessorCount;
				mProcessorCount=mProcessorCount> 8 ? 8 : mProcessorCount;
			}
			
			return _instance;
		}
	}
	
	private GameObject mCachedGameObject;
	public GameObject CachedGameObject
	{
		get{
			if(mCachedGameObject==null)
			{
				mCachedGameObject=this.gameObject;
			}
			return mCachedGameObject;
		}	
	}
	
	/// <summary>
	/// CPU number
	/// </summary>
	private static int mProcessorCount=0;  
	
	void Awake()
	{
		DontDestroyOnLoad(CachedGameObject);
	}
	
	/// <summary>
	/// All completed asset in a Dictionary
	/// </summary>
	private Dictionary<string,AssetPack> mAssetPackDic=new Dictionary<string, AssetPack>(); 
	
	/// <summary>
	/// Currently Loading Queue
	/// </summary>
	private List<ResRequest> mLoadListIns=new List<ResRequest>();
	
	/// <summary>
	/// the Queue waitting for loading. 
	/// </summary>
	private Queue<ResRequest> mWaitForLoadings=new Queue<ResRequest>();
	

	/// <summary>
	/// Loads the asset async.
	/// </summary>
	/// <param name="_prefabName">_prefab name.</param>
	/// <param name="_type">_type.</param>
	/// <param name="_callBack">_call back.</param>
	/// <param name="_isKeepInMemory">If set to <c>true</c> _is keep in memory.</param>
	public void LoadAssetAsync(string _prefabName,Type _type,ILoadListening _callBack, bool _isKeepInMemory)
	{
//		if(mAssetPackDic.ContainsKey(_prefabName))
//		{
//			if(_callBack!=null)
//			{
//				_callBack.Succeed(mAssetPackDic[_prefabName].resourceRequest.asset);
//			}
//			return;
//		}
//	
//		AssetPack ap=new AssetPack(_prefabName,_callBack,_type,_isKeepInMemory);
//		mLoadQueue.Add(ap);
		
		_LoadAssetAsync(_prefabName,_type,_callBack,_isKeepInMemory);	
		
	}

	private void _LoadAssetAsync (string _prefabName, Type _type, ILoadListening _callBack, bool _isKeepInMemory)
	{
		if(string.IsNullOrEmpty(_prefabName))
		{
			if(null!=_callBack)
				_callBack.Failure();
			
			return ;
		}
	
		if (mAssetPackDic.ContainsKey(_prefabName))
		{
			if(mAssetPackDic[_prefabName].asset==null)
			{
				if(_callBack!=null)
					_callBack.Failure();
				
			}
			else
			{
				_callBack.Succeed(mAssetPackDic[_prefabName].asset);
			}
			
			return;
		}
		
		for (int i = 0; i < mLoadListIns.Count; ++i) 
		{
			ResRequest request=mLoadListIns[i];
			
			if(request.assetName.Equals(_prefabName))	
			{
				request.AddListening(_callBack);
				return ; 
			}
		}
		
		foreach (ResRequest request in mWaitForLoadings) {
			if(request.assetName.Equals(_prefabName))
			{
				request.AddListening(_callBack);
				return;
			}
		}
		
		ResRequest loadRequest= new ResRequest(_prefabName,_isKeepInMemory,_type);
		loadRequest.listenings.Add(_callBack);
		mWaitForLoadings.Enqueue(loadRequest);
	}	
	
	
	/// <summary>
	/// Remove the asset which is not used now
	/// </summary>
	/// <param name="assetName">Asset name.</param>
	/// <param name="canRemoveKeepinMemory">If set to <c>true</c> can remove keepin memory.</param>
	public void Remove(string assetName,bool canRemoveKeepinMemory)
	{
		if(!mAssetPackDic.ContainsKey(assetName))
		{
			return;
		}
		
		if(mAssetPackDic[assetName].isKeepInMemory)
		{
			if(canRemoveKeepinMemory)
			{
				mAssetPackDic[assetName]=null;
				mAssetPackDic.Remove(assetName);
			}
		}
		else
		{
			mAssetPackDic[assetName]=null;
			mAssetPackDic.Remove(assetName);
		}
		
		Resources.UnloadUnusedAssets();
	}
	
	/// <summary>
	/// Removes all assets. Note to use
	/// </summary>
	public void RemoveAll()
	{
		foreach (KeyValuePair<string, AssetPack> pair in mAssetPackDic) 
		{
			mAssetPackDic[pair.Key]=null;
		}
		
		mAssetPackDic.Clear();
		Resources.UnloadUnusedAssets();
	}
	
	void Update()
	{
//		if(mLoadQueue.Count>0)
//		{
//			int count =mLoadQueue.Count-1;
//			while(count+1>0)
//			{
//				if(mLoadQueue[count]!=null)
//				{
//					Debug.Log(mLoadQueue[count].isLoad);
//					
//					if(!mLoadQueue[count].isLoad && mLoadQueue[count].resourceRequest==null)
//					{
//						_LoadAsset(mLoadQueue[count]);
//					}
//					else if(mLoadQueue[count].isLoad && mLoadQueue[count].resourceRequest!=null)
//					{
//						if(!mAssetPackDic.ContainsKey(mLoadQueue[count].assetName))
//						{
//							mAssetPackDic.Add(mLoadQueue[count].assetName,mLoadQueue[count]);
//							if(mLoadQueue[count].callBack!=null)
//							{
//								if(mLoadQueue[count].resourceRequest.asset!=null)
//								{
//									mLoadQueue[count].callBack.Succeed(mLoadQueue[count].resourceRequest.asset);
//								}
//								else
//								{
//									mLoadQueue[count].callBack.Failure();
//								}	
//							}
//						}
//						else
//						{
//							mAssetPackDic[mLoadQueue[count].assetName] = mLoadQueue[count];
//						}
//						mLoadQueue.RemoveAt(count);
//					}
//				}
//				else
//				{
//					mLoadQueue.RemoveAt(count);
//				}
//				count--;
//			}
//		}

		if(mLoadListIns.Count>0)
		{
			List<ResRequest> listRemove=new List<ResRequest>();
			for (int i = 0; i < mLoadListIns.Count;i++) 
			{
				ResRequest ins=mLoadListIns[i];
				if(ins.isDone)
				{
					listRemove.Add(ins);
					LoadFinish(ins);
				}
			}
			
			for (int i = 0; i < listRemove.Count; i++) 
			{
				mLoadListIns.Remove(listRemove[i]);
			}
		}
		
		while (mLoadListIns.Count<mProcessorCount  && mWaitForLoadings.Count>0) 
		{
			ResRequest request= mWaitForLoadings.Dequeue();
			mLoadListIns.Add(request);
			request.Load();
		}
	}
	
	#region resouces asset load complete
	void LoadFinish(ResRequest request)
	{
		if(request!=null)
		{
			for (int i = 0; i < request.listenings.Count; ++i) 
			{
				ILoadListening listen=request.listenings[i];
				if(listen!=null)
				{
					if(request.request!=null && request.request.asset!=null)
					{
						listen.Succeed(request.request.asset);
					}
					else
					{
						listen.Failure();
					}
				}
			}
		}

	}
	#endregion
	
	/// <summary>
	/// Using for loading
	/// </summary>
	public class ResRequest
	{
		public List<ILoadListening> listenings=new List<ILoadListening>();
		
		public ResourceRequest request;
		
		public string assetName;  //Name/Path of Asset
		public Type assetType;    //Type of asset
		public bool isKeepInMemory; //Whether need to keep in memory or not
		
		/// <summary>
		/// loaded asset
		/// </summary>
		/// <value>The asset.</value>
		public UnityEngine.Object asset
		{
			get{
				if(request!=null && request.asset!=null)
				{
					return request.asset;
				}
				else
					return null;
			}
		}
		
		/// <summary>
		/// whether is loaded asset or not
		/// </summary>
		/// <value><c>true</c> if is done; otherwise, <c>false</c>.</value>
		public bool isDone
		{
			get{
				if(request!=null)
				{
					return true;
				}
				else
					return false;
			}
		}
		
		
		public ResRequest(string _assetName,bool _isKeepInMemory, Type _assetType)
		{
			this.assetName=_assetName;
			this.assetType=_assetType;
			this.isKeepInMemory=_isKeepInMemory;
		}
		
		
		/// <summary>
		/// add one call back to the callback List
		/// </summary>
		/// <param name="iloadListening">Iload listening.</param>
		public void AddListening(ILoadListening iloadListening)
		{
			if(!listenings.Contains(iloadListening))
			{
				listenings.Add(iloadListening);
			}
		}
		
		/// <summary>
		/// Load this asset from request.
		/// </summary>
		public void Load()
		{
			if(assetType==null)
			{
				assetType=typeof(GameObject);
			}
			
			request=Resources.LoadAsync(assetName,assetType);
		}
	}
	
	#region asset load datas
	/// <summary>
	/// asset Data
	/// </summary>
	public class AssetPack
	{
		public AssetPack(Type _assetType,bool _isKeepInMemory)
		{
			this.assetType=_assetType;
			this.isKeepInMemory=_isKeepInMemory;
		}


		public Type assetType;    //Type of asset
		public bool isKeepInMemory; //Whether need to keep in memory or not
		public UnityEngine.Object asset; //asset
		
	}
	#endregion
	
	#region call back after load complete
	public interface ILoadListening
	{
		/// <summary>
		/// Succeed the specified asset.
		/// </summary>
		/// <param name="asset">Asset.</param>
		void Succeed(UnityEngine.Object asset);
		
		/// <summary>
		/// Failure load asset.
		/// </summary>
		void Failure();
	}
	#endregion
	
}
