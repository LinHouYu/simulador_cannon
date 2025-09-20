using UnityEngine;

public class Ballscript : MonoBehaviour
{
    private bool isOnGround = false; //是否在地面上
    private float timer; //计时器
    public float destroyTime = 3f; //延迟摧毁掉的时间

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnGround)
        {
            timer += Time.deltaTime;
            if(timer >= destroyTime)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;
        }
    }

    public void AutoKillBall()
    {
        Destroy(gameObject, destroyTime);
    }


}
