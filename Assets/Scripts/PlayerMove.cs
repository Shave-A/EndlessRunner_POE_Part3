using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float Speed = 2;
    public float sideSpeed = 3;
    public float leftLimit = -4;
    public float rightLimit = 4;
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed, Space.World);
        if (Input.GetKey(KeyCode.A))
        {
            if (this.gameObject.transform.position.x > leftLimit)
            {
                transform.Translate(Vector3.left * Time.deltaTime * sideSpeed);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (this.gameObject.transform.position.x < rightLimit)
            {
                transform.Translate(Vector3.right * Time.deltaTime * sideSpeed);
            }
        }
    }
}
