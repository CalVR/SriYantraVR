using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Utils;

// connects the view to the animation system
public class YantraController : MonoBehaviour {

	[SerializeField] private SelectionSlider toggleSlider;				// slider to play/pause animation
	[SerializeField] private UnityEngine.UI.Text toggleSliderText;		// text of play/pause slider
	[SerializeField] private SelectionSlider resetSlider;				// slider to reset animation
	[SerializeField] private YantraAnimator m_YantraAnimator;           // yantra animator class

    [SerializeField] private SelectionSlider infoSlider;				// slider to configure setting
    [SerializeField] private GameObject infoCanvas;              // setting canvas
	[SerializeField] private SelectionSlider resetConfigSlider;

	// Added for config by controller
    private GameObject world;
    private GameObject background;
    private GameObject overlay;
    private GameObject infoText;
    private Vector3 defaultPosition, defaultScale;
	private bool settingMode; 					// if true, enable configuration by controller
	private bool isMute = false;
	private bool clicked = false;  
	private bool swiping = false;                     
	private float lastClickTime; 
	private Vector2 lastTouchPos = new Vector2(0, 0);

	public bool isOverButton = false; 

	private void Start()
    {
        world = GameObject.Find("World");
        background = GameObject.Find("World/Sri Yantra/Background");
        overlay = GameObject.Find("World/Sri Yantra/Overlay");
		infoText = infoCanvas.transform.Find("Slider/Text").gameObject;
        defaultPosition = world.transform.localPosition;
        defaultScale = world.transform.localScale;
        settingMode = true;
    }

    private void OnEnable()
	{
		toggleSlider.OnBarFilled += toggleAnimation;
		resetSlider.OnBarFilled += resetAnimation;

        infoSlider.OnBarFilled += toggleInfo;
		resetConfigSlider.OnBarFilled += resetToDefault;
    }

	private void OnDisable()
	{
		toggleSlider.OnBarFilled -= toggleAnimation;
		resetSlider.OnBarFilled -= resetAnimation;

        infoSlider.OnBarFilled -= toggleInfo;
		resetConfigSlider.OnBarFilled -= resetToDefault;
    }

	// this is moved from animator
	void Update () 
	{
		if(m_YantraAnimator.isAnimating()) {
			bool isPlaying = m_YantraAnimator.animate();
			if (!isPlaying)
				resetAnimation();
		}
		controllerInput();
	}

	void controllerInput() {
		// config by swiping
		if (settingMode) {
			/*
			// ver 2: by swipe, smooth zooming
			if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad)){
				Vector2 touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
				if (swiping && Math.Abs(Vector2.Distance(touchPosition, lastTouchPos)) > 0.05f) {
					float x = touchPosition.x - lastTouchPos.x;
					float y = touchPosition.y - lastTouchPos.y;
					if (Math.Abs(x) > Math.Abs(y)) {
						world.transform.localScale += new Vector3(0.125f * x, 0.125f * x, 0);
					}
					else {
						world.transform.localPosition += new Vector3(0, 0.5f * y, 0);
					}
				}
				swiping = true;
				lastTouchPos = touchPosition;
			}
			else {
				swiping = false;
			}
			/*/
			// ver 3: by touch
			if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad)){
				Vector2 touchPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
				if (Math.Abs(Vector2.Distance(touchPosition, new Vector2(0,0))) > 0.5f) {
					float x = touchPosition.x;
					float y = touchPosition.y;
					if (Math.Abs(x) > Math.Abs(y)) {
						world.transform.localScale += new Vector3(0.01f * x, 0.01f * x, 0);
					}
					else {
						world.transform.localPosition += new Vector3(0, 0.05f * y, 0);
					}
				}
			}
			/*/
			// ver 1: by swipe, discrete zooming
			if (OVRInput.Get(OVRInput.Button.DpadUp))
            {
                world.transform.localPosition += new Vector3(0, 0, 0.5f);
            }
            if (OVRInput.Get(OVRInput.Button.DpadDown))
            {
            	world.transform.localPosition -= new Vector3(0, 0, 0.5f);
            }
            if (OVRInput.Get(OVRInput.Button.DpadLeft))
            {
                world.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            }
            if (OVRInput.Get(OVRInput.Button.DpadRight))
            {
                world.transform.localScale += new Vector3(0.05f, 0.05f, 0);
            }

			 */
		}

		// do not do any configuration by click if hitting the button
		if (isOverButton)
			return;

		bool isSingleClick = false;
		// check if there is a second click
		if (Time.time - lastClickTime < 0.3f)
		{
			// is double click
			if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad))// || Input.GetButtonDown("Fire1"))
        	{
				settingMode = !settingMode;
				if (settingMode)
					infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Setting Config: Disabled", "Setting Config: Enabled");
				else
					infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Setting Config: Enabled", "Setting Config: Disabled");
				clicked = false;
				return;
			}
			
		} else { 
			// is single click
			if (clicked) {
				isSingleClick = true;
				clicked = false;
			}
		}
		// check click
		if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad) )// || Input.GetButtonDown("Fire1"))
        {
        	lastClickTime = Time.time;
			clicked = true;
		}
       
		if (settingMode) {
			if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) {
                background.SetActive(!background.activeSelf);
                overlay.SetActive(!overlay.activeSelf);
				if (overlay.activeSelf)
					infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Marker: Disabled", "Marker: Enabled");
				else
					infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Marker: Enabled", "Marker: Disabled");
			}
			if (isSingleClick) {
				isMute = !isMute;
				m_YantraAnimator.setIsMute(isMute);
				if (!isMute)
					infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Audio: Disabled", "Audio: Enabled");
				else
					infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Audio: Enabled", "Audio: Disabled");
			}
		}
	}

	void toggleAnimation()
	{
		if (m_YantraAnimator.isAnimating ()) {
			m_YantraAnimator.pause();
			toggleSliderText.text = "Play";
		}
		else {
			m_YantraAnimator.play();
			toggleSliderText.text = "Pause";
		}
	}
		
	void resetAnimation()
	{
		m_YantraAnimator.reset ();
        m_YantraAnimator.play(); 
		//toggleSliderText.text = "Play";    //comment the above line and uncomment this to disable looping
	}

    void toggleInfo()
    {
        //settingMode = !settingMode;
        infoCanvas.SetActive(!infoCanvas.activeSelf);
    }

	void resetToDefault() 
	{
		world.transform.localPosition = defaultPosition;
		world.transform.localScale = defaultScale;

        background.SetActive(true);
        overlay.SetActive(true);
		isMute = false;
		m_YantraAnimator.setIsMute(isMute);

		infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Marker: Disabled", "Marker: Enabled");
		infoText.GetComponent<Text>().text = infoText.GetComponent<Text>().text.Replace("Audio: Disabled", "Audio: Enabled");
				
	}

}
