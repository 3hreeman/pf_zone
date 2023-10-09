using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AddressableManager : MonoBehaviour {
	private static AddressableManager instance = null;
	private static Dictionary<string, AudioClip> _soundCache = new Dictionary<string, AudioClip>();
	private static Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();
	private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
	private static Dictionary<string, SpriteAtlas> _atlasCache = new Dictionary<string, SpriteAtlas>();
	private static Dictionary<SpriteAtlas, Dictionary<string, Sprite>> _atlasSpriteCache = new Dictionary<SpriteAtlas, Dictionary<string, Sprite>>();

	public static long downloadSize;
	private void Awake() {
		instance = this;
		Init();
	}
	
	IEnumerator Start() {
		DontDestroyOnLoad(this);
		PreloadSprite();
		yield return null;
	}

	public static void Init() {
		var op = Addressables.InitializeAsync();
		op.Completed += handle => { PreloadLocator(); };
	}

	//////////////////////////////////
	// catalog
	//////////////////////////////////
	public static void applyCatalog(string url) {
		Addressables.LoadContentCatalogAsync(url).Completed += catalogLoaded2;
	}

	static void catalogLoaded(AsyncOperationHandle<IResourceLocator> handle) {
		var locator = (ResourceLocationMap)handle.Result;
		List<string> abList = locator.Locations.Keys.ToList()
		                             .Where(e =>
			                                    e.GetType() == typeof(string) &&
			                                    ((string)e).StartsWith("dummy_"))
		                             .Select(e => e.ToString())
		                             .ToList();

		downloadSize = 0;
		sizeCheckQueue = new Queue<string>(abList);
		downloadList = new Queue<string>();
		Addressables.GetDownloadSizeAsync(sizeCheckQueue.Peek()).Completed += GetDownlodsize;
	}

	static void catalogLoaded2(AsyncOperationHandle<IResourceLocator> handle) {
		var locator = (ResourceLocationMap)handle.Result;
		var abList = new List<string>() { "lang_01", "lang_02" };
		downloadSize = 0;
		sizeCheckQueue = new Queue<string>(abList);
		downloadList = new Queue<string>();
		Addressables.GetDownloadSizeAsync(sizeCheckQueue.Peek()).Completed += GetDownlodsize;
	}

	public static void CheckLabelDownloaded(string key) {
		Addressables.GetDownloadSizeAsync(key).Completed += GetLabelDownloadSize;
	}

	public static void GetLabelDownloadSize(AsyncOperationHandle<long> result) {
		long size = result.Result;
	}

	static Queue<string> sizeCheckQueue;

	static void GetDownlodsize(AsyncOperationHandle<long> result) {
		long size = result.Result;

		if (size > 0) {
			downloadSize += size;
			downloadList.Enqueue(sizeCheckQueue.Peek());
		}

		sizeCheckQueue.Dequeue();
		if (sizeCheckQueue.Count == 0) {
			sizeCheckQueue = null;
			assetTotalCount = downloadList.Count;
			return;
		}

		Addressables.GetDownloadSizeAsync(sizeCheckQueue.Peek()).Completed += GetDownlodsize;
	}

	//////////////////////////////////
	// preload
	//////////////////////////////////
	static Queue<string> downloadList;
	public static HashSet<string> resourceKeys;

	public static AsyncOperationHandle curAssetAsync;
	public static int assetTotalCount = 0;
	public static int assetLeftCount = 0;
	public static bool isStartDownload = false;

	public static void Preload() {
		if (downloadList == null || downloadList.Count == 0) {
			return;
		}

		isStartDownload = true;
		assetLeftCount = downloadList.Count;
		curAssetAsync = Addressables.DownloadDependenciesAsync(downloadList.Peek());
		curAssetAsync.Completed += SinglePreload;
	}

	static void SinglePreload(AsyncOperationHandle result) {
		downloadList.Dequeue();
		if (downloadList.Count == 0) {
			downloadList = null;
			return;
		}

		float PercentComplete = result.PercentComplete;
		assetLeftCount = downloadList.Count;
		curAssetAsync = Addressables.DownloadDependenciesAsync(downloadList.Peek());
		curAssetAsync.Completed += SinglePreload;
	}

	public static void PreloadLocator() {
		resourceKeys = new HashSet<string>();
		foreach (var data in Addressables.ResourceLocators) {
			var list = data.Keys.ToList();
			for (int i = 0; i < data.Keys.Count(); i++) {
				var key = list[i].ToString();
				if (key.Contains('/') || key.Contains('.')) {
					// Debug.Log(key);
					resourceKeys.Add(key);
				}
			}
		}
	}

	// 공용 어셋 처리
	public static void LoadAsset<TObject>(string key, object param, Action<object, object> successCallback, Action<object> failCallback = null) {
		instance.StartCoroutine(procLoadAsset<TObject>(key, param, successCallback, failCallback));
	}

	static IEnumerator procLoadAsset<TObject>(string key, object param, Action<object, object> successCallback, Action<object> failCallback = null) {
		var op = Addressables.LoadAssetAsync<TObject>(key);
		yield return op;

		if (op.Status == AsyncOperationStatus.Succeeded) {
			if (successCallback != null) {
				successCallback(param, op.Result);
			}
		} else {
			if (failCallback != null) {
				failCallback(key);
			}
		}
	}

	public static IEnumerator ProcInstantiateAsync(string key, object param, Action<object, object> success_callback, Action<object> fail_callback = null) {
		var op = Addressables.InstantiateAsync(key);
		yield return op;

		if (op.Status == AsyncOperationStatus.Succeeded) {
			if (success_callback != null) {
				success_callback(param, op.Result);
			}
		} else {
			if (fail_callback != null) {
				fail_callback(key);
			}
		}
	}

	public static string GetValidKey(string key, string defaultKey) {
		if (resourceKeys != null && resourceKeys.Contains(key)) {
			return key;
		}

		return defaultKey;
	}

	public static void LoadSprite(string key, object param, Action<object, object> success_callback) {
		instance.StartCoroutine(procLoadAsset<Sprite>(key, param, success_callback));
	}

	void PreloadSprite() {
		_spriteCache = new Dictionary<string, Sprite>();
		foreach (var spr in preloadSpriteList) {
			if (spr != null) {
				_spriteCache.Add(spr.name, spr);
			}
		}

		_atlasCache = new Dictionary<string, SpriteAtlas>();
		foreach (var atlas in preloadAtlasList) {
			if (atlas != null) {
				_atlasCache.Add(atlas.name, atlas);

				// Debug.Log(atlas.name + " atlas is added");
			}
		}
	}

	public static GameObject LoadPrefab(string key) {
		if (_prefabCache.ContainsKey(key)) {
			return _prefabCache[key];
		} else {
			string res_path = key;
			var result = Resources.Load<GameObject>(res_path);
			if (result != null) {
				_prefabCache.Add(key, result);
			} else {
				Debug.LogWarning("ResourceManager :: Prefab Load Failed - " + key);
			}

			return result;
		}
	}

	public static void LoadSound(string key, Action<object> successCallback) {
		if (_soundCache.ContainsKey(key)) {
			successCallback(_soundCache[key]);
			return;
		}

		instance.StartCoroutine(LoadAudioAsset(key, successCallback));
	}

	private static IEnumerator LoadAudioAsset(string key, Action<object> success_callback, Action<object> fail_callback = null) {
		var op = Addressables.LoadAssetAsync<AudioClip>(key);
		yield return op;

		if (op.Status == AsyncOperationStatus.Succeeded) {
			if (_soundCache.ContainsKey(key) == false) {
				_soundCache.Add(key, op.Result);
			}
			if (success_callback != null) {
				success_callback(op.Result);
			}
		} else {
			if (fail_callback != null) {
				fail_callback(key);
			}
		}
	}

	public List<Sprite> preloadSpriteList;
	public List<SpriteAtlas> preloadAtlasList;

	public static Sprite GetSprite(string value) {
		if (_spriteCache.ContainsKey(value) == true) {
			return _spriteCache[value];
		}

		return null;
	}

	public static Sprite GetSpriteFromAtlas(string atlasKey, string resKey) {
		if (_atlasCache.TryGetValue(atlasKey, out var targetAtlas)) {
			if (_atlasSpriteCache.ContainsKey(targetAtlas) == false) {
				_atlasSpriteCache.Add(targetAtlas, new Dictionary<string, Sprite>());
			}

			var targetCache = _atlasSpriteCache[targetAtlas];
			if (targetCache.ContainsKey(resKey) == false) {
				var spr = targetAtlas.GetSprite(resKey);
				if (spr == null) {
					return null;
				}

				// spr.name = spr.name + "_"+((int)Time.time);
				_atlasSpriteCache[targetAtlas].Add(resKey, spr);
			}

			return _atlasSpriteCache[targetAtlas][resKey];
		} else {
			return null;
		}
	}
}