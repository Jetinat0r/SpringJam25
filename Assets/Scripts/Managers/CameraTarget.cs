using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class CameraTarget : MonoBehaviour
{
    [SerializeField]
    public float ventSecondsPerScreen = 1.5f;
    public bool followingVent = false;
    private Transform targetPlayer;

    private Action onCompletePath = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Camera.main.GetComponent<CameraSnapToPlayerZone>().Init(transform);
    }

    public void Init(Transform _targetPlayer)
    {
        targetPlayer = _targetPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!followingVent)
        {
            transform.position = targetPlayer.position;
        }
    }

    public void TakeVentPath(List<int> _directions, Action _onCompletePath = null)
    {
        Sequence _ventSequence = DOTween.Sequence(transform);
        _ventSequence.AppendInterval(ventSecondsPerScreen);
        Vector3 _targetPos = transform.position;
        foreach (int d in _directions)
        {
            //Stupid foreach loops
            int _dir = d;
            if (Mathf.Abs(_dir) == 1)
            {
                _targetPos += new Vector3(_dir * LevelManager.instance.zoneSize.x, 0, 0);
            }
            else
            {
                _dir /= 2;
                _targetPos += new Vector3(0, _dir * LevelManager.instance.zoneSize.y, 0);
            }
            _ventSequence.Append(transform.DOMove(_targetPos, 0));
            _ventSequence.AppendInterval(ventSecondsPerScreen);
        }

        _ventSequence.onComplete += ExitVent;

        onCompletePath = _onCompletePath;

        Debug.Log("Started Venting!");
        followingVent = true;
        _ventSequence.Play();
    }

    public void ExitVent()
    {
        Debug.Log("Finished Venting!");
        followingVent = false;

        onCompletePath?.Invoke();
        onCompletePath = null;
    }
}
