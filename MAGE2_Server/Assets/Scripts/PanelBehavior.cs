using UnityEngine;
using UnityEngine.EventSystems;

public class PanelBehavior : MonoBehaviour
{
    public Player Player;
    public Device Device;
    public TextMesh Name;
    public TextMesh Status;
    public TextMesh DeviceName;
    public GameObject HealthBar;

    public Texture2D defaultDevice;

    private GameObject _borderGlow = null;
    private GameObject _spellObject = null;
    private float _health = 100.0f;
    private EntityState _state = EntityState.Alive;
    private Vector3 _healthScale, _healthPosition;
    private Spell _activeEffect;

    private Camera _mainCamera;
    private GameController _gameController;

    private Vector3 _startPos;
    private Vector3 _mouseOrigin;
    private bool _panning;	
    

    public void SetPlayer(Player p)
    {
        Player = p;
        Name.text = Player.Name;
        Status.text = "";

        if (Player.Device != null)
        {
            Device = Player.Device;
            DeviceName.text = Player.Device.Name;
            MeshRenderer playerDevicePicture = gameObject.transform.Find("Device").GetComponent<MeshRenderer>();

            if (Device.Picture != null)
                playerDevicePicture.material.mainTexture = Device.Picture;
            else
                playerDevicePicture.material.mainTexture = defaultDevice;

            playerDevicePicture.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
        }

        MeshRenderer playerPicture = gameObject.transform.Find("Player").GetComponent<MeshRenderer>();
        playerPicture.material.mainTexture = Player.Picture;
        playerPicture.material.mainTexture.wrapMode = TextureWrapMode.Clamp;

        MeshRenderer back = gameObject.transform.Find("Back").GetComponent<MeshRenderer>();
        _gameController = GameObject.Find("Game Controller").GetComponent<GameController>();

        if (!_gameController.Legacy)
        {
            if (Game.Rules.TeamBased)
            {
                switch (Player.Team)
                {
                    case Colors.Red:
                        back.material = Resources.Load("Materials/Red") as Material;
                        break;
                    case Colors.Yellow:
                        back.material = Resources.Load("Materials/Yellow") as Material;
                        Name.color = Color.black;
                        Status.color = Color.black;
                        DeviceName.color = Color.black;
                        break;
                    case Colors.Green:
                        back.material = Resources.Load("Materials/Green") as Material;
                        break;
                    case Colors.Blue:
                        back.material = Resources.Load("Materials/Blue") as Material;
                        break;
                }
            }
        }
        _gameController.AddPlayerPanel(this);
    }

    void Start()
    {
        _mainCamera = Camera.main;
        _gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        _healthScale = HealthBar.transform.localScale;
        _healthPosition = HealthBar.transform.localPosition;
    }

    void FixedUpdate()
    {
        HealthBar.transform.localScale = Vector3.Lerp(HealthBar.transform.localScale, _healthScale, 0.15f);
        HealthBar.transform.localPosition = Vector3.Lerp(HealthBar.transform.localPosition, _healthPosition, 0.15f);

        if (Player != null)
        {
            if(Player.Device != Device)
            {
                Device = Player.Device;
                DeviceName.text = Device.Name;
                MeshRenderer playerDevicePicture = gameObject.transform.Find("Device").GetComponent<MeshRenderer>();

                if (Device.Picture != null)
                    playerDevicePicture.material.mainTexture = Device.Picture;
                else
                    playerDevicePicture.material.mainTexture = defaultDevice;

                playerDevicePicture.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
            }
            if (Player.State != _state)
            {
                _state = Player.State;
                if (_state == EntityState.Dead)
                    KillEffect();
            }
            if (Player.HealthPercent != _health)
            {
                _health = Player.HealthPercent;
                _healthScale = new Vector3(_health * 0.03f, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
                _healthPosition = new Vector3(-0.17f + (_health * 0.015f), HealthBar.transform.localPosition.y, HealthBar.transform.localPosition.z);
            }

            if (_activeEffect != Player.ActiveEffect)
            {
                KillEffect();
                _activeEffect = Player.ActiveEffect;

                if (_activeEffect != null)
                {
                    Status.text = _activeEffect.GetType().ToString();
                    switch (_activeEffect.PrimaryEffect)
                    {
                        case SpellEffect.Damage:
                            _borderGlow = GameObject.Instantiate(Resources.Load("Prefabs/RedGlow")) as GameObject;
                            break;
                        case SpellEffect.Stun:
                            _borderGlow = GameObject.Instantiate(Resources.Load("Prefabs/BlueGlow")) as GameObject;
                            break;
                        case SpellEffect.Heal:
                            _borderGlow = GameObject.Instantiate(Resources.Load("Prefabs/GreenGlow")) as GameObject;
                            break;
                    }
                    if (_borderGlow != null)
                        _borderGlow.transform.SetParent(gameObject.transform, false);

                    System.Type type = _activeEffect.GetType();
                    if (!(type == typeof(Damage) || type == typeof(Stun) || type == typeof(Heal)))
                    {
                        _spellObject = GameObject.Instantiate(Resources.Load("Prefabs/Spells/" + type)) as GameObject;
                        _spellObject.transform.SetParent(gameObject.transform, false);
                    }
                }
            }
            if(_activeEffect == null)
            {
                if (_state == EntityState.Dead)
                    Status.text = "Dead";
                else
                    Status.text = "";
            }
        }
    }

    public void KillEffect()
    {
        if (_spellObject)
        {
            _spellObject.GetComponent<IKillable>().Kill();
            _spellObject = null;
        }
        KillBorder();
    }

    public void KillBorder()
    {
        if (_borderGlow)
        {
            Destroy(_borderGlow);
            _borderGlow = null;
        }
    }

    public void UpdateBorder()
    {
        KillBorder();
        switch (Player.State)
        {
            case EntityState.Damaged:
                _borderGlow = GameObject.Instantiate(Resources.Load("Prefabs/RedGlow")) as GameObject;
                break;
            case EntityState.Stunned:
                _borderGlow = GameObject.Instantiate(Resources.Load("Prefabs/BlueGlow")) as GameObject;
                break;
            case EntityState.Healed:
                _borderGlow = GameObject.Instantiate(Resources.Load("Prefabs/GreenGlow")) as GameObject;
                break;
        }
        if(_borderGlow != null)
            _borderGlow.transform.SetParent(gameObject.transform, false);
    }


    void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3 movement = _mouseOrigin - _mainCamera.ScreenToViewportPoint(Input.mousePosition);
        if (Mathf.Abs(movement.y) < 0.025f && Mathf.Abs(movement.x) < 0.025f)
            Select();
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!_panning)
        {
            _startPos = _mainCamera.transform.position;
            _mouseOrigin = _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            if(!_gameController.PlayerSelected) _panning = true;
        }
    }

    void LateUpdate()
    {
        if (!Input.GetMouseButton(0) && _panning)
        {
            _panning = false;
        }
    }

    void OnMouseDrag()
    {
        if (_panning)
        {
            Vector3 movement = _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            movement.x = _mouseOrigin.x;
            Vector3 pos = ((_mouseOrigin - movement) * 4.0f * _mainCamera.orthographicSize) + _startPos;
            _mainCamera.GetComponent<CameraMovement>().SetPosition(pos);
        }
    }

    public void Select()
    {
        _gameController.SelectPlayer(this);
    }
}
