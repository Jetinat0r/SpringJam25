using NUnit.Framework.Constraints;
using System;
using UnityEngine;

public class Mirror : MonoBehaviour, IRotatable
{
    [SerializeField]
    private Transform _customMagicLinePivot;
    public Transform CustomMagicLinePivot { get => _customMagicLinePivot != null ? _customMagicLinePivot : transform; set => _customMagicLinePivot = value; }

    private enum MirrorDirection
    {
        UpLeft,
        UpRight,
        DownRight,
        DownLeft
    }

    [SerializeField] private MirrorDirection state = MirrorDirection.UpLeft;

    private GameObject outputLight = null;
    private CapsuleCollider2D capsuleCollider = null;
    private Animator animator = null;
    private Vector3 lightOffset = Vector3.up * 0.5f;


    private void Start()
    {
        capsuleCollider = GetComponentInChildren<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();

        switch (state)
        {
            case MirrorDirection.UpLeft:
                animator.Play("mirrorRotate3");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 135);
                break;
            case MirrorDirection.UpRight:
                animator.Play("mirrorRotate0");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 225);
                break;
            case MirrorDirection.DownRight:
                animator.Play("mirrorRotate1");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 315);
                break;
            case MirrorDirection.DownLeft:
                animator.Play("mirrorRotate2");
                capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * 45);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (outputLight != null)
        {
            // Stop firing this event if there is already an output light
            return;
        }
        if (collision.gameObject.layer == 7 && collision.gameObject != outputLight)
        {
            ReflectLight(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && collision.gameObject != outputLight)
        {
            if (outputLight != null) Destroy(outputLight);
        }
    }

    public void OnRotate()
    {
        /*
         * Breaking down this mess of a line:
         * Ok, so, first, you get the next state in line in the enum, but you can only do math if you cast an enum state to an int
         * But if it goes OOB, you have to make it loop back to the start (hence the modulo with the number of enum states)
         * And this all returns an int, so you gotta cast it back to being an enum state
         */
        state = (MirrorDirection)(((int)state + 1) % Enum.GetNames(typeof(MirrorDirection)).Length);

        if (outputLight != null)
        {
            Destroy(outputLight);
            outputLight = null;
        }

        // Set animator trigger and change capsule collider rotation to match new rotation of the mirror
        animator.SetTrigger("mirrorTrig");
        capsuleCollider.transform.localRotation = Quaternion.Euler(Vector3.forward * (capsuleCollider.transform.localEulerAngles.z + 90));
    }

    private void ReflectLight(GameObject inputLight)
    {
        Vector2 inputDirection = inputLight.GetComponent<CustomLight>().lightDirection;

        if (inputDirection == null)
        {
            Debug.LogError("Error: Input light does not have the CustomLight component");
            return;
        }
        else
        {
            Debug.Log("Mirror triggered! Input direction: " + inputDirection);
        }

        switch (state)
        {
            case MirrorDirection.UpLeft:
                if (inputDirection == Vector2.down)
                {
                    // Light is coming down, so fire a new light off to the left.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.forward * 270);
                }
                else if (inputDirection == Vector2.right)
                {
                    // Light is coming from the left, so fire a new light upwards.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.forward * 180);
                }

                // If light is coming from a different direction, ignore it, the mirror is not a state where it can reflect it
                // Ergo, don't instantiate jack.
                break;
            case MirrorDirection.UpRight:
                if (inputDirection == Vector2.down)
                {
                    // Light is coming down, so fire a new light off to the right.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.forward * 90);
                }
                else if (inputDirection == Vector2.left)
                {
                    // Light is coming from the right, so fire a new light upwards.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.forward * 180);
                }
                break;
            case MirrorDirection.DownRight:
                if (inputDirection == Vector2.up)
                {
                    // Light is coming from below, so fire a new light off to the right.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.forward * 90);
                }
                else if (inputDirection == Vector2.left)
                {
                    // Light is coming from the right, so fire a new light downwards.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
                break;
            case MirrorDirection.DownLeft:
                if (inputDirection == Vector2.up)
                {
                    // Light is coming from below, so fire a new light off to the left.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.forward * 90);
                }
                else if (inputDirection == Vector2.right)
                {
                    // Light is coming from the left, so fire a new light downwards.
                    outputLight = Instantiate(inputLight, transform);
                    outputLight.transform.position = transform.position + lightOffset;
                    outputLight.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
                break;
        }
    }
}
