using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainScreenTest : MonoBehaviour
{
    [SerializeField] private Text textComponent; // ✅ For Unity's default Text UI
    [SerializeField] private TextMeshProUGUI tmpTextComponent; // ✅ For TextMeshPro

    [SerializeField] private float flashSpeed = 0.5f; // ✅ Time between flashes

    void Start()
    {
        StartCoroutine(FlashText());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ✅ Left Mouse Button Click
        {
            LoadGameScene();
        }
    }

    IEnumerator FlashText()
    {
        while (true) // ✅ Loops forever
        {
            // ✅ Toggle Text visibility
            if (textComponent != null)
                textComponent.enabled = !textComponent.enabled;

            if (tmpTextComponent != null)
                tmpTextComponent.enabled = !tmpTextComponent.enabled;

            yield return new WaitForSeconds(flashSpeed);
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("Game"); // ✅ Loads the "Game" scene
    }
}
