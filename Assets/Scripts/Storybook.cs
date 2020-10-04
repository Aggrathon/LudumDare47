using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Storybook : MonoBehaviour
{

    [SerializeField] Button nextButton;
    [SerializeField] Button prevButton;

    int currentPanel;
    bool eatFirstNext;

    void Start()
    {
        nextButton.onClick.AddListener(Continue);
        prevButton.onClick.AddListener(Previous);
        prevButton.gameObject.SetActive(false);
        currentPanel = 0;
        transform.GetChild(currentPanel).gameObject.SetActive(true);
        for (int i = currentPanel + 1; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        eatFirstNext = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            Previous();
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            if (eatFirstNext)
                eatFirstNext = false;
            else
                Continue();
        }
    }

    public void Continue()
    {
        if (currentPanel < transform.childCount - 2)
        {
            prevButton.gameObject.SetActive(true);
            transform.GetChild(currentPanel).gameObject.SetActive(false);
            currentPanel++;
            transform.GetChild(currentPanel).gameObject.SetActive(true);
        }
        else
        {
            int nextScene = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }

    public void Previous()
    {
        if (currentPanel > 0)
        {
            transform.GetChild(currentPanel).gameObject.SetActive(false);
            currentPanel--;
            transform.GetChild(currentPanel).gameObject.SetActive(true);
            if (currentPanel == 0)
                prevButton.gameObject.SetActive(false);
        }
    }
}
