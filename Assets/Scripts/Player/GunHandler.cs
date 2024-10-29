using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GunHandler : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private PauseMenu _pauser;
    [SerializeField] private Transform _gunHoldPoint;
    [SerializeField] private LayerMask _solidLayers;
    [SerializeField] private LayerMask _enemyHealthLayer;
    [SerializeField] private LayerMask _enemySolidLayer;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private List<Gun> _guns = new List<Gun>();
    [SerializeField] private PlayerPoints _playerPoints;
    [SerializeField] private TextMeshProUGUI _ammoCounter;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private PlayerPerks _playerPerks;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private float _meleeDamage;
    [SerializeField] private float _meleeRange;
    [SerializeField] private Animator _anims;
    [SerializeField] private PowerUpManager _powerUpManager;
    [SerializeField] private GameObject _meleeZombieHitEffect;
    [SerializeField] private float _gunMoveSmoothness;
    [SerializeField] private float _gunRotSmoothness;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private GlobalSettingsManager _settings;
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _throwForce;
    [SerializeField] private GameObject _grenadeUIPrefab;
    [SerializeField] private Transform _grenadeUI;
    [SerializeField] private Camera _weaponCamera;
    [SerializeField] private float _aimTime;
    private List<int> _magAmmo = new List<int>();
    private List<int> _spareAmmo = new List<int>();
    private int _selectedIndex;
    private Gun _selectedGun;
    private float _timePassed;
    private bool _reloading;
    private float _shootingTime;
    private Vector2 _preRecoil = Vector2.zero;
    private bool _meleeing;
    private Vector3 _gunHoldPointStart;
    private Vector3 _gunHoldPointPosition;
    private Vector3 _gunHoldPointRotation;
    private InputAction _fireInput;
    private InputAction _reloadInput;
    private InputAction _switchWeaponInput;
    private InputAction _meleeInput;
    private InputAction _lookInput;
    private InputAction _moveInput;
    private InputAction _grenadeInput;
    private InputAction _aimInput;
    private bool _gamepadScrolled;
    private int _numOfGrenades;
    private bool _aiming;
    private bool _startingReload;
    

    private void Start() {
        _numOfGrenades = 1;
        for(int i=0; i<_numOfGrenades; i++) {
            Instantiate(_grenadeUIPrefab, _grenadeUI);
        }
        _reloading = false;
        foreach(Gun gun in _guns) {
            _magAmmo.Add(gun.AmmoPerMag);
            _spareAmmo.Add(gun.MaxAmmoReserve/2);
        }
        SwapGun();
        _meleeing = false;
        _gunHoldPointStart = _gunHoldPoint.localPosition;
        _fireInput = _playerInput.actions["Fire"];
        _reloadInput = _playerInput.actions["Reload"];
        _switchWeaponInput = _playerInput.actions["SwitchWeapon"];
        _meleeInput = _playerInput.actions["Melee"];
        _lookInput = _playerInput.actions["Look"];
        _moveInput = _playerInput.actions["Move"];
        _grenadeInput = _playerInput.actions["Grenade"];
        _aimInput = _playerInput.actions["Aim"];
    }

    public Gun GetSelectedGun() {
        return _selectedGun;
    }

    public void RemoveGun(Gun __gun) {
        _magAmmo.RemoveAt(_guns.IndexOf(__gun));
        _spareAmmo.RemoveAt(_guns.IndexOf(__gun));
        _guns.Remove(__gun);
        if(_selectedIndex >= _guns.Count) {
            _selectedIndex = 0;
        }
        SwapGun();
    }

    public void OnNextWave() {
        AddGrenades(1);
    }

    public int GetNumberOfGuns() {
        return _guns.Count;
    }

    public bool HasGun(Gun __gun) {
        return _guns.Contains(__gun);
    }

    public bool GunAtFullAmmo(Gun __gun) {
        int index = _guns.IndexOf(__gun);
        return _magAmmo[index] == __gun.AmmoPerMag && _spareAmmo[index] == __gun.MaxAmmoReserve;
    }

    public void AddGun(Gun __gun) {
        if(_guns.Count == 2) {
            _guns[_selectedIndex] = __gun;
            _magAmmo[_selectedIndex] = __gun.AmmoPerMag;
            _spareAmmo[_selectedIndex] = __gun.MaxAmmoReserve;
        } else {
            _guns.Add(__gun);
            _selectedIndex = _guns.Count-1;
            _magAmmo.Add(__gun.AmmoPerMag);
            _spareAmmo.Add(__gun.MaxAmmoReserve);
        }
        SwapGun();
    }

    public void AddAmmo(Gun __gun) {
        int index = _guns.IndexOf(__gun);
        _magAmmo[index] = __gun.AmmoPerMag;
        _spareAmmo[index] = __gun.MaxAmmoReserve;
    }

    private void Update() {
        if(_pauser.Paused) return;
        _timePassed += Time.deltaTime;
        if(_switchWeaponInput.ReadValue<float>() < 0 && !_meleeing && !_gamepadScrolled) {
            _selectedIndex += 1;
            if(_selectedIndex >= _guns.Count) {
                _selectedIndex = 0;
            }
            if(_playerInput.currentControlScheme != "Keyboard And Mouse") _gamepadScrolled = true;
            SwapGun();
        }
        if(_switchWeaponInput.ReadValue<float>() > 0 && !_meleeing && !_gamepadScrolled) {
            _selectedIndex -= 1;
            if(_selectedIndex < 0) {
                _selectedIndex = _guns.Count-1;
            }
            if(_playerInput.currentControlScheme != "Keyboard And Mouse") _gamepadScrolled = true;
            SwapGun();
        }
        if(_switchWeaponInput.ReadValue<float>() == 0) _gamepadScrolled = false;
        if(_meleeInput.WasPressedThisFrame() && !_meleeing && (!_playerMovement.IsRunning() || _playerPerks.HasPerks(Perks.BETTER_RUN))) {
            if(_reloading) {
                _anims.speed = 1;
                _reloading = false;
                StopAllCoroutines();
            }
            _aiming = false;
            _anims.Play("Melee");
            _meleeing = true;
        }
        if(_selectedGun && !_meleeing && !_startingReload && !_reloading && (!_playerMovement.IsRunning() || _playerPerks.HasPerks(Perks.BETTER_RUN)) && _selectedGun) {
            TryFireGun();
            TryReload();
        }
        if(_selectedGun) {
            UpdateAmmoDisplay();
            CheckHealthBars();
        } else {
            ClearAmmoDisplay();
        }
        if(!_reloading && !_meleeing && !_playerMovement.IsRunning()) {
            if(_grenadeInput.WasPressedThisFrame() && _numOfGrenades > 0) {
                ThrowGrenade();
            }
        }
        if(!_meleeing && !_reloading && !_playerMovement.IsRunning() && !_startingReload) {
            _aiming = _aimInput.IsPressed();
        } else {
            _aiming = false;
        }
        if(_aiming) {
            AimIn();
        } else {
            AimOut();
        }
    }

    private void AimIn() {
        _weaponCamera.fieldOfView = Mathf.Lerp(_weaponCamera.fieldOfView, 30, Time.deltaTime*_aimTime);
    }

    private void AimOut() {
        _weaponCamera.fieldOfView = Mathf.Lerp(_weaponCamera.fieldOfView, 90, Time.deltaTime*_aimTime);
    }

    public bool GetAiming() {
        return _aiming;
    }

    public void AddGrenades(int __num) {
        int addedGrenades = Mathf.Clamp(_numOfGrenades+__num, 0, 4)-_numOfGrenades;
        for(int i=0; i<addedGrenades; i++) {
            Instantiate(_grenadeUIPrefab, _grenadeUI);
        }
        _numOfGrenades = Mathf.Clamp(_numOfGrenades+__num, 0, 4);
    }

    private void ThrowGrenade() {
        _numOfGrenades--;
        Destroy(_grenadeUI.GetChild(0).gameObject);
        Rigidbody grenadeRB = Instantiate(_grenadePrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        grenadeRB.GetComponent<Grenade>().SetData(_playerPoints, _powerUpManager, _audioManager);
        grenadeRB.AddForce(transform.forward*_throwForce, ForceMode.Impulse);
    }

    private void LateUpdate() {
        if(!_aiming) {
            Vector3 changeInPos = ((Vector3.forward*_moveInput.ReadValue<Vector2>().y)+(Vector3.right*_moveInput.ReadValue<Vector2>().x))*(_playerMovement.IsRunning()?(_playerPerks.HasPerks(Perks.BETTER_RUN)?2.5f:2):1);
            changeInPos.Normalize();
            if(_startingReload) changeInPos = Vector3.zero;
            _gunHoldPointPosition = Vector3.Lerp(_gunHoldPointPosition, _gunHoldPointStart-(changeInPos/20), Time.deltaTime*_gunMoveSmoothness);
            if(!_meleeing && !_reloading) _gunHoldPoint.localPosition = _gunHoldPointPosition;
            Vector3 changeInRot = new Vector3(Mathf.Clamp(-_lookInput.ReadValue<Vector2>().y*_settings.MouseSensitivity.y, -5f, 5f), Mathf.Clamp(_lookInput.ReadValue<Vector2>().x*_settings.MouseSensitivity.x, -5f, 5f), 0);
            if(_startingReload) changeInRot = Vector3.zero;
            _gunHoldPointRotation = Vector3.Lerp(_gunHoldPointRotation, changeInRot, Time.deltaTime*_gunRotSmoothness);
            if(!_meleeing && !_reloading) _gunHoldPoint.localEulerAngles = _gunHoldPointRotation;
        } else {
            _gunHoldPointPosition = Vector3.Lerp(_gunHoldPointPosition, new Vector3(0,-0.15f, 1), Time.deltaTime*_aimTime);
            _gunHoldPoint.localPosition = _gunHoldPointPosition;
            Vector3 changeInRot = new Vector3(Mathf.Clamp(-_lookInput.ReadValue<Vector2>().y*_settings.MouseSensitivity.y, -1f, 1f), Mathf.Clamp(_lookInput.ReadValue<Vector2>().x*_settings.MouseSensitivity.x, -1f, 1f), 0);
            if(_startingReload) changeInRot = Vector3.zero;
            _gunHoldPointRotation = Vector3.Lerp(_gunHoldPointRotation, changeInRot, Time.deltaTime*_gunRotSmoothness);
            _gunHoldPoint.localEulerAngles = _gunHoldPointRotation;
        }
    }

    private void TryReload() {
        if(_reloadInput.WasPressedThisFrame() && !_startingReload && !_reloading && _magAmmo[_selectedIndex] != _selectedGun.AmmoPerMag && _spareAmmo[_selectedIndex] > 0) {
            StartCoroutine(Reload());
        }
    }

    private void ClearAmmoDisplay() {
        _ammoCounter.text = "";
    }

    private void UpdateAmmoDisplay() {
        _ammoCounter.text = $"{_selectedGun.Name}: {_magAmmo[_selectedIndex]}/{_spareAmmo[_selectedIndex]}";
    }

    private void CheckHealthBars() {
        RaycastHit hit;
        if(!GlobalSettingsManager.Instance.ShowHealthBars) return;
        if(Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, _selectedGun.ShootRange, _enemySolidLayer)) {
            ShootableHealthRelay shot = hit.transform.GetComponent<ShootableHealthRelay>();
            if(shot) {
                shot.ShowHealth();
            }
        }
        else if(Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, _selectedGun.ShootRange, _enemyHealthLayer)) {
            ShootableHealthRelay shot = hit.transform.GetComponent<ShootableHealthRelay>();
            if(shot) {
                shot.ShowHealth();
            }
        }
    }

    public void StopMeleeing() {
        _meleeing = false;
        if(_guns.Count == 0) return;
        if(_magAmmo[_selectedIndex] == 0 && _spareAmmo[_selectedIndex] > 0) {
            StartCoroutine(Reload());
        }
    }

    private void TryFireGun() {
        if(_selectedGun.Automatic) {
            if(_fireInput.IsPressed() && _timePassed > (1/(_selectedGun.ShotsPerSecond+(_playerPerks.HasPerks(Perks.FAST_RELOAD)?_selectedGun.ExtraShotsPerSecond:0)))) {
                FireGun();
            } else if(_timePassed > (1/(_selectedGun.ShotsPerSecond+(_playerPerks.HasPerks(Perks.FAST_RELOAD)?_selectedGun.ExtraShotsPerSecond:0)))) {
                _shootingTime = Mathf.Max(_shootingTime-Time.deltaTime, 0);
                float recoilStep = _shootingTime/(_selectedGun.AmmoPerMag/_selectedGun.ShotsPerSecond);
                _preRecoil = new Vector2(_selectedGun.VerticalRecoilPattern.Evaluate(recoilStep), _selectedGun.HorizontalRecoilPattern.Evaluate(recoilStep));
            }
        } else {
            _shootingTime = 0;
            if(_fireInput.WasPressedThisFrame() && _timePassed > (1/(_selectedGun.ShotsPerSecond+(_playerPerks.HasPerks(Perks.FAST_RELOAD)?_selectedGun.ExtraShotsPerSecond:0)))) {
                FireGun();
            }
        }
    }

    private IEnumerator Reload() {
        _startingReload = true;
        while(Vector3.Distance(_gunHoldPointPosition, _gunHoldPointStart) > 0.2f || Vector3.Distance(_gunHoldPointRotation, Vector3.zero) > 1f) {
            yield return null;
        }
        _startingReload = false;
        _reloading = true;
        _preRecoil = new Vector2(_selectedGun.VerticalRecoilPattern.Evaluate(0), _selectedGun.HorizontalRecoilPattern.Evaluate(0));
        float reloadDiv = _playerPerks.HasPerks(Perks.FAST_RELOAD)?2:(_playerPerks.HasSideMixPerk(Perks.FAST_RELOAD)?1.5f:1);
        reloadDiv *= _playerPerks.HasMix(Perks.FAST_RELOAD, Perks.EXTRA_HEALTH)?_playerHealth.GetHealth()/100f:1;
        float reloadTime = _selectedGun.ReloadTime/reloadDiv;
        _anims.speed = 1/reloadTime;
        _anims.CrossFade("Reload", 0.05f);
        yield return new WaitForSeconds(reloadTime);
        _reloading = false;
        if(_playerPerks.HasMix(Perks.QUICK_HEAL_LIFE, Perks.FAST_RELOAD)) _playerHealth.Heal(50);
        int dif = _selectedGun.AmmoPerMag - _magAmmo[_selectedIndex];
        if(_spareAmmo[_selectedIndex] - dif >= 0) {
            _magAmmo[_selectedIndex] = _selectedGun.AmmoPerMag;
            _spareAmmo[_selectedIndex] -= dif;
        } else {
            _magAmmo[_selectedIndex] += _spareAmmo[_selectedIndex];
            _spareAmmo[_selectedIndex] = 0;
        }
        _anims.Play("Idle");
        _anims.speed = 1;
        _gunHoldPointPosition = _gunHoldPointStart;
        _gunHoldPointRotation = Vector3.zero;
    }

    private void FireGun() {
        if(_reloading) {
            _shootingTime = 0;
            return;
        }
        if(_spareAmmo[_selectedIndex] <= 0 && _magAmmo[_selectedIndex] <= 0) {
            int presel = _selectedIndex;
            _selectedIndex++;
            if(_selectedIndex >= _guns.Count) {
                _selectedIndex = 0;
            }
            if(_spareAmmo[_selectedIndex] <= 0 && _magAmmo[_selectedIndex] <= 0) {
                _selectedIndex = presel;
            } else {
                SwapGun();
            }
            _shootingTime = 0;
            return;
        }
        if(_magAmmo[_selectedIndex] <= 0 && _spareAmmo[_selectedIndex] > 0) {
            StartCoroutine(Reload());
            _shootingTime = 0;
            return;
        }
        _shootingTime += 1/(_selectedGun.ShotsPerSecond+(_playerPerks.HasPerks(Perks.FAST_RELOAD)?_selectedGun.ExtraShotsPerSecond:0));
        _magAmmo[_selectedIndex] -= 1;
        _timePassed = 0;
        _audioManager.PlaySound(_selectedGun.ShootSound);
        int extraTotalShots = _playerPerks.HasPerks(Perks.EXTRA_OVERALL_DAMAGE)?(_selectedGun.IsWonderWeapon?1:2):1;
        extraTotalShots += _playerPerks.HasMix(Perks.EXTRA_OVERALL_DAMAGE, Perks.EXTRA_HEALTH)?Mathf.FloorToInt(_playerHealth.GetHealth()/50):0;
        for(int j=0;j<extraTotalShots;j++) {
            for(int i = 0; i < _selectedGun.ShotsPerShot; i++) {
                Vector2 spread;
                if(!_aiming) spread = new Vector2(Random.Range(-_selectedGun.MaxShotSpread.x, _selectedGun.MaxShotSpread.x), Random.Range(-_selectedGun.MaxShotSpread.y, _selectedGun.MaxShotSpread.y));
                else spread = new Vector2(Random.Range(-_selectedGun.AimingShotSpread.x, _selectedGun.AimingShotSpread.x), Random.Range(-_selectedGun.AimingShotSpread.y, _selectedGun.AimingShotSpread.y));
                _playerCamera.Rotate(spread.y, spread.x, 0);
                ShootRay();
                _playerCamera.Rotate(-spread.y, -spread.x, 0);
            }
        }
        KickCamera();
        if(_magAmmo[_selectedIndex] <= 0 && _spareAmmo[_selectedIndex] > 0) {
            StartCoroutine(Reload());
            _shootingTime = 0;
        }
    }

    private float GetGunDamage(Vector3 __hitPos, int __pierce) {
        float totalDamage = _selectedGun.Damage;
        totalDamage *= _selectedGun.RangeDamageDropOff.Evaluate(Vector3.Distance(__hitPos, _playerCamera.position)/_selectedGun.ShootRange)*_selectedGun.PierceDamageDropOff.Evaluate(__pierce);
        if(_playerPerks.HasMix(Perks.EXTRA_OVERALL_DAMAGE, Perks.FAST_RELOAD)) totalDamage *= (_magAmmo[_selectedIndex] >= Mathf.FloorToInt(_selectedGun.AmmoPerMag*0.75f))?3:1;
        if(_selectedGun.IsWonderWeapon) totalDamage *= 2f;
        if(_powerUpManager.IsPowerupActive(PowerupType.INSTAKILL)) totalDamage = -1f;
        return totalDamage;
    }

    private void MeleeRay() {
        RaycastHit hit;
        if(Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, _meleeRange, _solidLayers)) {
            ShootableRelay shot = hit.transform.GetComponent<ShootableRelay>();
            if(shot) {
                if(_powerUpManager.IsPowerupActive(PowerupType.INSTAKILL)) shot.TakeDamage(-1, _playerPoints, true, _powerUpManager);
                else shot.TakeDamage(_meleeDamage, _playerPoints, true, _powerUpManager);
                _playerPoints.AddPoints(10);
                GameObject effect = Instantiate(_meleeZombieHitEffect, hit.point, Quaternion.identity);
                Destroy(effect, 5f);
            }
        }
    }

    private void HandleZombieHit(RaycastHit __hit, ShootableRelay __shot) {
        __shot.TakeDamage(GetGunDamage(__hit.point, 0), _playerPoints, false, _powerUpManager);
        _playerPoints.AddPoints(10);
        if(_selectedGun.IsWonderWeapon && _selectedGun.WonderWeaponType == WonderWeaponType.COILGUN) {
            CoilGunStunner effect = Instantiate(_selectedGun.ZombieHitEffect, __hit.point, Quaternion.identity).GetComponent<CoilGunStunner>();
            effect.SetData(_playerPoints, _powerUpManager);
        } else if(_selectedGun.IsWonderWeapon && (_selectedGun.WonderWeaponType == WonderWeaponType.BOOMPISTOL || _selectedGun.WonderWeaponType == WonderWeaponType.RAYGUN)) {
            Explosion effect = Instantiate(_selectedGun.ZombieHitEffect, __hit.point, Quaternion.identity).GetComponent<Explosion>();
            effect.SetData(_playerPoints, _powerUpManager, _audioManager, _selectedGun.Damage);
        } else {
            GameObject effect = Instantiate(_selectedGun.ZombieHitEffect, __hit.point, Quaternion.identity);
            effect.transform.forward = __hit.normal;
            Destroy(effect, 5f);
        }
    }

    private void HandleObjectHit(RaycastHit __hit) {
        if(_selectedGun.IsWonderWeapon && _selectedGun.WonderWeaponType == WonderWeaponType.COILGUN) {
            CoilGunStunner effect = Instantiate(_selectedGun.HitEffect, __hit.point, Quaternion.identity).GetComponent<CoilGunStunner>();
            effect.SetData(_playerPoints, _powerUpManager);
            Destroy(effect, 5f);
        } else if(_selectedGun.IsWonderWeapon && (_selectedGun.WonderWeaponType == WonderWeaponType.BOOMPISTOL || _selectedGun.WonderWeaponType == WonderWeaponType.RAYGUN)) {
            Explosion effect = Instantiate(_selectedGun.HitEffect, __hit.point, Quaternion.identity).GetComponent<Explosion>();
            effect.SetData(__playerPoints: _playerPoints, _powerUpManager, _audioManager,_selectedGun.Damage);
        } else {
            GameObject effect = Instantiate(_selectedGun.HitEffect, __hit.point, Quaternion.identity);
            effect.transform.forward = __hit.normal;
            Destroy(effect, 5f);
        }
    }

    private IEnumerator DoBulletTrail(RaycastHit __hit) {
        GameObject bulletTrail = Instantiate(_selectedGun.BulletTrail, _gunHoldPoint.GetChild(0).GetChild(0).position, Quaternion.identity);
        yield return null;
        bulletTrail.transform.position = __hit.point;
    }

    private void ShootRay() {
        RaycastHit hit;
        Collider collider;
        if(!Physics.Raycast(_playerCamera.position, _playerCamera.forward, out hit, _selectedGun.ShootRange, _solidLayers)) return;
        ShootableRelay shot = hit.transform.GetComponent<ShootableRelay>();
        collider = hit.transform.GetComponent<Collider>();
        StartCoroutine(DoBulletTrail(hit));
        if(shot) {
            HandleZombieHit(hit, shot);
        } else {
            HandleObjectHit(hit);
            return;
        }
        for(int i = 0; i < _selectedGun.Pierce; i++) {
            if(!hit.transform) break;
            collider.enabled = false;
            if(Physics.Raycast(hit.point, _playerCamera.forward, out hit, _selectedGun.ShootRange, _solidLayers)) {
                collider.enabled = true;
                ShootableRelay Pierceshot = hit.transform.GetComponent<ShootableRelay>();
                collider = hit.transform.GetComponent<Collider>();
                StartCoroutine(DoBulletTrail(hit));
                if(Pierceshot) {
                    _playerPoints.AddPoints(10);
                    KillNotification.Notification("10");
                    Pierceshot.TakeDamage(GetGunDamage(hit.point, i+1), _playerPoints, false, _powerUpManager);
                    GameObject effect = Instantiate(_selectedGun.ZombieHitEffect, hit.point, Quaternion.identity);
                    Destroy(effect, 5f);
                } else {
                    HandleObjectHit(hit);
                    break;
                }
            } else {
                collider.enabled = true;
            }
        }
        collider.enabled = true;
    }

    private void KickCamera() {
        if(_selectedGun.Automatic) {
            float recoilStep = _shootingTime/(_selectedGun.AmmoPerMag/_selectedGun.ShotsPerSecond);
            Vector2 currentRecoil = new Vector2(_selectedGun.VerticalRecoilPattern.Evaluate(recoilStep), _selectedGun.HorizontalRecoilPattern.Evaluate(recoilStep));
            Vector2 addedRecoil = new Vector2(-(currentRecoil.x-_preRecoil.x),currentRecoil.y-_preRecoil.y)+new Vector2(0.5f*Random.Range(-1f,1f), 0.5f*Random.Range(-1f, 1f));
            _playerLook.AddRecoil(addedRecoil);
            if(!_aiming) _gunHoldPointRotation += new Vector3(addedRecoil.x, addedRecoil.y, 0)*2;
            else _gunHoldPointPosition -= new Vector3(0, 0, addedRecoil.magnitude/10f);
            _preRecoil = currentRecoil;
        } else {
            Vector2 addedRecoil = new Vector2(-_selectedGun.OneTimeVerticalRecoil,Random.Range(-1,2)*_selectedGun.OneTimeHorizontalRecoil);
            _playerLook.AddReversibleRecoil(addedRecoil);
            if(!_aiming) _gunHoldPointRotation += new Vector3(addedRecoil.x, addedRecoil.y, 0)*2;
            else _gunHoldPointPosition -= new Vector3(0, 0, addedRecoil.magnitude/10f);
        }
    }

    private void SwapGun() {
        for(int i = 0; i < _gunHoldPoint.childCount; i++) {
            Destroy(_gunHoldPoint.GetChild(i).gameObject);
        }
        _aiming = false;
        _reloading = false;
        _startingReload = false;
        _meleeing = false;
        StopAllCoroutines();
        _anims.speed = 1;
        _anims.Play("Idle");
        _timePassed = 0;
        if(_guns.Count == 0) _selectedGun = null;
        else {
            _selectedGun = _guns[_selectedIndex];
            Transform shownGun = Instantiate(_selectedGun.GunModel, _gunHoldPoint).transform;
            Transform[] allChildren = shownGun.GetComponentsInChildren<Transform>();
            foreach(Transform child in allChildren) {
                child.gameObject.layer = 9;
            }
        }
    }

    public void RefillAllAmmo() {
        foreach(Gun gun in _guns) {
            AddAmmo(gun);
        }
    }

}
