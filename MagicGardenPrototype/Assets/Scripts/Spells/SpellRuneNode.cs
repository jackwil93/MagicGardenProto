using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpellRuneNode : MonoBehaviour {

    public ParticleSystem particleEffect;
    public Transform particleGroup;

    private void Start()
    {
        particleEffect = particleGroup.GetChild(transform.GetSiblingIndex()).GetComponent<ParticleSystem>();
    }

    public void HitRune()
    {
        this.GetComponent<Image>().color = Color.cyan;

        if (particleEffect.isPlaying == false)
            particleEffect.Play();
    }

    public void ResetRune()
    {
        this.GetComponent<Image>().color = Color.white;
    }
}
