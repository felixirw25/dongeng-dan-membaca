using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SusunKataManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public static SusunKataManager wordDragged;
    [SerializeField] TextMeshProUGUI wordDisplay;
    private bool hint, filled;
    private Vector3 startPosition;
    private Transform parentPosition;
    private Transform startSlot;
    float damage;
    public string Word { get ; private set; }
    private AcakKataManager acakKataManager;
    private bool isDropped = false;

    public void Initialize(Transform parent, string word, bool hint, AcakKataManager acakKataManager){
        Word = word;
        transform.SetParent(parent);
        this.hint = hint;
        this.acakKataManager = acakKataManager;
        GetComponent<CanvasGroup>().alpha = hint ? 0.5f : 1f;
        if(this.hint)
            wordDisplay.SetText("");
        else
            wordDisplay.SetText(Word);
    }

    public void Match(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        hint = true;
        isDropped = true;
    }

    private bool CheckIfParentHasChild()
    {
        Transform parentTransform = GameObject.FindGameObjectWithTag("startSlot").transform;
        if(parentTransform.childCount == 0){
            return true;
        }
        else{
            return false;
        }
    }

    private void ReturnWordsToStartSlot()
    {
        Debug.Log("Berhasil Hapus");
        Transform answerTransform = GameObject.FindGameObjectWithTag("answerSlot").transform;
        foreach (Transform child in answerTransform)
        {
            Destroy(child.gameObject);
        }
        AcakKata.Instance.InitKata(acakKataManager.currentQuiz.Answer);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if(hint || isDropped)
            return;
        
        startPosition = transform.position;
        parentPosition = transform.parent;
        wordDragged = this;
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Dapat merespon sentuhan
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if(hint || isDropped)
            return;
        
        transform.position = Input.mousePosition;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (hint && !filled){
            wordDragged.Match(transform);
            filled = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().interactable = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            bool allWordsDropped = CheckIfParentHasChild();
            if(wordDragged.Word == Word){
                AcakKata.Instance.TambahPoin();
            }
            
            if(allWordsDropped && AcakKata.Instance.poin!=AcakKata.Instance.poinKata){
                AcakKata.Instance.ResetPoin();
                this.acakKataManager.timer -= acakKataManager.quizTypes.Damage;
                acakKataManager.Wrong();
                ReturnWordsToStartSlot();
            }
        }
    }


    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if(hint)
            return;
        
        wordDragged = null;

        if(transform.parent == parentPosition){
            transform.position = startPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}