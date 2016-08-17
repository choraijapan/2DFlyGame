using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
    [HideInInspector]
    public bool gameIsStart;

    public Camera camera;
    public GameObject player;
    public GameObject bg;
    public GameObject menu;
    public GameObject dialog;
    public GameObject topDialog;
    public GameObject[] ground;
    public GameObject[] obstaclePrefabs;

    Animator menuAnim;
    Material bgMat;
    float groundDistance;
    ArrayList obstacles;
    
    PlayerCtrl playerCtrl;
    CameraFollow cameraFollow;
    float halfWinSize;
	void Start () {

        dialog.SetActive(false);
        topDialog.SetActive(false);

        obstacles = new ArrayList();
        menuAnim = menu.GetComponent<Animator>();
        playerCtrl = player.GetComponent<PlayerCtrl>();
        cameraFollow = camera.GetComponent<CameraFollow>();
        cameraFollow.enabled = false;

        //背景滚动
        bgMat = bg.GetComponent<MeshRenderer>().sharedMaterial;
        bgMat.SetFloat("_ScrollSpeed", 0f);

        groundDistance = ground[1].transform.position.x - ground[0].transform.position.x;
        halfWinSize = 2 * camera.ViewportToWorldPoint(new Vector3(1f, 0, 0)).x;
    }
	
	// Update is called once per frame
	void Update () {
        if (gameIsStart)
        {
            if (camera.transform.position.x - ground[0].transform.position.x >= groundDistance)
            {
                ground[0].transform.position = ground[1].transform.position + Vector3.right * groundDistance;
            }
            if (camera.transform.position.x - ground[1].transform.position.x >= groundDistance)
            {
                ground[1].transform.position = ground[0].transform.position + Vector3.right * groundDistance;
            }
        }
        

        //若障碍物超出屏幕 则移除
        for(int index = 0; index < obstacles.Count; index++)
        {
            GameObject obs = obstacles[index] as GameObject;
            if (obs.transform.position.x + halfWinSize <= camera.transform.position.x)
            {
                obstacles.Remove(obs);
                GameObject.Destroy(obs, 1.0f);
            }
        }
    }
    IEnumerator CreateObstacle()
    {
        while (gameIsStart)
        {
            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
            int index = Random.Range(0, obstaclePrefabs.Length - 1);
            Vector3 pos = new Vector3(camera.transform.position.x + halfWinSize, camera.transform.position.y, 0);
            GameObject obs = Instantiate(obstaclePrefabs[index], pos, obstaclePrefabs[index].transform.rotation) as GameObject;
            obstacles.Add(obs);
        }
    }
    //开始/暂停游戏
    public void StartGame()
    {
        gameIsStart = true;
        menu.SetActive(false); //隐藏菜单
        playerCtrl.Ready(); //准备开始
        cameraFollow.enabled = true;
        bgMat.SetFloat("_ScrollSpeed", 0.02f);
        dialog.SetActive(false);

        StartCoroutine(CreateObstacle());
    }
    public void GameOver(string gameOverText)
    {
        gameIsStart = false;
        StopCoroutine(CreateObstacle());
        bgMat.SetFloat("_ScrollSpeed", 0f);
        StartCoroutine(ShowDialog(gameOverText));
    }
    IEnumerator ShowDialog(string text)
    {
        yield return new WaitForSeconds(1.0f);
        //显示游戏结束对话框\
        dialog.SetActive(true);
        dialog.GetComponentInChildren<Text>().text = text;
    }
    public void OnMoreButtonClick()
    {
        bool b_more_button = menuAnim.GetBool("b_more_button");
        menuAnim.SetBool("b_more_button", !b_more_button);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnTopButtonClick()
    {
        topDialog.SetActive(!topDialog.activeSelf);
        if(topDialog.activeSelf)
        {
            topDialog.GetComponentInChildren<Text>().text = string.Format("NO.1 : {0}", PlayerPrefs.GetInt("Score"));
        }
    }
    public void OnTopDialogCloseButtonClick()
    {
        topDialog.SetActive(false);
    }
}
