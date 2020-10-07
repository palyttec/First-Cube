using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform camTransform;
    private float shakeDur = 1f, shakeAmount = 0.04f, decreaseFactor = 1.5f;

    private Vector3 originPos;

    private void Start()
    {
        camTransform = GetComponent<Transform>();
        originPos = camTransform.localPosition;
    }

    private void Update()
    {
        if(shakeDur > 0)
        {
            camTransform.localPosition = originPos + Random.insideUnitSphere * shakeAmount;// постоянно меняем позицию камеры ( к оригинальной позиции  добавляем случайнную точку -- выбираем случайную точку с определенным радиусом котрорую мы сами устанавливаем
            shakeDur -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDur = 0;
            camTransform.localPosition = originPos;
        }
    }
}
