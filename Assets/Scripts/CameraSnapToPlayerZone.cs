using DG.Tweening;
using JetEngine;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSnapToPlayerZone : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2Int zoneSize = new Vector2Int(10, 9);
    private Vector2Int currentZone;
    [SerializeField] private float scrollTime = .75f;
    private Tween panTween = null;
    private float oldTimeScale = 1f;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Error: Camera needs a target to follow");
        }

        //currentZone = new Vector2Int(Mathf.RoundToInt(target.position.x / zoneSize.x), Mathf.RoundToInt(target.position.y / zoneSize.y));
        currentZone = new Vector2(MathUtils.GetClosestEvenDivisor(target.position.x, zoneSize.x) / zoneSize.x, MathUtils.GetClosestEvenDivisor(target.position.y, zoneSize.y) / zoneSize.y).ToVector2Int();
        Vector2 _newPos = currentZone * zoneSize;
        transform.position = new Vector3(_newPos.x, _newPos.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        if (LevelMenuManager.playerOverride) return;

        // Only move screen in distinct zones, snap to current zone (2D Zelda / Metroid)
        Vector2Int _newZone = new Vector2(MathUtils.GetClosestEvenDivisor(target.position.x, zoneSize.x) / zoneSize.x, MathUtils.GetClosestEvenDivisor(target.position.y, zoneSize.y) / zoneSize.y).ToVector2Int();
        if (_newZone != currentZone)
        {
            currentZone = _newZone;
            panTween?.Kill();
            
            Vector2 _newPos = _newZone * zoneSize;
            
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            LevelMenuManager.roomScrollTransitionOverride = true;
            panTween = transform.DOMove(new Vector3(_newPos.x, _newPos.y, transform.position.z), scrollTime).SetEase(Ease.OutQuad).SetUpdate(true);
            panTween.onComplete = () => { Time.timeScale = oldTimeScale; LevelMenuManager.roomScrollTransitionOverride = false; };
        }
    }
}
