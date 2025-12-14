using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mirror : MonoBehaviour, IRotatable
{
    [SerializeField]
    private Transform _customMagicLinePivot;
    public Transform CustomMagicLinePivot { get => _customMagicLinePivot != null ? _customMagicLinePivot : transform; set => _customMagicLinePivot = value; }

    private enum MirrorDirection : int
    {
        UpLeft = 0,
        UpRight = 1,
        DownRight = 2,
        DownLeft = 3
    }

    [SerializeField] private MirrorDirection currentMirrorDirection = MirrorDirection.UpLeft;
    public CapsuleCollider2D capsuleCollider = null;
    //private Animator animator = null;
    //private Vector3 lightOffset = Vector3.zero;// Vector3.up * 0.5f;

    [Header("Animation Vars")]
    public SpriteRenderer spriteRenderer;
    private int activeState = 0;
    public int framesPerState = 2;
    public float animationTime = 0.2f;
    public List<Sprite> spriteSheet;
    private Sequence rotationSequence = null;

    private List<MirrorableLight> inputLights = new List<MirrorableLight>();
    public MirrorableLight upLight;
    public MirrorableLight rightLight;
    public MirrorableLight downLight;
    public MirrorableLight leftLight;

    private void Start()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider2D>();
        //animator = GetComponentInChildren<Animator>();

        //Tell the output lights that they are not source lights
        upLight.parentMirror = this;
        rightLight.parentMirror = this;
        downLight.parentMirror = this;
        leftLight.parentMirror = this;

        switch (currentMirrorDirection)
        {
            case MirrorDirection.UpLeft:
                //animator.Play("mirrorRotate3");
                activeState = 0;
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 135f);
                break;
            case MirrorDirection.UpRight:
                //animator.Play("mirrorRotate0");
                activeState = 1;
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 45f);
                break;
            case MirrorDirection.DownRight:
                //animator.Play("mirrorRotate1");
                activeState = 2;
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 315f);
                break;
            case MirrorDirection.DownLeft:
                //animator.Play("mirrorRotate2");
                activeState = 3;
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 225f);
                break;
        }
        UpdateSprite(activeState * framesPerState + 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If it's on the light layer
        if (collision.gameObject.layer == 7)
        {
            //If this is a mirrorable light
            if (collision.gameObject.TryGetComponent(out MirrorableLight _light))
            {
                //Skip self lights
                //  Shouldn't actually harm function, but is nice to keep in mind
                if (_light == upLight || _light == rightLight || _light == downLight || _light == leftLight)
                {
                    return;
                }

                if (!inputLights.Contains(_light))
                {
                    inputLights.Add(_light);
                    UpdateLightState();
                }
            }
            else
            {
                //Debug.LogWarning("Warning: Light in range of mirror does not have the CustomLight component. May be circle light or bug!");
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //If it's on the light layer
        if (collision.gameObject.layer == 7)
        {
            //If this is a mirrorable light
            if (collision.gameObject.TryGetComponent(out MirrorableLight _light))
            {
                //Skip self lights
                //  Shouldn't actually harm function, but is nice to keep in mind
                if (_light == upLight || _light == rightLight || _light == downLight || _light == leftLight)
                {
                    return;
                }

                if (inputLights.Contains(_light))
                {
                    inputLights.Remove(_light);
                    UpdateLightState();
                }
            }
        }
    }

    public void UpdateSprite(int _newSprite)
    {
        if (_newSprite < 0)
        {
            _newSprite = framesPerState * 4 + _newSprite;
        }
        else
        {
            _newSprite %= framesPerState * 4;
        }
        spriteRenderer.sprite = spriteSheet[_newSprite];
    }

    public void OnRotate(bool _clockwise)
    {
        //Convert enum to backing int type, increment/decrement, handle wrapping, and finally cast back to enum type
        if (_clockwise)
        {
            currentMirrorDirection = (MirrorDirection)(((int)currentMirrorDirection + 1) % 4);
        }
        else
        {
            int _mirrorState = (int)currentMirrorDirection - 1;
            if (_mirrorState < 0)
            {
                _mirrorState = 3;
            }
            currentMirrorDirection = (MirrorDirection)_mirrorState;
        }

        //Reset lights during animation
        upLight.gameObject.SetActive(false);
        rightLight.gameObject.SetActive(false);
        downLight.gameObject.SetActive(false);
        leftLight.gameObject.SetActive(false);

        //Horrible horrible animation shenanigans, but it's too late now!
        rotationSequence?.Kill();
        rotationSequence = DOTween.Sequence();
        rotationSequence.onKill += () => { rotationSequence = null; };
        int f = 0;
        if (_clockwise)
        {
            //Increment State
            //  Yes the placement of this matters
            activeState += 1;
            activeState %= 4;

            for (; f < framesPerState - 1; f++)
            {
                //Needs copied or else it gets a stale reference and explodes
                int _newSprite = activeState * framesPerState + f;
                rotationSequence.AppendCallback(() => { UpdateSprite(_newSprite); });
                rotationSequence.AppendInterval(animationTime / framesPerState);
            }
            int _destinationSprite = activeState * framesPerState + f;
            rotationSequence.AppendCallback(() => { UpdateSprite(_destinationSprite); });
        }
        else
        {
            for (; f < framesPerState - 1; f++)
            {
                //Needs copied or else it gets a stale reference and explodes
                int _newSprite = activeState * framesPerState - f;
                rotationSequence.AppendCallback(() => { UpdateSprite(_newSprite); });
                rotationSequence.AppendInterval(animationTime / framesPerState);
            }
            int _destinationSprite = activeState * framesPerState - f;
            rotationSequence.AppendCallback(() => { UpdateSprite(_destinationSprite); });

            //Decrement State
            //  Yes the placement of this matters
            activeState -= 1;
            activeState = activeState < 0 ? 3 : activeState;
        }

        //Update Lighting States After Animation
        rotationSequence.AppendCallback(() => { UpdateLightState(); });
        //Play Animation
        rotationSequence.Play();

        // Set animator trigger and change capsule collider rotation to match new rotation of the mirror
        //animator.SetTrigger("mirrorTrig");
        switch (currentMirrorDirection)
        {
            case MirrorDirection.UpLeft:
                //animator.Play("mirrorRotate3");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 135f);
                break;
            case MirrorDirection.UpRight:
                //animator.Play("mirrorRotate0");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 45f);
                break;
            case MirrorDirection.DownRight:
                //animator.Play("mirrorRotate1");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 315f);
                break;
            case MirrorDirection.DownLeft:
                //animator.Play("mirrorRotate2");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 225f);
                break;
        }
    }

    private void UpdateLightState()
    {
        /*
        upLight.gameObject.SetActive(false);
        rightLight.gameObject.SetActive(false);
        downLight.gameObject.SetActive(false);
        leftLight.gameObject.SetActive(false);
        */

        List<MirrorableLight> _encounteredLights;
        switch (currentMirrorDirection)
        {
            case MirrorDirection.UpLeft:
                _encounteredLights = new List<MirrorableLight> { leftLight, upLight };
                leftLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.DOWN, ref _encounteredLights));
                _encounteredLights = new List<MirrorableLight> { leftLight, upLight };
                upLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.RIGHT, ref _encounteredLights));
                break;
            case MirrorDirection.UpRight:
                _encounteredLights = new List<MirrorableLight> { rightLight, upLight };
                rightLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.DOWN, ref _encounteredLights));
                _encounteredLights = new List<MirrorableLight> { rightLight, upLight };
                upLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.LEFT, ref _encounteredLights));
                break;
            case MirrorDirection.DownRight:
                _encounteredLights = new List<MirrorableLight> { rightLight, downLight };
                bool x = HasNonMirroredAncestorLight(LightDirection.UP, ref _encounteredLights);
                rightLight.gameObject.SetActive(x);
                _encounteredLights = new List<MirrorableLight> { rightLight, downLight };
                downLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.LEFT, ref _encounteredLights));
                break;
            case MirrorDirection.DownLeft:
                _encounteredLights = new List<MirrorableLight> { leftLight, downLight };
                leftLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.UP, ref _encounteredLights));
                _encounteredLights = new List<MirrorableLight> { leftLight, downLight };
                downLight.gameObject.SetActive(HasNonMirroredAncestorLight(LightDirection.RIGHT, ref _encounteredLights));
                break;
        }
    }

    //Recursive function to determine if this light is sustained by a real light
    //  Prevents bootstrapping lights
    public bool HasNonMirroredAncestorLight(LightDirection _desiredInputDirection, ref List<MirrorableLight> _encounteredLights)
    {
        foreach (MirrorableLight m in inputLights)
        {
            //If this light is facing the wrong way, skip!
            if (m.lightDir != _desiredInputDirection)
            {
                continue;
            }


            //If our encountered light is not a child of a mirror, then we win!
            if (m.parentMirror == null)
            {
                return true;
            }

            //If we've already encountered this light, skip!
            if (_encounteredLights.Contains(m))
            {
                continue;
            }
            _encounteredLights.Add(m);

            bool _foundGoodAncestor = false;
            switch (m.parentMirror.currentMirrorDirection)
            {
                case MirrorDirection.UpLeft:
                    if (_desiredInputDirection == LightDirection.LEFT)
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.DOWN, ref _encounteredLights);
                    }
                    else
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.RIGHT, ref _encounteredLights);
                    }
                    break;
                case MirrorDirection.UpRight:
                    if (_desiredInputDirection == LightDirection.RIGHT)
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.DOWN, ref _encounteredLights);
                    }
                    else
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.LEFT, ref _encounteredLights);
                    }
                    break;
                case MirrorDirection.DownLeft:
                    if (_desiredInputDirection == LightDirection.LEFT)
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.UP, ref _encounteredLights);
                    }
                    else
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.RIGHT, ref _encounteredLights);
                    }
                    break;
                case MirrorDirection.DownRight:
                    if (_desiredInputDirection == LightDirection.RIGHT)
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.UP, ref _encounteredLights);
                    }
                    else
                    {
                        _foundGoodAncestor = m.parentMirror.HasNonMirroredAncestorLight(LightDirection.LEFT, ref _encounteredLights);
                    }
                    break;
            }

            if (_foundGoodAncestor)
            {
                return true;
            }
        }

        //Failed to find anything :(
        return false;
    }

    private void OnDestroy()
    {
        //Debug.LogWarning("MIRROR KILL");
        //rotationSequence?.Kill();
    }

    private void OnDrawGizmos()
    {
        Color _originalColor = Gizmos.color;
        Gizmos.color = Color.antiqueWhite;
        Gizmos.DrawLine(transform.position, transform.position + (capsuleCollider.transform.rotation * Vector3.right));

        Gizmos.color = _originalColor;
    }
}
