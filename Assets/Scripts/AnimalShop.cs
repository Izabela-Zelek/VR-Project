using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Handles the purchase and spawning of farm animals
/// </summary>
public class AnimalShop : MonoBehaviour
{
    public int Price;
    public GameObject Button;
    public GameObject BoughtObject;
    public Transform Pos;
    public UnityEvent OnPress;
    public UnityEvent OnRelease;

    private AudioSource _sound;
    private bool _isPressed;

    /// <summary>
    /// Loads button press audio upon start
    /// </summary>
    private void Start()
    {
        _sound = GetComponent<AudioSource>();
        _isPressed = false;
    }

    /// <summary>
    /// Upon touch, simulates downward movement of button and plays audio
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(!_isPressed && other.gameObject.layer == 11)
        {
            Button.transform.localPosition = new Vector3(0, 0.02f, 0);
            OnPress.Invoke();
            _sound.Play();
            _isPressed = true;
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
            Button.transform.localPosition = new Vector3(0, 0.035f, 0);
            OnRelease.Invoke();
            _isPressed = false;
        }
    }

    /// <summary>
    /// Takes away price of bought animal from player money
    /// Spawns animal in defined position
    /// </summary>
    public void SpawnObject()
    {
        if (Price <= GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() && GameObject.Find("GameManager").GetComponent<GameManager>().ShopOpen)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().MinusMoney(GameObject.Find("GameManager").GetComponent<GameManager>().GetMoney() - Price);
            GameObject boughtItem = Instantiate(BoughtObject, Pos.position, Quaternion.identity,GameObject.Find("Animals").transform);
        }
    }
}
