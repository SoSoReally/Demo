using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatFoam : MonoBehaviour {

    public RectTransform m_foam;
    //数量
    public int m_sumber;
    List<RectTransform> list_foam = new List<RectTransform>();

    [HideInInspector]
    public List<GameObject> list_foamHide = new List<GameObject>();

    float width;

    float height;

    Rect rect;
	// Use this for initialization
	void Start () {
        rect= transform.root.GetComponent<RectTransform>().rect;
        width = rect.width;
        height = rect.height;
        RandomCreatFoam(width,height,m_sumber);
	}
	
	// Update is called once per frame
	void Update () {
		


	}
    void RandomCreatFoam(float width,float height,int sumber)
    {

        //foam 半径
        float radiu_x = m_foam.rect.width;
        float radiu_y = m_foam.rect.height;
        width = Mathf.Floor(width / radiu_x)-1f;
        height = Mathf.Floor(height / radiu_y)-1f;

        //坐标点区间
        Vector2[] vec2array = new Vector2[(int)(width*height)];
        for (int i = 0; i < (int)width; i++)
        {
            for (int j = 0; j < (int)height; j++)
            {
                vec2array[i*(int) height + j] = new Vector2(i, j);
                
            }
        }

        //区间数量大于生成数量?
        sumber = Mathf.Min(sumber, vec2array.Length);

        for (int i = 0; i < sumber; i++)
        {
            //不重复随机
            int index = Random.Range(0,vec2array.Length - i - 1);
            Vector2 vec2 = vec2array[index];
            vec2array[index] = vec2array[vec2array.Length - i - 1];
            vec2array[vec2array.Length - i - 1] = vec2;

            //
            list_foam.Add(Instantiate(m_foam, new Vector3(vec2.x*radiu_x+radiu_x, vec2.y*radiu_y+radiu_y, 0f), Quaternion.identity,transform));
            float radius = Random.Range(15f, 50f);
            list_foam[i].sizeDelta = new Vector2(radius, radius);
            list_foam[i].GetComponent<CircleCollider2D>().radius = radius/2f;
        }
    }
}
