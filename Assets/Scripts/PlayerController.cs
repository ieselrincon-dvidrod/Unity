using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public float jumpForce = 5f;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public Button restartButton;
    public Button nextLevelButton;

    private Rigidbody rb; 
    private int count;
    private float movementX;
    private float movementY;
    private bool gameEnded = false;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0; 
        SetCountText();

        if (winTextObject != null) winTextObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);

        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(GoToNextLevel);
    }

    void OnMove(InputValue movementValue)
    {
        if (gameEnded) 
        {
            movementX = 0;
            movementY = 0;
            return;
        }

        Vector2 movementVector = movementValue.Get<Vector2>(); 
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    void FixedUpdate()
    {
        if (gameEnded)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnJump(InputValue value)
    {
        if (gameEnded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other) 
    {
        if (gameEnded) return;

        if (other.gameObject.CompareTag("PickUp")) 
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void SetCountText() 
    {
        if (countText != null)
            countText.text = "Count: " + count.ToString();

        if (count >= 7)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "MiniGane")
                EndGame("¡Has ganado!", true, true);
            else
                EndGame("¡Has ganado!", false, true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (gameEnded) return;

        if (collision.gameObject.CompareTag("Enemy"))
            EndGame("¡Has perdido!", false, false);
    }

    void EndGame(string message, bool hasNextLevel, bool playerWon)
    {
        gameEnded = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (winTextObject != null)
        {
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = message;
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);

            if (!hasNextLevel && SceneManager.GetActiveScene().name == "MiniGame2")
            {
                TextMeshProUGUI buttonText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = playerWon ? "Volver a empezar" : "Reiniciar nivel";
            }
        }

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(hasNextLevel);
    }

    void GoToNextLevel()
    {
        SceneManager.LoadScene("MiniGame2");
    }

    void RestartGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MiniGame2" && winTextObject.GetComponent<TextMeshProUGUI>().text == "¡Has ganado!")
            SceneManager.LoadScene("MiniGane");
        else
            SceneManager.LoadScene(currentScene);
    }
}
