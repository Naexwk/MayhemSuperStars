using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidSprite : MonoBehaviour
{
    private SpriteRenderer myRender;
    private Shader myMaterial;
    public Color _color;

    // Start is called before the first frame update
    void Start()
    {
        myRender = GetComponent<SpriteRenderer>();
        myMaterial = Shader.Find("GUI/Text Shader");
    }

    void ColorSprite()
    {
        myRender.material.shader = myMaterial;
        myRender.color = _color;
    }

    public void Finish()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ColorSprite();
    }
}
