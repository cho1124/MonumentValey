using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    public Rigidbody2D rig;
    LayerMask layer;

    float  f = 0;
    void Update()
    {
        int layerindex = layer.value;
        f += Time.deltaTime;
        if(f > .2f)
        {

        
            Rigidbody2D r = Instantiate(rig, transform.position, Quaternion.identity, transform);
            r.velocity = Vector2.zero;
            r.AddForce(Random.onUnitSphere * 200, ForceMode2D.Impulse);
            Destroy(r.gameObject, 3f);
            f = 0;
        }
    }
}
