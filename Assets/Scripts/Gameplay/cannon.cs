using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cannon : MonoBehaviour
{
    public GameObject cannonBallPrefab;
    public Transform firepoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0.0f;
    public float shootForce = 700f;
    public Slider angleSliderX;
    public Slider angleSliderY;
    public Slider cooldownSlider;
    public TMP_Text cooldownTMPText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cooldownSlider != null)
        {
            cooldownSlider.minValue = 0f;
            cooldownSlider.maxValue = fireRate;
            cooldownSlider.interactable = false;
        }

        UpdateCooldownUI(); //初始化显示
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        UpdateCooldownUI(); //模块化调用

    }

    public void Shoot()
    {
        if (cannonBallPrefab != null && firepoint != null)
        {
            GameObject cannonBall = Instantiate(cannonBallPrefab, firepoint.position, firepoint.rotation);
            Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firepoint.up * shootForce);
            }
        }
        else
        {
            Debug.LogWarning("Cannon ball prefab or firepoint is missing");
        }
    }

    public void ChangeCannonAngleX()
    {
        float anglex = angleSliderX.value;
        transform.localRotation = Quaternion.Euler(anglex, angleSliderY.value, 0);
    }

    public void ChangeCannonAngleY()
    {
        float angley = angleSliderY.value;
        transform.localRotation = Quaternion.Euler(angleSliderX.value, angley, 0);
    }
    public void UpdateCooldownUI()
    {
        float remainingCD = nextFireTime - Time.time;
        float clampedCD = Mathf.Clamp(remainingCD, 0f, fireRate);

        if (cooldownSlider != null)
            cooldownSlider.value = clampedCD;

        if (cooldownTMPText != null)
            cooldownTMPText.text = clampedCD > 0f ? clampedCD.ToString("F1") + "s" : "";
    }


}