using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    Animator animator;

    [SerializeField]
    CharacterController controller;

    [SerializeField]
    Transform coinPrefab;

    [SerializeField]
    Light flashlightLight;

    [SerializeField]
    float speed = 12f;

    [SerializeField]
    LayerMask groundMask;

    [SerializeField]
    LayerMask enemyLayer;

    public MouseLook mouseLookScript = null;

    CoinDisplay coinDisplay;

    AudioManager audioManager = null;

    int coins = 0;

    WorldState worldState = null;

    bool coinJiggleEnabled = false;

    public void KillPlayer() {
        // Disable enemies
        for (int i = 0; i < 4; ++i) {
            WorldState.enemies[i].GetComponent<EnemyController>().enabled = false;
            WorldState.enemies[i].GetComponent<Animator>().enabled = false;
        }

        // Disable player
        this.enabled = false;
        this.animator.SetFloat("Speed", 0);
        this.audioManager.Stop("CoinBagShake");
        mouseLookScript.enabled = false;

        // Ragdoll

        // Show death message
        // Show score
    }

    public void addCoin() {
        ++coins;
        coinDisplay.SetCoinCount(coins);

        if (!worldState.isInDoomMode() && coins >= 3) {
            worldState.startDoomMode();
        }
    }

    public void removeCoin() {
        if (coins > 0) {
            // Physically drop coin
            Transform coin = Instantiate(coinPrefab);

            if (Physics.Raycast(transform.position, transform.forward * -3, 1)) {
                coin.position = transform.position + (transform.forward * 3) + (transform.up * 0.8f);
            }
            else {
                coin.position = transform.position + (transform.forward * -3) + (transform.up * 0.8f);
            }

            // Remove coin
            --coins;
            coinDisplay.SetCoinCount(coins);
        }
    }

    private void Start() {
        coinDisplay = FindObjectOfType<CoinDisplay>();
        worldState = FindObjectOfType<WorldState>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update() {
        if (!WorldState.IsPaused()) {
            speed = getSpeed(coins);

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = ((transform.right * x + transform.forward * z).normalized / 2) * speed;

            if (Input.GetButton("Sprint") && z > 0) { // Moving forward, not backwards
                move *= 2;

                if (!coinJiggleEnabled && coins >= 2) {
                    coinJiggleEnabled = true;
                    audioManager.Play("CoinBagShake");
                }

                animator.SetBool("Sprint", true);
            }
            else {
                if (coinJiggleEnabled) {
                    coinJiggleEnabled = false;
                    audioManager.Stop("CoinBagShake");
                }

                animator.SetBool("Sprint", false);
            }

            if (coinJiggleEnabled) {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 35, enemyLayer);
                foreach (var hitCollider in hitColliders) {
                    hitCollider.GetComponent<EnemyController>().SetTarget(transform.position);
                }
            }

            if (z < 0) {
                move *= 0.4f;
            }

            animator.SetFloat("Speed", move.magnitude);

            controller.Move(move * Time.deltaTime);

            if (Input.GetButtonDown("Drop")) {
                removeCoin();
            }

            if (Input.GetButtonDown("Flashlight")) {
                flashlightLight.enabled = !flashlightLight.enabled;
                audioManager.Play("Flashlight");
            }

            if (Input.GetButtonDown("Cancel")) {
                worldState.PauseGame();
            }
        }
        else {
            animator.SetFloat("Speed", 0);
        }
    }

    float getSpeed(int coins) {
        return 12f + Mathf.Round(Mathf.Exp((coins - 14.9f) / -6) - 12);
    }

}
