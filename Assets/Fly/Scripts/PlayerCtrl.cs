using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerCtrl : MonoBehaviour {
    Rigidbody2D rbody;
    ParticleSystem boomParticle;

    public Vector2 speedXY = new Vector2(0.5f, 5f);
    public float maxUpSpeed = 30;
    public Text scoreText;

    GameManager manager;
    int score = 0;
	// Use this for initialization
	void Start () {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        rbody = GetComponent<Rigidbody2D>();
        boomParticle = GetComponent<ParticleSystem>();
        
    }
	
    void FixedUpdate()
    {
        if(rbody.isKinematic != true)
        {
            rbody.velocity = new Vector2(Time.deltaTime * speedXY.x, rbody.velocity.y);

            if (Input.GetAxis("Fire1") > 0.001)
            {
                if (rbody.transform.position.y >= 4.0f)
                {
                    rbody.velocity = new Vector2(rbody.velocity.x, 0f);
                }
                else
                {
                    float force = rbody.velocity.y < maxUpSpeed ? speedXY.y * Time.deltaTime : 0;
                    rbody.velocity = new Vector2(rbody.velocity.x, rbody.velocity.y + force);
                }

            }
            rbody.MoveRotation(Mathf.LerpAngle(rbody.transform.rotation.eulerAngles.z, rbody.velocity.y / maxUpSpeed * 60, 1f));
            
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Obstacle"))
        {
            boomParticle.Play();
            GetComponent<PlayerCtrl>().enabled=false;
            GetComponent<SpriteRenderer>().enabled = false;
            rbody.isKinematic = true;
            //GameObject.Destroy(gameObject, 2f);
            if(PlayerPrefs.GetInt("Score") < score)
            {
                PlayerPrefs.SetInt("Score", score);
                manager.GameOver("刷新纪录！");
            }
            else
            {
                manager.GameOver("游戏结束！");
            }
        }
        else if (collider.CompareTag("Star"))
        {
            collider.GetComponent<SpriteRenderer>().enabled = false;
            collider.GetComponent<ParticleSystem>().Play();
            GameObject.Destroy(collider.gameObject, 1f);
            score++;
            scoreText.text = string.Format("Score:{0}", score);
        }
    }

    public void Unready()
    {
        rbody.isKinematic = true;
    }

    public void Ready()
    {
        rbody.isKinematic = false;
    }  
}
