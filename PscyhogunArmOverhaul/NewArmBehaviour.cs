using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace PscyhogunArmOverhaul;

public class NewArmBehaviour : MonoBehaviour
{
    public static NewArmBehaviour Instance { get; private set; }
    
    public CobraCharacter character;
    public Transform prostheticArmTarget;

    public RuntimeAdditiveAnimation additiveAnimation;
    public AudioSource armOffSound;
    
    private const float DPadThreshold = 0.5f;

    private bool _puttingArmBackOn;
    private bool _takingArmOff;

    private readonly Dictionary<Token.HardCodedTokens, bool> _tokens = new();
    
    private bool _prostheticOn;
    private bool _forceGrabArmModelEnabled;

    private GameObject _newFist;
    
    private void Start()
    {
        Instance = this;
        SetToken(Token.HardCodedTokens.ForcePsychogunOff, true);
        _prostheticOn = true;
        _newFist = Instantiate(character.dependencies.unskinnedProthese);
        _newFist.SetActive(false);
        Destroy(_newFist.GetComponent<ParentConstraint>());

        additiveAnimation.OnLateUpdate = DoLateUpdate;
    }

    public bool GetCanShoot()
    {
        if (_takingArmOff || _puttingArmBackOn)
            return false;
        
        return !_prostheticOn;
    }
    
    private void Update()
    {
        if (_puttingArmBackOn || _takingArmOff)
            return;

        if (_prostheticOn && TokenController.GetTokenValue(Token.HardCodedTokens.ForcePsychogunOff) <= 1)
        {
            if (Input.GetKeyDown(Plugin.KeyboardBinding.Value) || GetDPadButton(Plugin.ControllerDPadOption.Value))
            {
                StartCoroutine(TakeArmOff());
            }
        }
        else if (!_prostheticOn && TokenController.GetTokenValue(Token.HardCodedTokens.ForcePsychogunOn) <= 1)
        {
            if (Input.GetKeyDown(Plugin.KeyboardBinding.Value) || GetDPadButton(Plugin.ControllerDPadOption.Value))
            {
                StartCoroutine(PutArmBackOn());
            }
        }
    }

    private void DoLateUpdate()
    {
        _newFist.SetActive(_forceGrabArmModelEnabled);
        if (_forceGrabArmModelEnabled)
        {
            _newFist.transform.position = prostheticArmTarget.position;
            _newFist.transform.eulerAngles = prostheticArmTarget.eulerAngles;
        }
    }

    private float GetAnimationDuration(float speed)
    {
        return additiveAnimation.ClipLength / speed;
    }

    public void OnFailToShootPsychogun()
    {
        if (!_takingArmOff && !_puttingArmBackOn && _prostheticOn)
        {
            StartCoroutine(TakeArmOff(3f));
        }
    }
    
    private IEnumerator PutArmBackOn()
    {
        _puttingArmBackOn = true;
        
        PlayAdditiveAnimation();
        
        yield return new WaitForSeconds(GetAnimationDuration(1) * 0.2f);

        _forceGrabArmModelEnabled = true;
        
        yield return new WaitForSeconds(GetAnimationDuration(1) * 0.3f);

        _forceGrabArmModelEnabled = false;
        // finish putting on
        SetToken(Token.HardCodedTokens.ForcePsychogunOn, false);
        SetToken(Token.HardCodedTokens.ForcePsychogunOff, true);
        character.ProtheseOn();
        
        _prostheticOn = true;
        _puttingArmBackOn = false;
    }

    private IEnumerator TakeArmOff(float speed = 2f)
    {
        _takingArmOff = true;
        PlayAdditiveAnimation(speed, true);
        yield return new WaitForSeconds(GetAnimationDuration(speed) * 0.5f);
        
        armOffSound.Play();
        _forceGrabArmModelEnabled = true;
        // finish taking off
        SetToken(Token.HardCodedTokens.ForcePsychogunOn, true);
        SetToken(Token.HardCodedTokens.ForcePsychogunOff, false);
        
        yield return new WaitForSeconds(GetAnimationDuration(speed) * 0.3f);
        _forceGrabArmModelEnabled = false;

        _prostheticOn = false;
        _takingArmOff = false;
    }
    
    private void PlayAdditiveAnimation(float speed = 1f, bool reverse = false)
    {
        if (reverse)
            additiveAnimation.PlayInReverse(speed);
        else
            additiveAnimation.Play(speed);
    }

    private void OnDisable()
    {
        SetToken(Token.HardCodedTokens.ForcePsychogunOn, false);
        SetToken(Token.HardCodedTokens.ForcePsychogunOff, false);
        _takingArmOff = false;
        _puttingArmBackOn = false;
    }

    private void OnDestroy()
    {
        SetToken(Token.HardCodedTokens.ForcePsychogunOn, false);
        SetToken(Token.HardCodedTokens.ForcePsychogunOff, false);
    }

    private void SetToken(Token.HardCodedTokens token, bool active)
    {
        bool wasActive = _tokens.TryGetValue(token, out var storedTokenValue) && storedTokenValue;
        if (wasActive && !active)
        {
            TokenController.SetTokenValue(token, 1, Token.ValueOperator.Minus);
            _tokens[token] = false;
        }
        else if (!wasActive && active)
        {
            TokenController.SetTokenValue(token, 1, Token.ValueOperator.Add);
            _tokens[token] = true;
        }
    }

    private static bool GetDPadButton(DPadDirection dpad)
    {
        float horizontal = Input.GetAxis("6th axis"); // X
        float vertical = Input.GetAxis("7th axis"); // Y

        switch (dpad)
        {
            case DPadDirection.Up:
                return vertical > DPadThreshold;
            case DPadDirection.Down:
                return vertical < -DPadThreshold;
            case DPadDirection.Left:
                return horizontal < -DPadThreshold;
            case DPadDirection.Right:
                return horizontal > DPadThreshold;
            default:
                Plugin.Logger.LogWarning("Invalid DPadDirection: " + dpad);
                return false;
        }
    }
}