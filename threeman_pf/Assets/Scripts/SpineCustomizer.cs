using Spine;
using Spine.Unity.AttachmentTools;
using Spine.Unity;
using UnityEngine;

public class SpineCustomizer : MonoBehaviour {
	public const string FRONT_HAIR_SLOT_KEY = "front_hair_pos";
	public const string BACK_HAIR_SLOT_KEY = "back_hair_pos";
	public const string EYE_SLOT_KEY = "eye_pos";
	public const string WEAPON_SLOT_KEY = "weapon_pos";
	public const string WEAPON_EFF_SLOT_KEY = "weapon_eff_pos";
	
	[SerializeField] protected bool inheritProperties = true;
	[SerializeField] protected bool useOrgSize = true;

	public SkeletonRenderer skeletonRenderer;
	
	[SpineSlot] public string availableSlot;		//스파인 내 슬롯명 체크용
	
	public SpineAtlasAsset hairAsset;
	public SpineAtlasAsset eyeAsset;
	public SpineAtlasAsset weaponAsset;

	[SpineAtlasRegion(atlasAssetField="hairAsset")] public string frontHairAtlasKey;
	[SpineAtlasRegion(atlasAssetField="hairAsset")] public string backHairAtlasKey;
	[SpineAtlasRegion(atlasAssetField="eyeAsset")] public string eyeAtlasKey;
	[SpineAtlasRegion(atlasAssetField = "weaponAsset")] public string weaponAtlasKey;
	[SpineAtlasRegion(atlasAssetField = "weaponAsset")] public string weaponEffAtlasKey;

	void Awake () {
		skeletonRenderer = GetComponent<SkeletonRenderer>();
		Apply();
	}
	
	void Apply () {
		if (!this.enabled) return;
		var frontHairSlot = skeletonRenderer.Skeleton.FindSlot(FRONT_HAIR_SLOT_KEY);
		var backHairSlot = skeletonRenderer.Skeleton.FindSlot(BACK_HAIR_SLOT_KEY);
		var eyeSlot = skeletonRenderer.Skeleton.FindSlot(EYE_SLOT_KEY);
		var weaponSlot = skeletonRenderer.Skeleton.FindSlot(WEAPON_SLOT_KEY);
		var effSlot = skeletonRenderer.Skeleton.FindSlot(WEAPON_EFF_SLOT_KEY);
		
		ApplyAsset(frontHairSlot, hairAsset, frontHairAtlasKey);
		ApplyAsset(backHairSlot, hairAsset, backHairAtlasKey);
		ApplyAsset(eyeSlot, eyeAsset, eyeAtlasKey);
		ApplyAsset(weaponSlot, weaponAsset, weaponAtlasKey);
		ApplyAsset(effSlot, weaponAsset, weaponEffAtlasKey);
	}

	void ApplyAsset(Slot targetSlot, SpineAtlasAsset atlasAset, string regionKey) {
		if (atlasAset == null) {
			Debug.LogWarning("Head Asset is Null.");
			return;
		}

		var orgAttachment = targetSlot.Attachment;
		var atlas = atlasAset.GetAtlas();
		if (atlas == null) return;
		float scale = skeletonRenderer.skeletonDataAsset.scale;
		var atlasRegion = atlas.FindRegion(regionKey);
		if (regionKey == null) {
			targetSlot.Attachment = null;
			Debug.Log(regionKey+" attachment remove");
		}else if (inheritProperties && orgAttachment != null) {
			targetSlot.Attachment = orgAttachment.GetRemappedClone(atlasRegion, true, useOrgSize, scale);
			Debug.Log(regionKey+" org attachment [" + orgAttachment.Name + "] replacement");
		} else {
			Debug.Log(regionKey+" new attachment added!");
			var newRegionAttachment = atlasRegion.ToRegionAttachment(atlasRegion.name, scale);
			targetSlot.Attachment = newRegionAttachment;
		}
	}
}

