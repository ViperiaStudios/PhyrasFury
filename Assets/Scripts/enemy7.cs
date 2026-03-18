using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

using System.Collections;
using UnityEngine;

public class enemy7 : MonoBehaviour
{
    [SerializeField] private float _eSpeed = 8.0f; // Faster movement speed
    [SerializeField] private GameObject childObject; // ✅ Assign the child in the Inspector
    [SerializeField] private float teleportCooldown = 1.0f; // ✅ Teleport cooldown duration

    private Vector3 _lastPlayerPosition;
    private bool _playerDetected = false;
    private bool _isPaused = false;
    private bool _canTeleport = true; // ✅ Controls teleport cooldown

    void Start()
    {
        // ✅ Auto-find the child if not assigned in the Inspector
        if (childObject == null && transform.childCount > 0)
        {
            childObject = transform.GetChild(0).gameObject;
        }
    }

    void Update()
    {
        if (!_isPaused)
        {
            Movement();
        }

        // ✅ Check if child is destroyed, then destroy parent
        if (childObject == null)
        {
            Debug.Log("Child destroyed! Destroying parent.");
            Destroy(gameObject);
        }
    }

    void Movement()
    {
        transform.Translate(Vector3.left * _eSpeed * Time.deltaTime);

        if (transform.position.x <= -5.6f)
        {
            float randomY = Random.Range(4f, 7f);
            transform.position = new Vector3(21, randomY, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canTeleport) // ✅ Prevents multiple teleports per second
        {
            _lastPlayerPosition = other.transform.position;
            _playerDetected = true;

            Debug.Log("Player detected! Last recorded position: " + _lastPlayerPosition);
            StartCoroutine(TeleportToPlayer());

            _canTeleport = false; // ✅ Start cooldown
            StartCoroutine(TeleportCooldown()); // ✅ Prevents teleporting for 1 second
        }
    }

    IEnumerator TeleportToPlayer()
    {
        yield return new WaitForSeconds(0.3f);

        if (_playerDetected)
        {
            transform.position = _lastPlayerPosition;
            _playerDetected = false;

            Debug.Log("Teleported to player's last known position: " + transform.position);

            _isPaused = true;
            yield return new WaitForSeconds(1.0f);
            _isPaused = false;
        }
    }

    IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(teleportCooldown); // ✅ Waits 1 second
        _canTeleport = true; // ✅ Allows teleporting again
    }
}
