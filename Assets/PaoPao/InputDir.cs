using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent,RequireComponent(typeof(Rigidbody2D))]
public class InputDir : MonoBehaviour {


    Rigidbody2D m_rig2d;

    RectTransform m_rectransform;

    float radiu_x = 0f;

    float radiu_y = 0f;

    Animation m_ani;
    public float m_speed;

    public AnimationCurve ac_x;
    public AnimationCurve ac_y;
    // Use this for initialization
    void Start () {
        m_rig2d = GetComponent<Rigidbody2D>();
        m_rectransform = GetComponent<RectTransform>();
        radiu_x= m_rectransform.rect.width / 2f;
        radiu_y = m_rectransform.rect.height / 2f;
        m_ani = GetComponent<Animation>();
    }
 

    // Update is called once per frame
    void Update () {

       
        UpdateSpeed();
    }

    Vector3 oldposition = Vector3.zero;
    private void FixedUpdate()
    {
        UpdateMouse();
    }
    void UpdateMouse()
    {
        Vector3 vec3 = Input.mousePosition;
        vec3.x = Mathf.Clamp(vec3.x,radiu_x, Screen.width-radiu_x);
        vec3.y = Mathf.Clamp(vec3.y, radiu_y, Screen.height-radiu_y);
        // Vector3 dir = vec3 - oldposition;
        // m_rig2d.AddForce(dir* m_speed);
       m_rig2d.MovePosition(vec3);
       
       //transform.position = vec3;
       // oldposition  = vec3;
    }
    void UpdateSpeed()
    {
        m_speed = Vector3.Distance(oldposition, transform.position);

        m_rig2d.AddForce(transform.position - oldposition.normalized);
        oldposition = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (!isPlay)
        {
            StartCoroutine(Play());
        }
    }
   
    [SerializeField]
     float dua = 0f;
    bool isPlay = false;
    public IEnumerator Play( )
    {
        isPlay = true;
        float time = 0.15f* dua;
        float curve_time = 0f;
        float current_time = Time.time;
        float size_x = m_rectransform.sizeDelta.x;
        float size_y = m_rectransform.sizeDelta.y;
        while (Time.time<current_time+time)
        {
            yield return new WaitForEndOfFrame();
            float x = size_x* ac_x.Evaluate(curve_time/dua);
            float y = size_y*ac_y.Evaluate(curve_time/dua);
            m_rectransform.sizeDelta = new Vector2(x, y);
            curve_time += Time.deltaTime;

        }
       
        m_rectransform.sizeDelta = new Vector2(size_x, size_y);
        isPlay = false;
    }
    
}
