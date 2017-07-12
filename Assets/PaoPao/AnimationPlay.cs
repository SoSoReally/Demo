using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlay : MonoBehaviour {

   RectTransform m_rectransform;

    public InputDir m_inputDir;
    // Use this for initialization
    void Start () {

        m_rectransform = GetComponent<RectTransform>();
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
    public bool isPlay = false;
    public float curve_time = 0f;
    public IEnumerator Play()
    {
        if (!m_rectransform)
        {
            yield return new WaitForEndOfFrame();
        }
        isPlay = true;
        float time = 0.15f * dua;
        curve_time = 0f;
        float current_time = Time.time;
        float size_x = m_rectransform.sizeDelta.x;
        float size_y = m_rectransform.sizeDelta.y;
        while (Time.time < current_time + time)
        {
            yield return new WaitForEndOfFrame();
            float x = size_x * m_inputDir.ac_x.Evaluate(curve_time / dua);
            float y = size_y * m_inputDir.ac_y.Evaluate(curve_time / dua);
            m_rectransform.sizeDelta = new Vector2(x, y);
            curve_time += Time.deltaTime;

        }

        m_rectransform.sizeDelta = new Vector2(size_x, size_y);
        isPlay = false;
    }
}
