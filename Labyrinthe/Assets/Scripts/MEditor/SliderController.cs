using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderController : MonoBehaviour {

	public Text textSlider;

	private Slider slider;

	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider>();
		if(slider == null){
			Debug.LogError("Impossible de récupèrer le slider == null");
		}
	}
	
	// Update is called once per frame
	public void UpdateSlider () {
		textSlider.text = slider.value.ToString();
	}
}
