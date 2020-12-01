using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public event System.Action TocouObjetivo;

	//variaveis de movimento
	public float velocidade = 7f;
	public float gravidade = -9.8f;
	public float distanciaDoGuarda;
	public float distanciaDoChao = 0.4f;
	public CharacterController controller;
	public Transform groundCheck;
	public LayerMask groundMask;
	bool isGrounded;
	bool desativarControles;
	Vector3 aceleracao;

	//interação
	public GameObject passagemSecretaOff;
	public GameObject passagemSecretaOn;
	GameObject chave;
	public int distanciaPassagem = 2;

	bool temChave = false;


	void Start(){
		chave = GameObject.FindGameObjectWithTag("Chave");
		//metodos de outra classe que são capazes de desativar o movimento do player
		Guard.GuardaViuPlayer += Disable;
		Helicoptero.GuardaViuPlayer += Disable;
		GameUI.GamePause += Disable;
		GameUI.GameUnpause += ReativarControles;
	}

	// Update is called once per frame
	void Update () {
		isGrounded = Physics.CheckSphere(groundCheck.position, distanciaDoChao, groundMask);

		if(isGrounded && aceleracao.y < 0){
			aceleracao.y = -2f;
		}

		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		Vector3 inputDirection = transform.right * x + transform.forward * z;
		if(!desativarControles){
			controller.Move(inputDirection * velocidade * Time.deltaTime);
			controller.Move(aceleracao * Time.deltaTime);
		}

		aceleracao.y += gravidade * Time.deltaTime;
		//metodo para interagir com algo
		Interagir();
		//para testar o jogo durante a build ou durante o jogo sem precisar ficar mexendo na hierarquia
		if(Input.GetKeyDown(KeyCode.AltGr)){
			GameObject.FindGameObjectWithTag("Player").layer = 9;
		}
		if(Input.GetKeyDown(KeyCode.CapsLock)){
			GameObject.FindGameObjectWithTag("Player").layer = 0;
		}
	}
	
	void OnTriggerEnter(Collider hitCollider){
		if(hitCollider.tag == "Finish"){
			Disable();
			if(TocouObjetivo != null){
				TocouObjetivo();
			}
		}
		if(hitCollider.tag == "PassagemSecreta"){
			passagemSecretaOn.SetActive(!passagemSecretaOn.activeInHierarchy);
			passagemSecretaOff.SetActive(!passagemSecretaOff.activeInHierarchy);
		}
	}

	void Interagir(){
		if(Input.GetKeyDown(KeyCode.Space)){
			if(Vector3.Distance(transform.position, chave.transform.position) < distanciaDoGuarda){
				temChave = true;
				Debug.Log("Pegou Chave");
			}
		}
		//para não ter que trocar de scene, quando o player esta perto o suficiente da porta "interagivel" ele se teletransporta
		//pra onde começa o próximo desafio
		if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("TP1").transform.position) < 2 && temChave){
			transform.position = new Vector3(29.32438f, 1.08f ,4.560293f);
		}
	}

	void ReativarControles(){
		desativarControles = false;
	}
	
	void Disable(){
		desativarControles = true;
	}

	void OnDestroy(){
		Guard.GuardaViuPlayer -= Disable;
		Helicoptero.GuardaViuPlayer -= Disable;
	}
}
