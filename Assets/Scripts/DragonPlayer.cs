using System.Collections;
using UnityEngine;

public class DragonPlayer : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _jumpHeight = 3.0f;
    [SerializeField] private float _jumpSpeed = 5.0f;
    [SerializeField] private float _fallSpeed = 5.0f;
    private Animator _animator;
    private bool _canAttack = true;
    private bool _isAttacking = false; 
    private bool _isJumping = false; // ✅ Prevents double jumping
    private float _groundY = 6.43f; // ✅ Fixed ground position

    [SerializeField] private GameObject clawEffectPrefab; 
    [SerializeField] private Transform clawSpawnPoint;  


    void Start()
    {
        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator not found on DragonPlayer!");
        }

        if (clawSpawnPoint == null)
        {
            Debug.LogError("Claw spawn point not assigned!");
        }
    }

    void Update()
    {
        if (!_isAttacking) // ✅ Blocks movement updates while attacking
        {
            HandleMovement();
        }

        HandleJump();
        HandleAttack();
    }

    void HandleMovement()
    {
        if (_isAttacking) return; // ✅ Completely lock movement during attack

        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 direction = new Vector3(horizontalInput, 0, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        float minX = 2f;
        float maxX = 16f;
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, 0);

        // ✅ Only update isWalking if NOT attacking and NOT jumping
        if (!_isAttacking && !_isJumping)
        {
            _animator.SetBool("isWalking", horizontalInput != 0);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
        {
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump()
    {
        _isJumping = true;
        float targetY = _groundY + _jumpHeight;

        // ✅ Move UP
        while (transform.position.y < targetY)
        {
            transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, targetY, 0);
        yield return new WaitForSeconds(0.1f);

        // ✅ Move DOWN
        while (transform.position.y > _groundY)
        {
            transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, _groundY, 0);
        _isJumping = false;

        // ✅ Reset walking animation ONLY if NOT attacking
        if (!_isAttacking)
        {
            _animator.SetBool("isWalking", true);
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        _canAttack = false;
        _isAttacking = true; // ✅ Fully locks movement & animation updates

        _animator.SetBool("isWalking", false); // ✅ Stop movement animation
        _animator.SetTrigger("clawanim");

        yield return new WaitForSeconds(0.6f); // ✅ Ensures the full attack animation plays

        // ✅ FORCE the walking animation to play once, even if standing still
        _animator.SetBool("isWalking", true);

        _isAttacking = false; // ✅ Re-enable movement & animations

        //yield return new WaitForSeconds(0.1f); // ✅ Small delay so it syncs with animation

        if (clawEffectPrefab != null && clawSpawnPoint != null)
        {
            Instantiate(clawEffectPrefab, clawSpawnPoint.position, Quaternion.identity);
        }

       

        // ✅ FORCE the walking animation to play once, even if standing still
        //_animator.SetBool("isWalking", true);

        yield return new WaitForSeconds(0.1f); // ✅ Attack cooldown
        _canAttack = true;
    }
}
