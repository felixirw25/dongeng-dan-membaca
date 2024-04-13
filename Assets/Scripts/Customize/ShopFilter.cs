using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopFilter : MonoBehaviour
{
    [SerializeField] ShopGenerator shopGenerator;
    [SerializeField] GameObject kontenPakaian;

    public void FilterAksesoris(){
        kontenPakaian = GameObject.FindGameObjectWithTag("KontenPakaian");
        
        for(int i = 0; i < shopGenerator.listPakaianInfo.Count; i++){
            if(shopGenerator.listPakaianInfo[i].Tipe=="baju"){
                kontenPakaian.transform.GetChild(i).gameObject.SetActive(false);
            }
            else{
                kontenPakaian.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void FilterBaju(){
        kontenPakaian = GameObject.FindGameObjectWithTag("KontenPakaian");
        
        for(int i = 0; i < shopGenerator.listPakaianInfo.Count; i++){
            if(shopGenerator.listPakaianInfo[i].Tipe=="acc"){
                kontenPakaian.transform.GetChild(i).gameObject.SetActive(false);
            }
            else{
                kontenPakaian.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void FilterAll(){
        kontenPakaian = GameObject.FindGameObjectWithTag("KontenPakaian");
        
        for(int i = 0; i < shopGenerator.listPakaianInfo.Count; i++){
            kontenPakaian.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
