using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Mirror : MonoBehaviour, IRotatable
{
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
    private SpriteRenderer spriteRenderer = null;
    private Animator animator = null;

    private void Start()
    {

        capsuleCollider = GetComponentInChildren<CapsuleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Yup, something's hitting me");
        if (collision.gameObject.layer == 7 && collision.gameObject != outputLight)
        {
            Debug.Log("Yup, that's a light that's not mine");
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
        
        if (outputLight != null) Destroy(outputLight);

        // Animator stuff and change capsule collider rotation


        // transform.localRotation = Quaternion.Euler(new Vector3(0, 0, transform.localEulerAngles.z + 90));
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
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
                }
                else if (inputDirection == Vector2.right)
                {
                    // Light is coming from the left, so fire a new light upwards.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                }

                // If light is coming from a different direction, ignore it, the mirror is not a state where it can reflect it
                // Ergo, don't instantiate jack.

                break;
            case MirrorDirection.UpRight:
                if (inputDirection == Vector2.down)
                {
                    // Light is coming down, so fire a new light off to the right.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                else if (inputDirection == Vector2.left)
                {
                    // Light is coming from the right, so fire a new light upwards.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
                }
                break;
            case MirrorDirection.DownRight:
                if (inputDirection == Vector2.up)
                {
                    // Light is coming from below, so fire a new light off to the right.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                else if (inputDirection == Vector2.left)
                {
                    // Light is coming from the right, so fire a new light downwards.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                break;
            case MirrorDirection.DownLeft:
                if (inputDirection == Vector2.up)
                {
                    // Light is coming from below, so fire a new light off to the left.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                else if (inputDirection == Vector2.right)
                {
                    // Light is coming from the left, so fire a new light downwards.
                    outputLight = Instantiate(inputLight);
                    outputLight.transform.position = transform.position + new Vector3(0, 0.5f, 0);
                    outputLight.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                break;
        }
    }
}
