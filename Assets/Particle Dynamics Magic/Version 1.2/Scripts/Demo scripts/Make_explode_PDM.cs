using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Artngame.PDM;

namespace Artngame.PDM {

public class Make_explode_PDM : MonoBehaviour {

	void Start () {
		HERO_transform = HERO.transform;
		thisTransform=this.gameObject.transform;

		Particles = GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
		Attractor_script = GetComponent("AttractParticles") as AttractParticles;

		wait = Time.fixedTime;

	}

	PlaceParticleOnSpline[] TentaclesS;

	public List<GameObject> Tentacles;

	ParticleSystem Particles;
	AttractParticles Attractor_script;

	Transform HERO_transform;
	Transform thisTransform;
	public GameObject HERO;

	bool appeared = false;

	float wait;
	
	void Update () {

		if(Attractor_script!=null){
	
		if( Vector3.Distance(HERO_transform.position, thisTransform.position) < 5f & !appeared){

			Attractor_script.dumpen = 0.8f;
			Particles.startLifetime = 5f;

		appeared = true;

			wait = Time.fixedTime;

			Tentacles[0].SetActive(true);


		}


		if(appeared & (Time.fixedTime-wait>3) ){

			Attractor_script.enabled=false;

			//v2.1
			ParticleSystem.EmissionModule em = Particles.emission;
			em.enabled = false;
			//Particles.enableEmission=false;
			Particles.Clear();
			Particles.Stop();
		}else if (appeared){

			Particles.startLifetime = Particles.startLifetime+0.1f;
		}




	}

	}
}
}