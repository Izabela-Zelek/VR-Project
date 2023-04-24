using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Handles the purchase and spawning of farm animals
/// </summary>
public class AnimalShop : MonoBehaviour
{
    public int price;
    public GameObject button;
    public GameObject boughtObject;
    public Transform pos;
    private AudioSource sound;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    bool isPressed;

    /// <summary>
    /// Loads button press audio upon start
    /// </summary>
    void Start()
    {
        sound = GetComponent<AudioSource>();
        isPressed = false;
    }

    /// <summary>
    /// Upon touch, simulates downward movement of button and plays audio
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(!isPressed && other.gameObject.layer == 11)
        {
            button.transform.localPosition = new Vector3(0, 0.02f, 0);
            onPress.Invoke();
            sound.Play();
            isPressed = true;
        }
    }

    /// <summary>
    /// Upon exit of touch, return button to original position and invokes the SpawnObject function
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject)
        {
            button.transform.localPosition = new Vector3(0, 0.035f, 0);
            onRelease.Invoke();
            isPressed = false;
        }
    }

    /// <summary>
    /// Takes away price of bought animal from player money
    /// Spawns animal in defined position
    /// </summary>
    public void SpawnObject()
    {
        if (price <= GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() && GameObject.Find("GameManager").GetComponent<GameManager>().shopOpen)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().MinusMoney(GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() - price);
            GameObject boughtItem = Instantiate(boughtObject, pos.position, Quaternion.identity,GameObject.Find("Animals").transform);
        }
    }
}
