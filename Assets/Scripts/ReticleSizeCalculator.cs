using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReticleSizeCalculator : MonoBehaviour {
    private RectTransform _rectTransform;
    private Image[] _reticleImages;

    private void Start() {
        _rectTransform = GetComponent<RectTransform>();
        _reticleImages = transform.GetComponentsInChildren<Image>();
    }

    private Vector2 UIErrorSizeToFireErrorAndAverageDistance(Vector2 __maxFireError, float __distance) {
        Vector2 uiReticleSize = Vector2.zero;
        if(__maxFireError.x != 0) uiReticleSize.x = Mathf.Cos(Mathf.Deg2Rad*(90-__maxFireError.x))*__distance;
        if(__maxFireError.y != 0) uiReticleSize.y = Mathf.Cos(Mathf.Deg2Rad*(90-__maxFireError.y))*__distance;
        return uiReticleSize;
    }

    public void UpdateReticleForGun(Gun __currentGun) {
        if(__currentGun == null) {
            _rectTransform.sizeDelta = Vector2.zero;
            return;
        }
        Vector2 reticleSize = UIErrorSizeToFireErrorAndAverageDistance(__currentGun.MaxShotSpread, __currentGun.ShootRange);
        _rectTransform.sizeDelta = reticleSize*30;
    }

    public void ShowReticle() {
        foreach(Image image in _reticleImages) {
            image.color = new Color(0, 0, 0, 0.25f);
        }
    }

    public void HideReticle() {
        foreach(Image image in _reticleImages) {
            image.color = new Color(0, 0, 0, 0);
        }
    }
}
