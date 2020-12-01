using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
	public static event System.Action GamePause;
	public static event System.Action GameUnpause;
	//telas que aparecem no canvas durante o jogo
	public GameObject telaDerrota;
	public GameObject telaVitoria;
	public GameObject pauseScreen;

	public GameObject player;
	bool gameIsOver;
	bool gameIsPaused = false;
	// Use this for initialization
	
	void Start () {
		//os npcs são capazes de chamar o metodo que acaba com o jogo
		Guard.GuardaViuPlayer += MostrarTelaDerrota;
		Helicoptero.GuardaViuPlayer += MostrarTelaDerrota;
		FindObjectOfType<Player>().TocouObjetivo += MostrarTelaVitoria;
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(gameIsOver){
			//se o jogo acaba, verifica se o jogador ganhou a fase ou foi avistado e reage conforme o resultado
			if(Input.GetKeyDown(KeyCode.Space)){
				if(telaVitoria.active){
					if(SceneManager.GetActiveScene().buildIndex == 1){
						SceneManager.LoadScene(0);
					}else{
						SceneManager.LoadScene(1);
					}
				}else if(telaDerrota.active){
					SceneManager.LoadScene(0);
				}
			}
		}else{
			//para que o jogo não seja "pausado" durante uma tela de vitoria ou derrota, o else
			if(Input.GetKeyDown(KeyCode.P)){
				if(!gameIsPaused) {
					gameIsPaused = true;
					GamePaused(pauseScreen, gameIsPaused);
					Time.timeScale = 0;
				}else{
					gameIsPaused = false;
					GamePaused(pauseScreen, gameIsPaused);
					Time.timeScale = 1;
				}
			}
		}
	}
	//mostra a tela de pause
	void GamePaused(GameObject pauseScreen, bool gameIsPaused){
		if(gameIsPaused){
			pauseScreen.SetActive(true);
			GamePause();
		}else{
			pauseScreen.SetActive(false);
			GameUnpause();
		}
	}

	//mostra a tela de vitoria ou de derrota e acaba com o jogo/fase
	void GameOver(GameObject gameOverScreen){
		gameOverScreen.SetActive(true);
		gameIsOver = true;
	}

	void MostrarTelaDerrota(){
		GameOver(telaDerrota);
		FindObjectOfType<Player>().TocouObjetivo -= MostrarTelaVitoria;
		Guard.GuardaViuPlayer -= MostrarTelaDerrota;
		Helicoptero.GuardaViuPlayer -= MostrarTelaDerrota;
	}

	void MostrarTelaVitoria(){
		GameOver(telaVitoria);
		FindObjectOfType<Player>().TocouObjetivo -= MostrarTelaVitoria;
		Guard.GuardaViuPlayer -= MostrarTelaDerrota;
		Helicoptero.GuardaViuPlayer -= MostrarTelaDerrota;

	}

}
