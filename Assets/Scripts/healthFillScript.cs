using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthFillScript : MonoBehaviour
{
    private Transform player;
    private PlayerScript playerController;
    private int maxLife;
    private int life;

    private float initialLength;
    private float initialScaleX;
    private Vector3 initialScale;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerScript playerController = player.GetComponent<PlayerScript>();
        maxLife = playerController.getMaxLife();

        initialLength = GetObjectLength();
        initialScale = transform.localScale;
        initialScaleX = initialScale.x;
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            PlayerScript playerController = player.GetComponent<PlayerScript>();
            life = playerController.getLife();
        }
        else
        {
            life = 0;
        }

        float fillAmount = (float)life / maxLife;
        Vector3 newScale = initialScale;
        newScale.x = initialScaleX * fillAmount;
        transform.localScale = newScale;

        float newPositionX = initialPosition.x - (initialScaleX - newScale.x)*initialLength / 4.6f;
        transform.position = new Vector3(newPositionX, initialPosition.y, initialPosition.z);
    }

    // ��ȡ����ԭʼ�ĳ���
    private float GetObjectLength()
    {
        Bounds bounds = GetObjectBounds();
        return bounds.size.x;
    }

    // ��ȡ����İ�Χ����Ϣ
    private Bounds GetObjectBounds()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        else
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                return collider.bounds;
            }
        }

        // ����޷���ȡ��Χ����Ϣ���򷵻�һ����Ч��Bounds
        return new Bounds();
    }
}
