using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {


    RectTransform m_rectransform;

    GameObject clone;

    CreatFoam m_creatFoam;

    CircleCollider2D m_mainCirlce2d;

    List<CircleCollider2D> list_Cirlce2d;

    Rigidbody2D m_rig2d;
    [SerializeField]
    float pixRadiu = 5f;

    public float speed=0f;

    float random = 0;

    Animation m_ani;

    bool isBoo;
    bool isboo1;
    private void Start()
    {
       
        m_ani = GetComponent<Animation>();
        m_rig2d = GetComponent<Rigidbody2D>();
        list_Cirlce2d = new List<CircleCollider2D>();
        m_mainCirlce2d = GetComponent<CircleCollider2D>();
        list_Cirlce2d.AddRange(transform.GetComponents<CircleCollider2D>());
        list_Cirlce2d.RemoveAt(0);
        m_rectransform = GetComponent<RectTransform>();
        m_creatFoam = transform.parent.GetComponent<CreatFoam>();

        random = Random.Range(0f, 100f);
    }
    // Update is called once per frame
    void Update () {
        ScreenPoint();
        Move();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (!m_rig2d)
            {
                return;
            }
         
        }
      
    }

    void ScreenPoint()
    {

        Vector3 pos = new Vector3();
        if (isBoo= IsCloneRange(out pos))
        {
            if (transform.childCount>0)
            {
                transform.GetChild(0).position = pos;

                list_Cirlce2d[list_Cirlce2d.Count - 1].offset = pos - transform.position;

                transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = m_rectransform.sizeDelta;
            }
            else
            {
                clone = GetGameObject();
                clone.transform.position = new Vector3(pos.x, pos.y);
                InitializationClone(clone);
            }
        }
        if (isboo1 =!IsRangVisualized())
        {
            clone.transform.parent = m_creatFoam.transform;
            clone.SetActive(false);
            m_creatFoam.list_foamHide.Add(clone);
           // clone.GetComponent<CheckPoint>().enabled = true;
            gameObject.transform.position = clone.transform.position;
            gameObject.transform.rotation = clone.transform.rotation;
            clone = null;
           
        }
        if (!isBoo &&!isboo1)
        {
           
        }
     
    }
    //克隆区间,out 克隆坐标
    bool IsCloneRange(out Vector3 clone_pos)
    {
        clone_pos = new Vector3();
        Rect rect = m_rectransform.rect;
        rect.width -= pixRadiu * 2f;
        rect.height -= pixRadiu * 2f;
        Vector3 pos = m_rectransform.position;
        float min_x = rect.width / 2f;
        float max_x = Screen.width - rect.width / 2f;
        float min_y = rect.height / 2f;
        float max_y = Screen.height - rect.height / 2f;
        clone_pos = pos;
        bool result = false;
        if (pos.x < min_x)
        {
            clone_pos.x = pos.x + Screen.width;
            result = true;
        }
        if (pos.y < min_y)
        {
            clone_pos.y = pos.y + Screen.height;
            result = true;
        }
        if (pos.x > max_x)
        {
            clone_pos.x = pos.x - Screen.width;
            result = true;
        }
        if (pos.y > max_y)
        {
            clone_pos.y = pos.y - Screen.height;
            result = true;
        }
        return result;

    }

    bool IsRangVisualized()
    {
        Rect rect = m_rectransform.rect;
        float min_x = -rect.width;
        float min_y = -rect.height;
        float max_x = Screen.width + rect.width;
        float max_y = Screen.height + rect.height;
        Vector3 pos = m_rectransform.position;
        if (pos.x<min_x||pos.x>max_x||pos.y<min_y||pos.y>max_y)
        {
            return false;
        }
        return true;
    }

    GameObject GetGameObject()
    {
        GameObject go;
        var list = m_creatFoam.list_foamHide;
        if (list.Count>0)
        {
            go = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
        else
        {
            go = Instantiate(transform.gameObject);
            list.Add(go);
        }
        return go;
    }

    void InitializationClone(GameObject clone)
    {
        clone.SetActive(true);

        clone.transform.rotation = transform.rotation;

        clone.transform.parent = transform;

        clone.transform.GetComponent<CircleCollider2D>().enabled = false;

        clone.transform.GetComponent<RectTransform>().sizeDelta = m_rectransform.sizeDelta;

        var vec3 = clone.transform.position - transform.position;

        CircleCollider2D[]  circle2d_array = clone.GetComponents<CircleCollider2D>();

        foreach (var item in circle2d_array)
        {
            item.enabled = false;
        }
        if (list_Cirlce2d.Count<=0)
        {
            list_Cirlce2d.Add(gameObject.AddComponent<CircleCollider2D>());
        }
        list_Cirlce2d[list_Cirlce2d.Count - 1].radius = m_mainCirlce2d.radius;
        list_Cirlce2d[list_Cirlce2d.Count - 1].offset = vec3;
        clone.GetComponent<CheckPoint>().enabled = false;
    }



    void Move()
    {
        if (m_rig2d.velocity==Vector2.zero)
        {
            float x = transform.position.x * Mathf.Sin(Time.time+random);
            float y = transform.position.y * Mathf.Cos(Time.time+random);
            Vector2 vec2 = new Vector2(x, y);
            m_rig2d.AddForce(vec2 * speed );
        }
    }




}
