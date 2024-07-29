using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZstdSharp.Unsafe;

public class IntroManager : MonoBehaviour
{
    public GameObject MaleCha;
    public GameObject FemaleCha;
    private Animator _animator;
    void Start()
    {
        int _genderNum = PersonalManager.Instance.ChoiceGender();
        if (_genderNum == 1)
        {
            MaleCha.gameObject.SetActive(true);
            FemaleCha.gameObject.SetActive(false);
            _animator = MaleCha.GetComponent<Animator>();
        }
        else if (_genderNum == 2)
        {
            FemaleCha.gameObject.SetActive(true);
            MaleCha.gameObject.SetActive(false);
            _animator = FemaleCha.GetComponent<Animator>();
        }

        StartCoroutine(Walk_Coroutine());

    }

    IEnumerator Walk_Coroutine()
    {
        yield return new WaitForSeconds(1f);
        _animator.SetBool("Walk", true);
        yield return new WaitForSeconds(3f);
        _animator.SetBool("Walk", false);
    }
}
