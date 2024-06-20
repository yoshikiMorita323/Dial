using UnityEngine;

// 敵の乗っている車いすを挙動処理
public class WheelChairController : MonoBehaviour
{
    // Wheel_Lを指定
    [SerializeField]
    private GameObject leftWheel = null;
    // Wheel_Rを指定
    [SerializeField]
    private GameObject rightWheel = null;
    //Sub_Wheelを指定
    [SerializeField]
    private GameObject subWheel = null;
    // 親オブジェクトのスピードが１の時のタイヤの回転スピードを指定
    [SerializeField]
    private float wheelRotateSpeed = 50;
    // X軸の角度を指定
    private float rotateAngleX = 0;
    // 親オブジェクトのスピードを指定
    private float objectSpeed = 2;
    // 車いすの動作音を指定
    [SerializeField]
    private AudioSource wheelChairSound = null;

    void Update()
    {
        // 親オブジェクトが動いている場合、タイヤを回す
        if(objectSpeed > 0)
        {
            rotateAngleX += wheelRotateSpeed * objectSpeed;
            leftWheel.transform.localRotation = Quaternion.Euler(rotateAngleX, 0, 0);
            rightWheel.transform.localRotation = Quaternion.Euler(rotateAngleX, 0, 0);
            subWheel.transform.localRotation = Quaternion.Euler(rotateAngleX, 0, 0);
        }
    }
    // 親オブジェクトのスピードを更新
    public void InputObjectSpeed(float speed)
    {
        objectSpeed = speed;
        // 親オブジェクトのスピードが0になった場合、車いすの動作音を消す
        if(speed == 0)
        {
            wheelChairSound.volume = 0;
        }
        else
        {
            wheelChairSound.volume = 1;
        }
    }
}
