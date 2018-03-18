using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI; //สามารถควบคุม ui ได้

public class CountDown : MonoBehaviour {	
	public Text text; //สร้างตัวแปร t เป็นตัวแทนกล่องข้อความ
	private float n = 0; //สร้างตัวแปร n เป็นตัวเลขแบบมีจุดทศนิยม

    public event Action OnTimesUpE;

    void Start()
    {
        if (!text) //ถ้าหากล่องข้อความไม่เจอ หรือไม่ได้ลากใส่ไว้
        {
            text = GetComponent<Text>();
            if (!text) //ถ้ายังหาไม่เจอให้ปิดการทำงานของสคริปต์นี้ไว้ เพื่อกันเออเรอร์
            {
                this.enabled = false;
            }
        }

    }

    public void SetTime(float time)
    {
        if (time < 0) return;
        n = time;
        UpdateText();
    }
    public void StartTimer(float time = 0)
    {
        if (time > 0)
        {
            n = time;
        }
        this.enabled = true;
    }

    private void UpdateText()
    {
        // t.text = System.Math.Round(n,2).ToString(); //ทศนิยม 2 ตำแหน่ง
        text.text = Mathf.Round(n).ToString(); //จำนวนเต็ม
    }
	
	// Update is called once per frame
	void Update () {
	    if (n > 0)
	    {
	        n -= Time.deltaTime; // ให้ n มีค่าค่อยๆลดไปทีละ 1 ภายใน 1 วินาที
	        
	        UpdateText();

            if (n <= 0)
	        {
	            text.text = "0";
	            if (OnTimesUpE != null)
	            {
	                OnTimesUpE();
	            }
	            this.enabled = false;
	        }
        }
	}
}
