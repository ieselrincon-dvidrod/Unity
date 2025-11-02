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
        winTextObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
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
            count = count + 1;
            SetCountText();
        }
    }

    void SetCountText() 
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 7)
        {
            EndGame("¡Has ganado!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (gameEnded) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EndGame("¡Has perdido!");
        }
    }

    void EndGame(string message)
    {
        gameEnded = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        winTextObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = message;
        restartButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
