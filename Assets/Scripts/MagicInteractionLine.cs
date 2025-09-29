using UnityEngine;

public class MagicInteractionLine : MonoBehaviour
{
    [SerializeField]
    ParticleSystem lineParticles;

    [SerializeField]
    public float particlesPerUnit = 4;

    [SerializeField]
    public float particleVelocityMultiplier = 1f;

    [SerializeField]
    //The particles move fast enough to overshoot their goal, so we end the spawn box a bit early
    public float endBufferSpace = 0.25f;

#if UNITY_EDITOR
    [SerializeField]
    public Vector3 DEBUG_START_POINT = Vector3.zero;
    [SerializeField]
    public Vector3 DEBUG_END_POINT = Vector3.zero;
    [SerializeField]
    public bool DEBUG_TRIGGER = false;
#endif

    public void SetupLine(Vector3 _startPos, Vector3 _endPos)
    {
        //Shift end pos to account for particle velocity overshooting the target
        _endPos = (_startPos - _endPos).normalized * endBufferSpace + _endPos;

        transform.position = (_startPos + _endPos) / 2f;
        Vector3 _targetDir = _endPos - _startPos;
        float _targetRot = Mathf.Atan2(_targetDir.y, _targetDir.x);
        transform.rotation = Quaternion.Euler(0f, 0f, _targetRot * Mathf.Rad2Deg);

        //Set up box size
        //You're not allowed to do lineParticles.shape.scale = ...
        var _psBox = lineParticles.shape;
        _psBox.scale = new Vector3(_targetDir.magnitude, lineParticles.shape.scale.y, lineParticles.shape.scale.z);

        //Set up particles to maintain density across any size
        //lineParticles.emission.SetBurst(0, new ParticleSystem.Burst(0f, _targetDir.magnitude * particlesPerUnit));
        var _psEmission = lineParticles.emission;
        _psEmission.rateOverTime = _targetDir.magnitude * particlesPerUnit;

        //Make particles move towards target
        var _psVelocity = lineParticles.velocityOverLifetime;
        _psVelocity.xMultiplier = particleVelocityMultiplier;
    }

    public void PlayParticles()
    {
        lineParticles.Play();
    }

#if UNITY_EDITOR
    private void Start()
    {
        DEBUG_TRIGGER = false;
    }

    private void Update()
    {
        if (DEBUG_TRIGGER)
        {
            DEBUG_TRIGGER = false;

            //SetupLine(DEBUG_START_POINT, DEBUG_END_POINT);
            PlayParticles();
        }
    }

    private void OnDrawGizmos()
    {
        Color _initialColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(DEBUG_START_POINT, 0.25f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(DEBUG_END_POINT, 0.25f);
        Gizmos.color = _initialColor;
    }
#endif
}
