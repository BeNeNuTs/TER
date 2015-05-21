using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/** Classe permettant de gérer les mises à jour des sliders dans la vue Editeur */ 
public class SliderController : MonoBehaviour {

	public Text textSlider;

	private Slider slider;

	/** Initialise le slider */
	void Start () {
		slider = GetComponent<Slider>();
		if(slider == null){
			Debug.LogError("Impossible de récupèrer le slider == null");
		}
	}
	
	// Mise à jour du texte affiché sur le slider */
	public void UpdateSlider () {
		textSlider.text = slider.value.ToString();
	}
}
