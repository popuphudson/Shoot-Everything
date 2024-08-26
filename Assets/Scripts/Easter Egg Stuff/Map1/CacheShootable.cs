using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheShootable : MonoBehaviour
{
    [SerializeField] private GameObject _keyCard;
    [SerializeField] private Vector3[] _cachePositions;
    [SerializeField] private Vector3[] _cacheRotations;
    [SerializeField] private Vector3[] _cacheCardPositions;
    private int _cacheIndex;
    private bool _locked = true;
    private bool _flingCard = false;
    private float _timer;
    private Vector3 _keyCardStartPos;

    private void Start() {
        _cacheIndex = Random.Range(0, _cachePositions.Length);
        transform.localPosition = _cachePositions[_cacheIndex];
        transform.localEulerAngles = _cacheRotations[_cacheIndex];
        _keyCardStartPos = _keyCard.transform.localPosition;
    }

    public void Unlock() {
        _locked = false;
        _keyCard.SetActive(true);
    }

    private void Update() {
        if(_flingCard) {
            _timer = Mathf.Min(_timer+(Time.deltaTime/4f), 1);
            _keyCard.transform.localPosition = Parabola(_keyCardStartPos, _cacheCardPositions[_cacheIndex], 2, _timer);
            _keyCard.transform.localEulerAngles = Vector3.Lerp(new Vector3(90, 0, 0), Vector3.zero, _timer);
        }
    }

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Explosion>() && !_locked) {
            _flingCard = true;
            _timer = 0;
        }
    }
}
