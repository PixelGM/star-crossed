using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D body;
    private Animator animator;
    private Vector2 moveInput;
    private float targetSpeed;
    private float currentSpeed;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    private bool canDash = true;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip[] footstepSounds;
    private AudioSource audioSource;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("XInput", moveInput.x);
            animator.SetFloat("YInput", moveInput.y);
        }
    }

    private void Update()
    {
        HandleDash();
        HandleMovement();
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        animator.SetInteger("state", 2);
        body.velocity = new Vector2(animator.GetFloat("XInput"), animator.GetFloat("YInput")) * dashSpeed;
        audioSource.PlayOneShot(dashSound);
        yield return new WaitForSeconds(dashDuration);
        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void HandleMovement()
    {
        if (moveInput != Vector2.zero)
        {
            targetSpeed = moveSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime);
            Vector2 moveVector = moveInput * currentSpeed * Time.fixedDeltaTime;
            body.MovePosition(body.position + moveVector);
            if (!audioSource.isPlaying)
            {
                PlayRandomFootstepSound();
            }

            animator.SetInteger("state", 1);
        }
        else
        {
            targetSpeed = 0;
            animator.SetInteger("state", 0);
            audioSource.Stop();
        }
    }

    private void PlayRandomFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }
}
