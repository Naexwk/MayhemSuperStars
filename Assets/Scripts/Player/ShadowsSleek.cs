using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowsSleek : MonoBehaviour
{

    public static ShadowsSleek me;
    public GameObject shadow;
    public List<GameObject> pool = new List<GameObject>();
    private float timer;
    public float speed;
    public Color _color;
    // Start is called before the first frame update
    void Awake()
    {
        me= this;
    }

    public GameObject GetShadows()
    {
        Debug.Log("Dashing");
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                pool[i].transform.position = transform.position;
                pool[i].transform.rotation = transform.rotation;
                //pool[i].GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                //pool[i].GetComponent<SolidSprite>()._color = _color;
                return pool[i];
            }
        }
        GameObject obj = Instantiate(shadow, transform.position, transform.rotation) as GameObject;
        //bj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        //obj.GetComponent<SolidSprite>()._color = _color;
        pool.Add(obj);
        return obj;
    }

    public void shadow_skill()
    {
        timer += speed * Time.deltaTime;
        if (timer > 1)
        {
            GetShadows();
            timer = 0;
        }
    }
}
