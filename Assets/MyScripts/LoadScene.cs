using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���J����
/// </summary>
public class LoadScene : MonoBehaviour
{
    static LoadScene instance;
    static AsyncOperation ao;//���J����

    static Image background;//���J�I��
    static Image loadBack_Image;//���J�i�ױ�(�I��)
    static Image loadFront_Image;//���J�i�ױ�(�i��)
    static float loadValue;//���J�i��

    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    void Start()
    {
        //���J�I��
        background = ExtensionMethods.FindAnyChild<Image>(transform, "Background_Image");
        background.sprite = Resources.Load<Sprite>("Texture/LoadBackground");
        background.enabled = false;

        //���J�i�ױ�(�I��)
        loadBack_Image = ExtensionMethods.FindAnyChild<Image>(transform, "LoadBack_Image");
        loadBack_Image.enabled = false;

        //���J�i�ױ�(�i��)
        loadFront_Image = ExtensionMethods.FindAnyChild<Image>(transform, "LoadFront_Image");
        loadFront_Image.enabled = false;
    }

    void Update()
    {
        if (loadFront_Image.enabled && loadFront_Image.rectTransform.localScale.x < loadValue)
        {
            if (loadValue >= 1) loadValue = 1;
            loadFront_Image.rectTransform.localScale = new Vector3(loadFront_Image.rectTransform.localScale.x + Time.deltaTime, 1, 1);//�i�ױ�
        }
    }

    /// <summary>
    /// ���J����
    /// </summary>
    /// <param name="path">�����W��</param>
    /// <returns></returns>
    public static IEnumerator OnLoadScene(string scene)
    {  
        //�}��UI
        background.enabled = true;
        loadBack_Image.enabled = true;
        loadFront_Image.enabled = true;

        ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);//���J����
        if (ao == null) yield break;//�S�����H�U����

        ao.allowSceneActivation = false;//���J�����۰ʤ���

        while (!ao.isDone)//���J������
        {
            loadValue = 0.5f;
            if (ao.progress > 0.89f)//���J�i��
            {
                loadValue = 0.7f;
                yield return new WaitForSeconds(0.3f);

                loadValue = 0.9f;
                yield return new WaitForSeconds(0.3f);

                loadValue = 1.0f;
                yield return new WaitForSeconds(0.3f);

                //����UI
                background.enabled = false;
                loadBack_Image.enabled = false;
                loadFront_Image.enabled = false;

                //�i�J����
                ao.allowSceneActivation = true;
            }
            yield return 0;
        }
        yield return 0;
    }
}