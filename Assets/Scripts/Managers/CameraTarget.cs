using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class CameraTarget : MonoBehaviour
{
    [SerializeField]
    public float ventSecondsPerScreen = 0.25f;
    //Screen transition is 0.75 seconds
    //Check CameraSnapToPlayerZone.cs if this ever changes
    private float screenScrollTime = 0.75f;
    public bool followingVent = false;
    private Transform targetTransform;

    private Action onCompletePath = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //Camera.main.GetComponent<CameraSnapToPlayerZone>().Init(transform);
    }

    public void Init(Transform _targetTransform)
    {
        targetTransform = _targetTransform;
        transform.position = targetTransform.position;
        Camera.main.GetComponent<CameraSnapToPlayerZone>().Init(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!followingVent)
        {
            transform.position = targetTransform.position;
        }
    }

    public void TakeVentPath(List<int> _directions, Action _onCompletePath = null)
    {
        Sequence _ventSequence = DOTween.Sequence(transform);
        _ventSequence.SetUpdate(true);
        //_ventSequence.AppendInterval(ventSecondsPerScreen);

        if (_directions.Count > 0)
        {
            //Wait a tick before screen transitioning
            _ventSequence.AppendInterval(ventSecondsPerScreen);
        }
        Vector3 _targetPos = transform.position;
        for (int i = 0; i < _directions.Count; i++)
        {
            //Don't fiddle with the original value
            int _dir = _directions[i];
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

            if (i < _directions.Count - 1)
            {
                _ventSequence.AppendInterval(screenScrollTime + ventSecondsPerScreen);
            }
            else
            {
                _ventSequence.AppendInterval(screenScrollTime + (ventSecondsPerScreen ));
            }
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
