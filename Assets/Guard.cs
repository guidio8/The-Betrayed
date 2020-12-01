using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {
	public static event System.Action GuardaViuPlayer;

	public Transform caminho;
	public Transform player;
	//atributos de movimento
	public float velocidade = 5;
	public float tempoEspera = .3f;
	//atributos para saber se o player foi avistado
	public LayerMask viewMask;
	public float tempoMaxAvistado = .5f;
	private float tempoSendoAvistado;
	public float distanciaVisao;
	float campoDeVisao;

	public Light spotlight;
	Color originalSpotlightColor;
	
	void Start(){
		originalSpotlightColor = spotlight.color;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		campoDeVisao = spotlight.spotAngle;
		//cria um vetor com o caminho que cada npc vai fazer, definido na IDE com empty game objects
		Vector3 [] paradas = new Vector3[caminho.childCount];
		for(int i = 0; i < paradas.Length; i++){
			paradas[i] = caminho.GetChild(i).position;
		}
		StartCoroutine(Andar(paradas));
	}

	void Update(){
		//verifica se o player está dentro do campo de visão, e em caso positivo muda a cor do spotlight
		if (CanSeePlayer()){
			tempoSendoAvistado += Time.deltaTime;
			spotlight.color = Color.red;
		}else{
			tempoSendoAvistado -= Time.deltaTime;
			spotlight.color = originalSpotlightColor;
		}
		tempoSendoAvistado = Mathf.Clamp(tempoSendoAvistado, 0, tempoMaxAvistado);

		if(tempoSendoAvistado >= tempoMaxAvistado){
			if(GuardaViuPlayer != null){
				GuardaViuPlayer();
			}
		}
	}

	bool CanSeePlayer(){
		if(Vector3.Distance(transform.position, player.position) < distanciaVisao){
			Vector3 dirToPlayer = (player.position - transform.position).normalized;
			float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);
			if (angleBetweenGuardAndPlayer < campoDeVisao / 2f){
				if(!Physics.Linecast(transform.position, player.position, viewMask)){
					return true;
				}
			}
		}
		return false;
	}
	//coroutine para fazer o npc ir de um waypoint para o proximo com a velocidade pré-definida
	IEnumerator Andar(Vector3[] paradas){
		transform.position = paradas[0];
		int paradaIndex = 1;
		Vector3 proximaParada = paradas[paradaIndex];
		transform.LookAt(proximaParada);
		while(true){
			transform.position = Vector3.MoveTowards(transform.position, proximaParada, velocidade * Time.deltaTime);
			if(transform.position == proximaParada){
				paradaIndex = (paradaIndex + 1)% paradas.Length;
				proximaParada = paradas[paradaIndex];
				yield return new WaitForSeconds(tempoEspera);
				yield return StartCoroutine(Girar(proximaParada));
			}
			yield return null;
		}
	}
	//sempre que o npc chega em um dos waypoints ele vira para o próximo antes de começar a andar de novo
	IEnumerator Girar(Vector3 direcaoOlhar){
		Vector3 direcaoObjetivo = (direcaoOlhar - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2(direcaoObjetivo.z, direcaoObjetivo.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05){
			float angulo = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, /*TURN SPEED*/90 * Time.deltaTime);
			transform.eulerAngles = Vector3.up * angulo;
			yield return null;
		}
	}

	void OnDrawGizmos() {
		foreach (Transform parada in caminho){
			Gizmos.DrawSphere(parada.position, .3f);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward * distanciaVisao);
	}
}
