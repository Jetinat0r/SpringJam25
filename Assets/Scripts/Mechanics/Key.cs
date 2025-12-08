using DG.Tweening;
using UnityEngine;

public class Key : Collectible
{
    [SerializeField] protected ParticleSystem ambientSmallVfx;
    [SerializeField] protected ParticleSystem ambientLargeVfx;
    [SerializeField] protected ParticleSystem unlockVfx;
    [Header("Key Launch")]
    [SerializeField]
    public float punchDistance = 1.5f;
    [SerializeField]
    public float punchTime = 0.35f;
    [SerializeField]
    public AnimationCurve punchCurve;
    [SerializeField]
    public float launchTime = 0.25f;
    [SerializeField]
    public AnimationCurve launchCurve;

    protected override void OnCollected(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            /*
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            
            Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
            foreach (Door door in doors)
            {
                if (door.remainingKeys.Contains(this))
                {
                    door.remainingKeys.Remove(this);
                    door.UpdateKeyholes(true);
                    Debug.Log("Keyholes updated");
                }
            }

            DisplayPickupVFX();
            Destroy(gameObject);
            */

            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            Vector3 _avoidPos = player.cameraTarget.position;
            PlayPickupSFX(player);

            Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
            foreach (Door door in doors)
            {
                if (door.remainingKeys.Contains(this))
                {
                    amplitude = 0f;
                    Vector3 _punchPos = (transform.position - _avoidPos).normalized * punchDistance + transform.position;
                    Vector3 _targetPos = door.transform.position + (door.transform.rotation * new Vector3(0f, 0.5f, 0f));
                    Vector3 _targetDir = _targetPos - _punchPos;
                    float _rotAngle = Mathf.Atan2(_targetDir.y, _targetDir.x) * Mathf.Rad2Deg;
                    //spriteTransform.rotation = Quaternion.Euler(0f, 0f, _rotAngle);
                    //spriteTransform.transform.localRotation = Quaternion.Euler(0f, 0f, -_rotAngle);



                    GetComponentInChildren<Collider2D>().enabled = false;

                    

                    Sequence _sequence = DOTween.Sequence();

                    /*
                    _sequence.Append(transform.DOLocalMoveX(_targetDir.magnitude, punchTime).SetEase(Ease.OutQuad));
                    _sequence.Join(transform.DOLocalMoveY((_targetPos - _targetDir).magnitude, launchTime).SetEase(Ease.InBack).SetDelay(launchStartTime));
                    */
                    if (_rotAngle < 0f)
                    {
                        _rotAngle += 360f;
                    }

                    _sequence.Append(transform.DOMove(_punchPos, punchTime).SetEase(punchCurve));
                    _sequence.Join(transform.DORotate(new Vector3(0f, 0f, 360f + 90f + _rotAngle), punchTime, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad));
                    _sequence.Append(transform.DOMove(_targetPos, launchTime).SetEase(launchCurve).OnStart(() =>
                    {
                        player.soundPlayer.PlaySound("Game.KeyFly");
                    }));
                    _sequence.onComplete += UnlockDoor;
                    _sequence.Play();
                }
            }
        }
    }

    private void UnlockDoor()
    {
        Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
        foreach (Door door in doors)
        {
            if (door.remainingKeys.Contains(this))
            {
                door.remainingKeys.Remove(this);
                door.UpdateKeyholes(true);
                Debug.Log("Keyholes updated");
            }
        }

        ambientSmallVfx.Stop();
        ambientLargeVfx.Stop();
        unlockVfx.Play();
        PlayerMovement.instance.soundPlayer.PlaySound("Game.KeyholeRemove");
        Destroy(gameObject);
    }
}
