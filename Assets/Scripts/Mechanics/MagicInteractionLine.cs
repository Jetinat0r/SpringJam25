using UnityEngine;

public class MagicInteractionLine : MonoBehaviour
{
    [SerializeField]
    ParticleSystem lineParticles;

    [SerializeField]
    public float particlesPerUnit = 4;

    [SerializeField]
    public float minParticles = 8f;

    [SerializeField]
    public float particleVelocityMultiplier = 1f;

    [SerializeField]
    //The particles move fast enough to overshoot their goal, so we end the spawn box a bit early
    //  1-endBufferSpace = percentage of particles that make it to the end. Too low, and they overshoot a lot! Too high and they don't make it at all!
    public float endBufferSpace = 0.6f;

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
        //Special case if start and end points are the same
        //  Our math has divisions over distance, and 0 will explode!
        if (_startPos == _endPos)
        {
            ZeroLine(_startPos);
            return;
        }

        //Make particles move towards target
        //  We do velocity up here so we can change it if we need to when we calculate our emission box size
        var _psVelocity = lineParticles.velocityOverLifetime;
        _psVelocity.xMultiplier = particleVelocityMultiplier;

        //Concept: Assume simple linear velocity decay and use that to calculate maximum traveled distance
        //  Then, scale by endBufferSpace to find a nice medium where most particles will make it near the end without overshooting spectacularly
        //deltaPosition = initialVelocity*time + 0.5*acceleration*time*time

        //Get max lifetime of particle to use as our idealized time
        float _maxLifetime = lineParticles.main.startLifetime.constantMax;

        //Find the absolute maximum starting velocity of a particle
        //  Code may need changed if constants are being used instead of curves, but we're not doing that!
        float _startParticleVelocity = lineParticles.velocityOverLifetime.x.curveMax.Evaluate(0f) * particleVelocityMultiplier;
        //Find the absolute maximum ending velocity of a particle
        float _endParticleVelocity = lineParticles.velocityOverLifetime.x.curveMax.Evaluate(1f) * particleVelocityMultiplier;

        //Acceleration = deltaVelocity / time
        float _acceleration = (_endParticleVelocity - _startParticleVelocity) / _maxLifetime;

        //Calculate max change in position over a particles lifetime
        float _maxDistanceTraveled = (_startParticleVelocity * _maxLifetime) + (0.5f * _acceleration * _maxLifetime * _maxLifetime);
        
        ///In theory the above 2 steps can be rewritten like so,
        /// but this isn't run often enough to require optimization over readability
        ///float _maxDistanceTraveled = _maxLifetime * (_startParticleVelocity + (0.5f * (_endParticleVelocity - _startParticleVelocity) * _maxLifetime));

        Vector3 _endToStart = _startPos - _endPos;

        //Shift end pos to account for particle velocity overshooting the target
        //_endPos = (_startPos - _endPos) * _maxDistanceTraveled * endBufferSpace + _endPos;
        //_endPos = _endToStart * Mathf.Min((_maxDistanceTraveled / _endToStart.magnitude), 1f) * endBufferSpace + _endPos;
        if (_maxDistanceTraveled < _endToStart.magnitude)
        {
            _endPos = _maxDistanceTraveled * endBufferSpace * _endToStart.normalized + _endPos;
        }
        else
        {
            _endPos = _endToStart * endBufferSpace + _endPos;
            _psVelocity.xMultiplier *= _endToStart.magnitude;
        }

        //Put the particle system in the middle of our target points
        transform.position = (_startPos + _endPos) / 2f;
        //Rotate the particle system so that local Right goes towards _endPos
        Vector3 _targetDir = _endPos - _startPos;
        float _targetRot = Mathf.Atan2(_targetDir.y, _targetDir.x);
        transform.rotation = Quaternion.Euler(0f, 0f, _targetRot * Mathf.Rad2Deg);

        //Set up box size
        //  Scaled so left edge is on _startPos and right edge is on _endPos
        //You're not allowed to do lineParticles.shape.scale = ...
        var _psBox = lineParticles.shape;
        _psBox.scale = new Vector3(_targetDir.magnitude, lineParticles.shape.scale.y, lineParticles.shape.scale.z);

        //Set up particles to maintain density across any size
        //lineParticles.emission.SetBurst(0, new ParticleSystem.Burst(0f, _targetDir.magnitude * particlesPerUnit));
        var _psEmission = lineParticles.emission;
        _psEmission.rateOverTime = Mathf.Max(minParticles, _targetDir.magnitude * particlesPerUnit);
    }

    //Special case where start pos and end pos are identical so that things don't explode
    private void ZeroLine(Vector3 _startPos)
    {
        transform.position = _startPos;

        var _psBox = lineParticles.shape;
        _psBox.scale = new Vector3(lineParticles.shape.scale.y, lineParticles.shape.scale.y, lineParticles.shape.scale.z);

        var _psEmission = lineParticles.emission;
        _psEmission.rateOverTime = minParticles;

        var _psVelocity = lineParticles.velocityOverLifetime;
        _psVelocity.xMultiplier = 0f;
    }

    public void PlayParticles()
    {
        if (lineParticles != null)
        {
            lineParticles.Play();
        }
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

            SetupLine(DEBUG_START_POINT, DEBUG_END_POINT);
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
