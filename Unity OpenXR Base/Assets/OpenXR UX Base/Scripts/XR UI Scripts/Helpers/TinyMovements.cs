using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyMovements : MonoBehaviour
{
    float startTime;
    Quaternion targetRotation = new Quaternion();
    Vector3 targetPosition = new Vector3();
    Vector3 startPosition;
    float timeSpan;
    MeshRenderer theRenderer;
    Vector2 offset = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        theRenderer = this.gameObject.GetComponent<MeshRenderer>();
        ChooseNew();
    }

    // Update is called once per frame
    void Update()
    {
        float step = (Time.time - startTime) * 0.01f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        transform.position = Vector3.Slerp(transform.position, targetPosition, step);

        float step2 = 0.01f * Time.deltaTime;
        offset = Vector2.MoveTowards(offset, Vector2.one, step2);
        if (offset == Vector2.one) offset = Vector2.zero;

        theRenderer.material.SetTextureOffset("_MainTex", offset);
        if (Time.time-startTime > timeSpan)
        {
            ChooseNew();
        }  
    }

    private void ChooseNew()
    {
        Quaternion newQuat = new Quaternion();
        newQuat.eulerAngles = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        targetRotation = newQuat;

        targetPosition = startPosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

        timeSpan = Random.Range(1.0f, 4.0f);
        startTime = Time.time;
    }
}
