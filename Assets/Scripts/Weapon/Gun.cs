using UnityEngine;

public enum WonderWeaponType {
    RAYGUN,
    COILGUN,
    BOOMPISTOL
}

[CreateAssetMenu(fileName = "Gun", menuName = "Shoot Everything/Gun", order = 0)]
public class Gun : ScriptableObject {
    public string Name;
    public bool IsWonderWeapon;
    public WonderWeaponType WonderWeaponType;
    public Gun PAPedWeapon;
    public string ItemRequirementToPAP;
    public float Damage;
    public float ShootRange;
    public AnimationCurve RangeDamageDropOff;
    public int Pierce;
    public AnimationCurve PierceDamageDropOff;
    public float ShotsPerSecond;
    public int ShotsPerShot;
    public Vector2 MaxShotSpread;
    public Vector2 AimingShotSpread;
    public int AmmoPerMag;
    public int MaxAmmoReserve;
    public float ReloadTime;
    public AnimationCurve VerticalRecoilPattern;
    public AnimationCurve HorizontalRecoilPattern;
    public float OneTimeVerticalRecoil;
    public float OneTimeHorizontalRecoil;
    public bool Automatic;
    public float ExtraShotsPerSecond;
    public GameObject GunModel;
    public GameObject HitEffect;
    public GameObject ZombieHitEffect;
    public GameObject BulletTrail;
    public Sound ShootSound;
}
