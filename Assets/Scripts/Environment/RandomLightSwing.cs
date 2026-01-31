
using UnityEngine;

public class RandomLightSwing : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 1.5f;
    
    [Tooltip("ความเร็วในการเปลี่ยนแสง (ยิ่งมากยิ่งสั่นเร็ว)")]
    [SerializeField] private float speed = 1.0f;

    private Light targetLight;
    private float noiseOffset;

    void Start()
    {
        targetLight = GetComponent<Light>();
        // สุ่มจุดเริ่มของ Noise เพื่อไม่ให้ไฟทุกดวงขยับเหมือนกันเป๊ะ
        noiseOffset = Random.Range(0f, 9999f);
    }

    void Update()
    {
        // สร้างค่าสุ่มที่นุ่มนวลระหว่าง 0 ถึง 1
        float noise = Mathf.PerlinNoise(Time.time * speed, noiseOffset);
        
        // แปลงค่า Noise (0-1) ให้เป็นช่วง Intensity ที่เราต้องการ
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
