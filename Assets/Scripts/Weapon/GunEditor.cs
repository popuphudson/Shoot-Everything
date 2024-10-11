#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor {
    public override void OnInspectorGUI() {
        SerializedProperty damage = serializedObject.FindProperty("Damage");
        SerializedProperty shotsPerSecond = serializedObject.FindProperty("ShotsPerSecond");
        SerializedProperty shootRange = serializedObject.FindProperty("ShootRange");
        SerializedProperty ammoPerMag = serializedObject.FindProperty("AmmoPerMag");
        SerializedProperty maxAmmoReserve = serializedObject.FindProperty("MaxAmmoReserve");
        SerializedProperty reloadTime = serializedObject.FindProperty("ReloadTime");
        SerializedProperty pierce = serializedObject.FindProperty("Pierce");
        SerializedProperty verticalRecoilPattern = serializedObject.FindProperty("VerticalRecoilPattern");
        SerializedProperty horizontalRecoilPattern = serializedObject.FindProperty("HorizontalRecoilPattern");
        SerializedProperty oneTimeVerticalRecoil = serializedObject.FindProperty("OneTimeVerticalRecoil");
        SerializedProperty oneTimeHorizontalRecoil = serializedObject.FindProperty("OneTimeHorizontalRecoil");
        SerializedProperty automatic = serializedObject.FindProperty("Automatic");
        SerializedProperty gunModel = serializedObject.FindProperty("GunModel");
        SerializedProperty hitEffect = serializedObject.FindProperty("HitEffect");
        SerializedProperty zombieHitEffect = serializedObject.FindProperty("ZombieHitEffect");
        SerializedProperty rangeDamageDropOff = serializedObject.FindProperty("RangeDamageDropOff");
        SerializedProperty pierceDamageDropOff = serializedObject.FindProperty("PierceDamageDropOff");
        SerializedProperty shotsPerShot = serializedObject.FindProperty("ShotsPerShot");
        SerializedProperty maxShotSpread = serializedObject.FindProperty("MaxShotSpread");
        SerializedProperty isWonderWeapon = serializedObject.FindProperty("IsWonderWeapon");
        SerializedProperty wonderWeaponType = serializedObject.FindProperty("WonderWeaponType");
        SerializedProperty extraShotsPerSecond = serializedObject.FindProperty("ExtraShotsPerSecond");
        SerializedProperty isPAPed = serializedObject.FindProperty("IsPAPed");
        SerializedProperty pAPedWeapon = serializedObject.FindProperty("PAPedWeapon");
        SerializedProperty name = serializedObject.FindProperty("Name");
        SerializedProperty shootSound = serializedObject.FindProperty("ShootSound");
        SerializedProperty aimingShotSpread = serializedObject.FindProperty("AimingShotSpread");

        name.stringValue = EditorGUILayout.TextField("Name", name.stringValue);
        isWonderWeapon.boolValue = EditorGUILayout.Toggle("Is Wonder Weapon", isWonderWeapon.boolValue);
        if(isWonderWeapon.boolValue) EditorGUILayout.PropertyField(wonderWeaponType);
        isPAPed.boolValue = EditorGUILayout.Toggle("Is Pack A Punched", isPAPed.boolValue);
        if(!isPAPed.boolValue) pAPedWeapon.objectReferenceValue = EditorGUILayout.ObjectField("Pack A Punched Weapon", pAPedWeapon.objectReferenceValue, typeof(Gun), false);
        damage.floatValue = EditorGUILayout.FloatField("Damage", damage.floatValue);
        shootRange.floatValue = EditorGUILayout.FloatField("Shoot Range", shootRange.floatValue);
        rangeDamageDropOff.animationCurveValue = EditorGUILayout.CurveField("Range Damage Drop Off", rangeDamageDropOff.animationCurveValue, Color.green, new Rect(0, 0, 1, 1));
        pierce.intValue = EditorGUILayout.IntField("Pierce", pierce.intValue);
        pierceDamageDropOff.animationCurveValue = EditorGUILayout.CurveField("Pierce Damage Drop Off", pierceDamageDropOff.animationCurveValue, Color.green, new Rect(0, 0, pierce.intValue, 1));
        shotsPerShot.intValue = EditorGUILayout.IntField("Shots Per Shot", shotsPerShot.intValue);
        maxShotSpread.vector2Value = EditorGUILayout.Vector2Field("Max Shot Spread", maxShotSpread.vector2Value);
        aimingShotSpread.vector2Value = EditorGUILayout.Vector2Field("Aiming Shot Spread", aimingShotSpread.vector2Value);
        shotsPerSecond.floatValue = EditorGUILayout.FloatField("Shots Per Second", shotsPerSecond.floatValue);
        extraShotsPerSecond.floatValue = EditorGUILayout.FloatField("Extra Shots Per Second", extraShotsPerSecond.floatValue);
        ammoPerMag.intValue = EditorGUILayout.IntField("Ammo Per Mag", ammoPerMag.intValue);
        maxAmmoReserve.intValue = EditorGUILayout.IntField("Max Ammo Reserve", maxAmmoReserve.intValue);
        reloadTime.floatValue = EditorGUILayout.FloatField("Reload Time", reloadTime.floatValue);
        automatic.boolValue = EditorGUILayout.Toggle("Automatic", automatic.boolValue);
        if(automatic.boolValue) {
            verticalRecoilPattern.animationCurveValue = EditorGUILayout.CurveField("Vertical Recoil Pattern", verticalRecoilPattern.animationCurveValue, Color.green, new Rect(0, 0, 1, 30));
            horizontalRecoilPattern.animationCurveValue = EditorGUILayout.CurveField("Horizontal Recoil Pattern", horizontalRecoilPattern.animationCurveValue, Color.green, new Rect(0, -15, 1, 30));
        } else {
            oneTimeVerticalRecoil.floatValue = EditorGUILayout.FloatField("Vertical Recoil", oneTimeVerticalRecoil.floatValue);
            oneTimeHorizontalRecoil.floatValue = EditorGUILayout.FloatField("Random Horizontal Recoil", oneTimeHorizontalRecoil.floatValue);
        }
        gunModel.objectReferenceValue = EditorGUILayout.ObjectField("Gun Model", gunModel.objectReferenceValue, typeof(GameObject), true);
        hitEffect.objectReferenceValue = EditorGUILayout.ObjectField("Hit Effect", hitEffect.objectReferenceValue, typeof(GameObject), true);
        zombieHitEffect.objectReferenceValue = EditorGUILayout.ObjectField("Zombie Hit Effect", zombieHitEffect.objectReferenceValue, typeof(GameObject), true);
        EditorGUILayout.PropertyField(shootSound);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif