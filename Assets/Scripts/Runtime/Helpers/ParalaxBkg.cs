using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBkg : MonoBehaviour
{

    [SerializeField]
    SpriteRenderer SpriteRenderer;

    [SerializeField]
    Transform target;

    Vector2 Offset;

    [SerializeField]
    float Speed;

    Vector3 original;
    public void Init(Transform _target)
    {
        original = transform.position;
        target = _target;
        Offset = SpriteRenderer.material.GetTextureOffset("_MainTex");
    }

    // Update is called once per frame
    void Update()
    {


      //  transform.position = new Vector3(Mathf.CeilToInt(target.position.x), Mathf.Round(original.y), Mathf.Round(original.z));
        Offset = new Vector2((transform.position.x - original.x) *Speed, Offset.y);
        SpriteRenderer.material.SetTextureOffset("_MainTex", Offset);
    }
}
